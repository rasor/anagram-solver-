using anagramsolver.containers;
using anagramsolver.models;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace anagramsolver.helpers
{
    public class LoopSetsOf2WordsHelper: LoopSetsBase
    {
        public LoopSetsOf2WordsHelper(Action<string> ConsoleWriteLine, MD5 Md5HashComputer,
            AnagramContainer AnagramCtrl, WordlistContainer WordlistCtrl) : 
            base(ConsoleWriteLine, Md5HashComputer, AnagramCtrl, WordlistCtrl)
        { }

        /// <summary>
        /// Pseudo:
        /// Create permutationsets-loop-algoritm.
        /// In the loop do
        /// - Foreach set (of two words)
        /// -- If set 1000 has been reached print the set number and the set words
        /// -- Loop permuatations (AB and BA, when words are only two)
        /// --- Validate A+B against anagram
        /// --- If valid then check "A B" md5 against all 3 md5 solutions
        /// ---- If found then remove the md5 from the list, so there only will be two to check against
        /// ----- and return the found sentense ("A B")
        /// </summary>
        public int LoopSetsOf2WordsDoValidateAndCheckMd5()
        {
            int numberOfJackpots = 0;
            UInt64 combinationCounter = 0; // max 18.446.744.073.709.551.615 .... yarn
            UInt64 subsetCounter = 0; // count number of combinations that is also subset of anagram
            // The program finds Combinations: 1623 having Subsets: 0 from the wordlist

            var tableToLoopThrough = _wordlistCtrl.TableByWordLength;
            var totalLetters = _anagramCtrl.Anagram.RawDataWithoutSpace.Length; //18
            var hasUnEvenChars = totalLetters % 2; //if even the then the middle words are both first and last word - so that row in the table needs special looping
            var middleWordLetters = (totalLetters + hasUnEvenChars) / 2;

            // Set initial set - [1, 17] - a total of 18 chars
            CurrentSetOfTwoPos currentSet = new CurrentSetOfTwoPos(totalLetters);
            // Loop initial set - [1, 17]
            numberOfJackpots += Loop2WordCombinationsInCurrentSet(currentSet, ref combinationCounter, ref subsetCounter);
            // Continue with the rest of the sets - downto set [9, 9]
            while (currentSet.SetNextSet())
            {
                numberOfJackpots += Loop2WordCombinationsInCurrentSet(currentSet, ref combinationCounter, ref subsetCounter);
            }
            _consoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter + ". No more sets");

            return numberOfJackpots;
        }

        private int Loop2WordCombinationsInCurrentSet(CurrentSetOfTwoPos currentSetLength, ref ulong combinationCounter, ref ulong subsetCounter)
        {
            int numberOfJackpots = 0;
            _consoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter + ". CurrentSet: " + currentSetLength.ToString());

            var listOfPointersToWordLong =  _tableByWordLength[currentSetLength.Word2Length];
            var listOfPointersToWordShort = _tableByWordLength[currentSetLength.Word1Length];

            // Since we know that there won't be any long words before len = 11, then we make the outer loop pass those 0 values first
            foreach (var wordLongPointer in listOfPointersToWordLong)
            {
                foreach (var wordShortPointer in listOfPointersToWordShort)
                {
                    // ConsoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter);

                    var wordShortRow = _wordlistCtrl.TableFilter2_WordMatrix[wordShortPointer];
                    var wordLongRow = _wordlistCtrl.TableFilter2_WordMatrix[wordLongPointer];
                    var combinedWordToValidate = CombineRows(wordShortRow, wordLongRow);
                    var isSubset = _anagramCtrl.IsSubset(combinedWordToValidate);

                    // Do MD5 check if the two words combined is still a subset of anagram
                    bool gotJackpot = false;
                    if (isSubset)
                    {
                        subsetCounter++;
                        var wordShort = _wordlistCtrl.ListFilter1_WorddictHavingAllowedChars.Keys.ElementAt(wordShortPointer);
                        var wordLong = _wordlistCtrl.ListFilter1_WorddictHavingAllowedChars.Keys.ElementAt(wordLongPointer);

                        // did we get lucky?
                        gotJackpot = checkMd5(ref numberOfJackpots, wordShort + " " + wordLong);

                        if (!gotJackpot)
                        {
                            // did we get lucky with reverse set?
                            gotJackpot = checkMd5(ref numberOfJackpots, wordLong + " " + wordShort);
                        }
                    }

                    combinationCounter++;
                }
            }
            return numberOfJackpots;
        }

        /// <summary>
        /// Add number of each letter of two words, 
        /// so the sum can be compared with the sum in the anagram
        /// </summary>
        /// <param name="row1">number of each letter in word1</param>
        /// <param name="row2">number of each letter in word2</param>
        /// <returns>number of each letter in both words</returns>
        private int[] CombineRows(int[] row1, int[] row2)
        {
            // Make a copy of row2
            int[] combinedRow = (int[])row2.Clone();

            // Word is stored from col3 onwards - loop it.
            // Col1 is number of chars
            for (int i = 1; i < row1.Length - 1; i++)
            {
                // Add row1 to row2
                combinedRow[i] += row1[i];
            }

            return combinedRow;
        }
    }
}

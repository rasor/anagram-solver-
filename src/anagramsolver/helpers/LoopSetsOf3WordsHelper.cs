using anagramsolver.containers;
using anagramsolver.models;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace anagramsolver.helpers
{
    public class LoopSetsOf3WordsHelper: LoopSetsBase
    {
        public LoopSetsOf3WordsHelper(Action<string> ConsoleWriteLine, MD5 Md5HashComputer,
            AnagramContainer AnagramCtrl, WordlistContainer WordlistCtrl) :
            base(ConsoleWriteLine, Md5HashComputer, AnagramCtrl, WordlistCtrl)
        { }

        public int LoopSetsOf3WordsDoValidateAndCheckMd5(int numberOfJackpots)
        {
            UInt64 combinationCounter = 0; // max 18.446.744.073.709.551.615 .... yarn
            UInt64 subsetCounter = 0; // count number of combinations that is also subset of anagram
            // If the program does not check md5 if finds Combinations: 83.743.632 having Subsets: 5672 from the wordlist

            var tableToLoopThrough = _wordlistCtrl.TableByWordLength;
            var totalLetters = _anagramCtrl.Anagram.RawDataWithoutSpace.Length; //18
            var hasUnEvenChars = totalLetters % 2; //if even the then the middle words are both first and last word - so that row in the table needs special looping
            var middleWordLetters = (totalLetters + hasUnEvenChars) / 2;

            // Set initial set - [1, 1, 16] - a total of 18 chars
            CurrentSetOfThreePos currentSetLength = new CurrentSetOfThreePos(totalLetters);
            // Loop initial set - [1, 1, 16]
            numberOfJackpots += Loop3WordCombinationsInCurrentSet(currentSetLength, ref combinationCounter, ref subsetCounter);
            //_consoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter + ". CurrentSet: " + currentSetLength.ToString());
            // Continue with the rest of the sets - downto set [9, 9]
            while (currentSetLength.SetNextSet())
            {
                numberOfJackpots += Loop3WordCombinationsInCurrentSet(currentSetLength, ref combinationCounter, ref subsetCounter);
                //_consoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter + ". CurrentSet: " + currentSetLength.ToString());
            }
            _consoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter + ". No more sets");

            return numberOfJackpots;
        }

        private int Loop3WordCombinationsInCurrentSet(CurrentSetOfThreePos currentSetLength, ref ulong combinationCounter, ref ulong subsetCounter)
        {
            int numberOfJackpots = 0;
            _consoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter + ". CurrentSet: " + currentSetLength.ToString());

            var listOfPointersToWord3 = _tableByWordLength[currentSetLength.Word3Length];
            var listOfPointersToWord2 = _tableByWordLength[currentSetLength.Word2Length];
            var listOfPointersToWord1 = _tableByWordLength[currentSetLength.Word1Length];

            // Since we know that there won't be any long words before len = 11, then we make the outer loop pass those 0 values first
            foreach (var word3Pointer in listOfPointersToWord3)
            {
                foreach (var word2Pointer in listOfPointersToWord2)
                {
                    foreach (var word1Pointer in listOfPointersToWord1)
                    {
                        // ConsoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter);

                        var word1Row = _wordlistCtrl.TableFilter2_WordMatrix[word1Pointer];
                        var word2Row = _wordlistCtrl.TableFilter2_WordMatrix[word2Pointer];
                        var word3Row = _wordlistCtrl.TableFilter2_WordMatrix[word3Pointer];
                        var combinedWordToValidate = CombineRows(word1Row, word2Row, word3Row);
                        var isSubset = _anagramCtrl.IsSubset(combinedWordToValidate);

                        // Do MD5 check if the two words combined is still a subset of anagram
                        bool gotJackpot = false;
                        if (isSubset)
                        {
                            subsetCounter++;
                            var word1 = _wordlistCtrl.ListFilter1_WorddictHavingAllowedChars.Keys.ElementAt(word1Pointer);
                            var word2 = _wordlistCtrl.ListFilter1_WorddictHavingAllowedChars.Keys.ElementAt(word2Pointer);
                            var word3 = _wordlistCtrl.ListFilter1_WorddictHavingAllowedChars.Keys.ElementAt(word3Pointer);

                            // did we get lucky?
                            gotJackpot = checkMd5(ref numberOfJackpots, word1 + " " + word2 + " " + word3);

                            //if (!gotJackpot)
                            //{
                            //    // did we get lucky with reverse set?
                            //    gotJackpot = checkMd5(ref numberOfJackpots, word3 + " " + word2);
                            //}
                        }

                        combinationCounter++;
                    }
                }
            }
            return numberOfJackpots;
        }

        /// <summary>
        /// Add number of each letter of three words, 
        /// so the sum can be compared with the sum in the anagram
        /// </summary>
        /// <param name="row1">number of each letter in word1</param>
        /// <param name="row2">number of each letter in word2</param>
        /// <param name="row3">number of each letter in word3</param>
        /// <returns>number of each letter in both words</returns>
        private int[] CombineRows(int[] row1, int[] row2, int[] row3)
        {
            // Make a copy of row3
            int[] combinedRow = (int[])row3.Clone();

            // Word is stored from col3 onwards - loop it.
            // Col1 is number of chars
            for (int i = 1; i < row1.Length - 1; i++)
            {
                // Add row1 and row2 to row3
                combinedRow[i] += (row1[i]+ row2[i]);
            }

            return combinedRow;
        }
    }
}

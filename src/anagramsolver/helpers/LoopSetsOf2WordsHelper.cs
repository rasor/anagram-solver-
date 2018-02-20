using anagramsolver.containers;
using anagramsolver.models;
using System;
using System.Collections.Generic;
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

            CurrentSetOf2Pos currentSetLength = new CurrentSetOf2Pos(totalLetters);
            // Loop sets - [1, 17] - downto set [9, 9]
            while (currentSetLength.SetNextSet())
            {
                numberOfJackpots += Loop2WordCombinationsInCurrentSet(currentSetLength, ref combinationCounter, ref subsetCounter);
                //_consoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter + ". NextSet: " + currentSetLength.ToString());
            }
            _consoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter + ". No more sets");

            return numberOfJackpots;
        }

        private int Loop2WordCombinationsInCurrentSet(CurrentSetOf2Pos currentSetLength, ref ulong combinationCounter, ref ulong subsetCounter)
        {
            int numberOfJackpots = 0;
            _consoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter + ". NextSet: " + currentSetLength.ToString());

            var listOfPointersToWordLong =  _tableByWordLength[currentSetLength.Word2Length];
            var listOfPointersToWordShort = _tableByWordLength[currentSetLength.Word1Length];

            // List to avoid checking same sentence twice
            HashSet<int[]> uniqueListOfSentencesHavingWordsWithSameLength = new HashSet<int[]>(new ArrayComparer());
            ulong uniqueListOfSentencesHavingWordsWithSameLengthCounter = 0;
            ulong skippedChecksCounter = 0;

            // Since we know that there won't be any long words before len = 11, then we make the outer loop pass those 0 values first
            foreach (var word2Pointer in listOfPointersToWordLong)
            {
                foreach (var word1Pointer in listOfPointersToWordShort)
                {
                    // ConsoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter);

                    var word1Row = _wordlistCtrl.TableFilter2_WordMatrix[word1Pointer];
                    var word2Row = _wordlistCtrl.TableFilter2_WordMatrix[word2Pointer];
                    var combinedWordToValidate = CombineRows(word1Row, word2Row);
                    var isSubset = _anagramCtrl.IsSubset(combinedWordToValidate);

                    // Do MD5 check if the two words combined is still a subset of anagram
                    bool gotJackpot = false;
                    if (isSubset)
                    {
                        subsetCounter++;

                        // Now that we are down to the few sentences that are also subsets, then we'll keep them in an ordered unique list,
                        // So those sentences having same words are not checked more than once
                        if (currentSetLength.AnyOfSameLength)
                        {
                            var currentSentence = new int[] { word1Pointer, word2Pointer};
                            Array.Sort(currentSentence);
                            // If we don't have that sentence, then do md5Check
                            if (!uniqueListOfSentencesHavingWordsWithSameLength.Contains(currentSentence))
                            {
                                uniqueListOfSentencesHavingWordsWithSameLengthCounter++;
                                uniqueListOfSentencesHavingWordsWithSameLength.Add(currentSentence);

                                gotJackpot = FetchWordsAndCheckMd5(ref numberOfJackpots, word2Pointer, word1Pointer);
                            }
                            else {
                                skippedChecksCounter++;
                            }
                        }
                        // No words of same lenght, so just do check
                        else
                        {
                            gotJackpot = FetchWordsAndCheckMd5(ref numberOfJackpots, word2Pointer, word1Pointer);
                        }
                    }

                    combinationCounter++;
                }
            }
            if (uniqueListOfSentencesHavingWordsWithSameLengthCounter > 0)
            {
                _consoleWriteLine("  UniqueListOfSentencesHavingWordsWithSameLength: " + uniqueListOfSentencesHavingWordsWithSameLengthCounter + ". SkippedChecks: " + skippedChecksCounter);
            }
            return numberOfJackpots;
        }

        private bool FetchWordsAndCheckMd5(ref int numberOfJackpots, int wordLongPointer, int wordShortPointer)
        {
            var word1 = _wordlistCtrl.ListFilter1_WorddictHavingAllowedChars.Keys.ElementAt(wordShortPointer);
            var word2 = _wordlistCtrl.ListFilter1_WorddictHavingAllowedChars.Keys.ElementAt(wordLongPointer);

            bool gotJackpot = false;
            // did we get lucky?
            if (!gotJackpot) { gotJackpot = checkMd5(ref numberOfJackpots, string.Format("{0} {1}", word1, word2)); }
            if (!gotJackpot) { gotJackpot = checkMd5(ref numberOfJackpots, string.Format("{1} {0}", word1, word2)); }

            return gotJackpot;
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

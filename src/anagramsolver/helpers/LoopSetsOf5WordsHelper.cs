using anagramsolver.containers;
using anagramsolver.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace anagramsolver.helpers
{
    public class LoopSetsOf5WordsHelper: LoopSetsBase
    {
        public LoopSetsOf5WordsHelper(Action<string> ConsoleWriteLine, MD5 Md5HashComputer,
            AnagramContainer AnagramCtrl, WordlistContainer WordlistCtrl) :
            base(ConsoleWriteLine, Md5HashComputer, AnagramCtrl, WordlistCtrl)
        { }

        public int LoopSetsOf5WordsDoValidateAndCheckMd5(int numberOfJackpots)
        {
            UInt64 combinationCounter = 0; // max 18.446.744.073.709.551.615 .... yarn
            UInt64 subsetCounter = 0; // count number of combinations that is also subset of anagram
            // If the program does not check md5 if finds Combinations: 83.743.632 having Subsets: 5672 from the wordlist

            var tableToLoopThrough = _wordlistCtrl.TableByWordLength;
            var totalLetters = _anagramCtrl.Anagram.RawDataWithoutSpace.Length; //18
            var hasUnEvenChars = totalLetters % 2; //if even the then the middle words are both first and last word - so that row in the table needs special looping
            var middleWordLetters = (totalLetters + hasUnEvenChars) / 2;

            CurrentSetOf5Pos currentSetLength = new CurrentSetOf5Pos(totalLetters);
            // Loop sets - [1, 1, 1, 1, 14] - downto set [3, 3, 4, 4, 4] - 3,3,3,4,5
            while (currentSetLength.SetNextSet())
            {
                numberOfJackpots += Loop5WordCombinationsInCurrentSet(currentSetLength, ref combinationCounter, ref subsetCounter);
                //_consoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter + ". NextSet: " + currentSetLength.ToString());
            }
            _consoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter + ". No more sets");

            return numberOfJackpots;
        }

        private int Loop5WordCombinationsInCurrentSet(CurrentSetOf5Pos currentSetLength, ref ulong combinationCounter, ref ulong subsetCounter)
        {
            // List from [0,1,2,3,4] to [4,3,2,1,0] = 120 permutations - used for swapping order of words in sentence
            int[] permutationValues = new int[] { 0, 1, 2, 3, 4 };
            var listOfWordPermutations = PermutationsCreator.GetPermutations(permutationValues, permutationValues.Length);
            // Convert to a list for string.Format: "{0} {1} {2} {3} {4}"
            var listOfWordPermutationsReplacementString = PermutationsCreator.ToReplacementString(listOfWordPermutations).ToArray(); ;

            int numberOfJackpots = 0;

            var listOfPointersToWord5 = _tableByWordLength[currentSetLength.Word5Length];
            var listOfPointersToWord4 = _tableByWordLength[currentSetLength.Word4Length];
            var listOfPointersToWord3 = _tableByWordLength[currentSetLength.Word3Length];
            var listOfPointersToWord2 = _tableByWordLength[currentSetLength.Word2Length];
            var listOfPointersToWord1 = _tableByWordLength[currentSetLength.Word1Length];

            long currentSetCombinations = listOfPointersToWord1.Count * listOfPointersToWord2.Count * listOfPointersToWord3.Count * listOfPointersToWord4.Count * listOfPointersToWord5.Count;
            _consoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter + ". NextSet: " + currentSetLength.ToString() + " having " + currentSetCombinations + " combinations");

            // List to avoid checking same sentence twice
            HashSet<int[]> uniqueListOfSentencesHavingWordsWithSameLength = new HashSet<int[]>(new ArrayComparer());
            ulong uniqueListOfSentencesHavingWordsWithSameLengthCounter = 0;
            ulong skippedChecksCounter = 0;

            // Since we know that there won't be any long words before len = 11, then we make the outer loop pass those 0 values first
            foreach (var word5Pointer in listOfPointersToWord5)
            {
                foreach (var word4Pointer in listOfPointersToWord4)
                {
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
                                var word4Row = _wordlistCtrl.TableFilter2_WordMatrix[word4Pointer];
                                var word5Row = _wordlistCtrl.TableFilter2_WordMatrix[word5Pointer];
                                var combinedWordToValidate = CombineRows(word1Row, word2Row, word3Row, word4Row, word5Row);
                                var isSubset = _anagramCtrl.IsSubset(combinedWordToValidate);

                                // Do MD5 check if the two words combined is still a subset of anagram
                                bool gotJackpot = false;
                                if (isSubset)
                                {
                                    subsetCounter++;

                                    // Now that we are down to the few sentences that are also subsets, then we'll keep them in an ordered unique list,
                                    // So those sentences having same words are not checked more than once
                                    //if (currentSetLength.AnyOfSameLength)
                                    //{
                                    //    var currentSentence = new int[] { word1Pointer, word2Pointer, word3Pointer, word4Pointer, word5Pointer };
                                    //    Array.Sort(currentSentence);
                                    //    // If we don't have that sentence, then do md5Check
                                    //    if (!uniqueListOfSentencesHavingWordsWithSameLength.Contains(currentSentence))
                                    //    {
                                    //        uniqueListOfSentencesHavingWordsWithSameLengthCounter++;
                                    //        uniqueListOfSentencesHavingWordsWithSameLength.Add(currentSentence);

                                    //        gotJackpot = FetchWordsAndCheckMd5(ref numberOfJackpots, word1Pointer, word2Pointer, word3Pointer, word4Pointer, word5Pointer, listOfWordPermutationsReplacementString);
                                    //    }
                                    //    else
                                    //    {
                                    //        skippedChecksCounter++;
                                    //    }
                                    //}
                                    //// No words of same lenght, so just do check
                                    //else
                                    {
                                        gotJackpot = FetchWordsAndCheckMd5(ref numberOfJackpots, word1Pointer, word2Pointer, word3Pointer, word4Pointer, word5Pointer, listOfWordPermutationsReplacementString);
                                    }
                                }
                                combinationCounter++;
                            }
                        }
                    }
                }
            }
            if (uniqueListOfSentencesHavingWordsWithSameLengthCounter > 0)
            {
                _consoleWriteLine("  UniqueListOfSentencesHavingWordsWithSameLength: " + uniqueListOfSentencesHavingWordsWithSameLengthCounter + ". SkippedChecks: " + skippedChecksCounter);
            }
            return numberOfJackpots;
        }

        private bool FetchWordsAndCheckMd5(ref int numberOfJackpots, int word1Pointer, int word2Pointer, int word3Pointer, int word4Pointer, int word5Pointer, string[] listOfWordPermutationsReplacementString)
        {
            var word1 = _wordlistCtrl.ListFilter1_WorddictHavingAllowedChars.Keys.ElementAt(word1Pointer);
            var word2 = _wordlistCtrl.ListFilter1_WorddictHavingAllowedChars.Keys.ElementAt(word2Pointer);
            var word3 = _wordlistCtrl.ListFilter1_WorddictHavingAllowedChars.Keys.ElementAt(word3Pointer);
            var word4 = _wordlistCtrl.ListFilter1_WorddictHavingAllowedChars.Keys.ElementAt(word4Pointer);
            var word5 = _wordlistCtrl.ListFilter1_WorddictHavingAllowedChars.Keys.ElementAt(word5Pointer);

            bool gotJackpot = LoopPermutationsAndCheckMd5(ref numberOfJackpots, word1, word2, word3, word4, word5, listOfWordPermutationsReplacementString);
            return gotJackpot;
        }

        /// <summary>
        /// When this method is called we know that the characters in the sentence match (isSubset of) anagram
        /// In here we loop through the order of the words and check md5
        /// </summary>
        /// <param name="numberOfJackpots"></param>
        /// <param name="word1"></param>
        /// <param name="word2"></param>
        /// <param name="word3"></param>
        /// <param name="word4"></param>
        /// <param name="word5"></param>
        /// <returns></returns>
        private bool LoopPermutationsAndCheckMd5(ref int numberOfJackpots, string word1, string word2, string word3, string word4, string word5, string[] listOfWordPermutationsReplacementString)
        {
            bool gotJackpot = false;
            // did we get lucky? - loop permutations of the words in the sentence
            foreach (var permutationReplacementString in listOfWordPermutationsReplacementString)
            {
                if (!gotJackpot)
                {
                    gotJackpot = checkMd5(ref numberOfJackpots, string.Format(permutationReplacementString, word1, word2, word3, word4, word5));
                }
                else
                {
                    break;
                }
            }
            return gotJackpot;
        }

        /// <summary>
        /// Add number of each letter of three words, 
        /// so the sum can be compared with the sum in the anagram
        /// </summary>
        /// <param name="row1">number of each letter in word1</param>
        /// <param name="row2">number of each letter in word2</param>
        /// <param name="row3">number of each letter in word3</param>
        /// <param name="row4">number of each letter in word4</param>
        /// <param name="row5">number of each letter in word5</param>
        /// <returns>number of each letter in both words</returns>
        private int[] CombineRows(int[] row1, int[] row2, int[] row3, int[] row4, int[] row5)
        {
            // Make a copy of row3
            int[] combinedRow = (int[])row5.Clone();

            // Word is stored from col3 onwards - loop it.
            // Col1 is number of chars
            for (int i = 1; i < row1.Length - 1; i++)
            {
                // Add row1 and row2 to row3
                combinedRow[i] += (row1[i]+ row2[i] + row3[i] + row4[i]);
            }

            return combinedRow;
        }
    }
}

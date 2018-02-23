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

            ulong currentSetCombinations = (ulong)(listOfPointersToWord1.Count * listOfPointersToWord2.Count * listOfPointersToWord3.Count * listOfPointersToWord4.Count * listOfPointersToWord5.Count);
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
                                var rows = new int[][] { word1Row, word2Row, word3Row, word4Row, word5Row };
                                var combinedWordToValidate = CombineRows(rows);
                                var isSubset = _anagramCtrl.IsSubset(combinedWordToValidate);

                                // Do MD5 check if the two words combined is still a subset of anagram
                                bool gotJackpot = false;
                                if (isSubset)
                                {
                                    subsetCounter++;
                                    // Put words in a list, so they can be passed on as a collection
                                    int[] currentSentence = new int[] { word1Pointer, word2Pointer, word3Pointer, word4Pointer, word5Pointer };

                                    // Now that we are down to the few sentences that are also subsets, then we'll keep them in an ordered unique list,
                                    // So those sentences having same words are not checked more than once
                                    //if (currentSetLength.AnyOfSameLength)
                                    //{
                                    //    Array.Sort(currentSentence);
                                    //    // If we don't have that sentence, then do md5Check
                                    //    if (!uniqueListOfSentencesHavingWordsWithSameLength.Contains(currentSentence))
                                    //    {
                                    //        uniqueListOfSentencesHavingWordsWithSameLengthCounter++;
                                    //        uniqueListOfSentencesHavingWordsWithSameLength.Add(currentSentence);

                                    //        gotJackpot = FetchWordsAndCheckMd5(ref numberOfJackpots, currentSentence, listOfWordPermutationsReplacementString);
                                    //    }
                                    //    else
                                    //    {
                                    //        skippedChecksCounter++;
                                    //    }
                                    //}
                                    //// No words of same lenght, so just do check
                                    //else
                                    {
                                        gotJackpot = FetchWordsAndCheckMd5(ref numberOfJackpots, currentSentence, listOfWordPermutationsReplacementString);
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
    }
}

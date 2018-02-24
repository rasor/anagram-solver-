using anagramsolver.containers;
using anagramsolver.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace anagramsolver.services
{
    public class LoopSetsOf3WordsHelper: LoopSetsBase
    {
        public LoopSetsOf3WordsHelper(Action<string> ConsoleWriteLine, MD5 Md5HashComputer,
            IAnagramContainer AnagramCtrl, IWordlistContainer WordlistCtrl) :
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

            CurrentSetOf3Pos currentSetLength = new CurrentSetOf3Pos(totalLetters);
            // Loop sets - [1, 1, 16] - downto set [6, 6, 6]
            while (currentSetLength.SetNextSet())
            {
                numberOfJackpots += Loop3WordCombinationsInCurrentSet(currentSetLength, ref combinationCounter, ref subsetCounter);
                //_consoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter + ". NextSet: " + currentSetLength.ToString());
            }
            _consoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter + ". No more sets");

            return numberOfJackpots;
        }

        private int Loop3WordCombinationsInCurrentSet(CurrentSetOf3Pos currentSetLength, ref ulong combinationCounter, ref ulong subsetCounter)
        {
            // Create list with permutations for string.Format: "{0} {1} {2}" from [0,1,2] to [2,1,0] = 6 permutations
            string[] listOfWordPermutationsReplacementString = PermutationsCreator.CreateListOfWordPermutationsReplacementStrings(3);

            int numberOfJackpots = 0;

            var listOfPointersToWord3 = _tableByWordLength[currentSetLength.Word3Length];
            var listOfPointersToWord2 = _tableByWordLength[currentSetLength.Word2Length];
            var listOfPointersToWord1 = _tableByWordLength[currentSetLength.Word1Length];

            ulong currentSetCombinations = (ulong)(listOfPointersToWord1.Count * listOfPointersToWord2.Count * listOfPointersToWord3.Count);
            _consoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter + ". NextSet: " + currentSetLength.ToString() + " having " + currentSetCombinations + " combinations");

            // List to avoid checking same sentence twice
            HashSet<int[]> uniqueListOfSentencesHavingWordsWithSameLength = new HashSet<int[]>(new ArrayComparer());
            ulong uniqueListOfSentencesHavingWordsWithSameLengthCounter = 0;
            ulong skippedChecksCounter = 0;

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
                        var rows = new int[][] { word1Row, word2Row, word3Row };
                        var combinedWordToValidate = CombineRows(rows);
                        var isSubset = _anagramCtrl.IsSubset(combinedWordToValidate);

                        // Do MD5 check if the two words combined is still a subset of anagram
                        bool gotJackpot = false;
                        if (isSubset)
                        {
                            subsetCounter++;
                            // Put words in a list, so they can be passed on as a collection
                            int[] currentSentence = new int[] { word1Pointer, word2Pointer, word3Pointer };

                            // Now that we are down to the few sentences that are also subsets, then we'll keep them in an ordered unique list,
                            // So those sentences having same words are not checked more than once
                            if (currentSetLength.AnyOfSameLength)
                            {
                                Array.Sort(currentSentence);
                                // If we don't have that sentence, then do md5Check
                                if (!uniqueListOfSentencesHavingWordsWithSameLength.Contains(currentSentence))
                                {
                                    uniqueListOfSentencesHavingWordsWithSameLengthCounter++;
                                    uniqueListOfSentencesHavingWordsWithSameLength.Add(currentSentence);

                                    gotJackpot = FetchWordsAndCheckMd5(ref numberOfJackpots, currentSentence, listOfWordPermutationsReplacementString);
                                }
                                else
                                {
                                    skippedChecksCounter++;
                                }
                            }
                            // No words of same lenght, so just do check
                            else
                            {
                                gotJackpot = FetchWordsAndCheckMd5(ref numberOfJackpots, currentSentence, listOfWordPermutationsReplacementString);
                            }
                        }
                        combinationCounter++;
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

﻿using anagramsolver.containers;
using anagramsolver.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace anagramsolver.services
{
    public class LoopSetsOf2WordsHelper<TCurrentSetOfXPos> : LoopSetsBase<TCurrentSetOfXPos> where TCurrentSetOfXPos : CurrentSetOf2Pos, new()
    {
        public LoopSetsOf2WordsHelper(ConsoleLogger logger, MD5 Md5HashComputer,
            IAnagramContainer AnagramCtrl, IWordlistContainer WordlistCtrl) : 
            base(logger.ConsoleWriteLine, Md5HashComputer, AnagramCtrl, WordlistCtrl)
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

            var totalLetters = _anagramCtrl.Anagram.RawDataWithoutSpace.Length; //18
            TCurrentSetOfXPos currentSetLength = Activator.CreateInstance(typeof(TCurrentSetOfXPos), new object[] { totalLetters }) as TCurrentSetOfXPos;//new TCurrentSetOfXPos(totalLetters);
            //TCurrentSetOfXPos currentSetLength = new TCurrentSetOfXPos(totalLetters);
            // Loop sets - [1, 17] - downto set [9, 9]
            while (currentSetLength.SetNextSet() && numberOfJackpots < 3)
            {
                numberOfJackpots = LoopWordCombinationsInCurrentSet(numberOfJackpots, currentSetLength, ref combinationCounter, ref subsetCounter);
            }
            _consoleWriteLine(" Combinations: " + string.Format("{0:n0}", combinationCounter) + ". Subsets: " + string.Format("{0:n0}", subsetCounter) + ". No more sets");

            return numberOfJackpots;
        }

        protected override int LoopWordCombinationsInCurrentSet(int numberOfJackpots, TCurrentSetOfXPos currentSetLength, ref ulong combinationCounter, ref ulong subsetCounter)
        {
            // Create list with permutations for string.Format: "{0} {1}" from [0,1] to [1,0] = 2 permutations
            string[] listOfWordPermutationsReplacementString = PermutationsCreator.CreateListOfWordPermutationsReplacementStrings(2);

            var tableByWordLength = _wordlistCtrl.TableByWordLength;
            var listOfPointersToWord2 =  tableByWordLength[currentSetLength.Word2Length];
            var listOfPointersToWord1 = tableByWordLength[currentSetLength.Word1Length];

            ulong currentSetCombinations = (ulong)(listOfPointersToWord1.Count * listOfPointersToWord2.Count);
            _consoleWriteLine(" Combinations: " + string.Format("{0:n0}", combinationCounter) + ". Subsets: " + string.Format("{0:n0}", subsetCounter) + ". NextSet: " + currentSetLength.ToString() + " having " + string.Format("{0:n0}", currentSetCombinations) + " combinations");

            // List to avoid checking same sentence twice
            HashSet<int[]> uniqueListOfSentencesHavingWordsWithSameLength = new HashSet<int[]>(new ArrayComparer());
            ulong uniqueListOfSentencesHavingWordsWithSameLengthCounter = 0;
            ulong skippedChecksCounter = 0;

            // Since we know that there won't be any long words before len = 11, then we make the outer loop pass those 0 values first
            foreach (var word2Pointer in listOfPointersToWord2)
            {
                foreach (var word1Pointer in listOfPointersToWord1)
                {
                    // ConsoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter);

                    var word1Row = _wordlistCtrl.TableFilter2_WordMatrix[word1Pointer];
                    var word2Row = _wordlistCtrl.TableFilter2_WordMatrix[word2Pointer];
                    var rows = new int[][] { word1Row, word2Row };
                    var combinedWordToValidate = CombineRows(rows);
                    var isSubset = _anagramCtrl.IsSubset(combinedWordToValidate);

                    // Do MD5 check if the two words combined is still a subset of anagram
                    bool gotJackpot = false;
                    if (isSubset)
                    {
                        subsetCounter++;
                        // Put words in a list, so they can be passed on as a collection
                        int[] currentSentence = new int[] { word1Pointer, word2Pointer };

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

                                gotJackpot = FetchWordsAndCheckMd5RemoveFoundHash(ref numberOfJackpots, currentSentence, listOfWordPermutationsReplacementString);
                            }
                            else {
                                skippedChecksCounter++;
                            }
                        }
                        // No words of same lenght, so just do check
                        else
                        {
                            gotJackpot = FetchWordsAndCheckMd5RemoveFoundHash(ref numberOfJackpots, currentSentence, listOfWordPermutationsReplacementString);
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
    }
}

using anagramsolver.containers;
using anagramsolver.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace anagramsolver.services
{
    public abstract class LoopSetsBase<TCurrentSetOfXPos>: LoopSetsBase where TCurrentSetOfXPos : CurrentSetOf2Pos
    {
        public LoopSetsBase(Action<string> ConsoleWriteLine, MD5 Md5HashComputer,
            IAnagramContainer AnagramCtrl, IWordlistContainer WordlistCtrl) :
            base(ConsoleWriteLine, Md5HashComputer, AnagramCtrl, WordlistCtrl)
        {
        }

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
        public int LoopSetsOfWordsDoValidateAndCheckMd5(int numberOfJackpots, string[] remainingHashes)
        {
            // Update hashes, so there are fewer to check against, if any was found
            this.Md5Checker.Md5Hashes = remainingHashes;

            UInt64 combinationCounter = 0; // max 18.446.744.073.709.551.615 .... yarn
            UInt64 subsetCounter = 0; // count number of combinations that is also subset of anagram
            // In sets of two words: The program finds Combinations: 1623 having Subsets: 0 from the wordlist
            // In sets of three words: If the program does not check md5 if finds Combinations: 83.743.632 having Subsets: 5672 from the wordlist

            var totalLetters = _anagramCtrl.Anagram.RawDataWithoutSpace.Length; //18
            var noOfSecrets = _anagramCtrl.Md5Hashes.Length; //3
            //TCurrentSetOfXPos currentSetLength = new TCurrentSetOfXPos(totalLetters); // Not possible with parameters in generics - so instead doing as below
            TCurrentSetOfXPos currentSetLength = Activator.CreateInstance(typeof(TCurrentSetOfXPos), new object[] { totalLetters }) as TCurrentSetOfXPos;//new TCurrentSetOfXPos(totalLetters);
            // When Sentence Lenght (ex spaces) = 18 = totalLetters
            // Loop sets - [1, 17] - downto set [9, 9] when using CurrentSetOf2Pos
            // Loop sets - [1, 1, 16] - downto set [6, 6, 6] when using CurrentSetOf3Pos
            // Loop sets - [1, 1, 1, 15] - downto set [4, 4, 5, 5] when using CurrentSetOf4Pos
            // Loop sets - [1, 1, 1, 1, 14] - downto set [3, 3, 4, 4, 4] - 3,3,3,4,5 when using CurrentSetOf4Pos
            while (currentSetLength.SetNextSet() && numberOfJackpots < noOfSecrets)
            {
                numberOfJackpots = LoopWordCombinationsInCurrentSet(numberOfJackpots, currentSetLength, ref combinationCounter, ref subsetCounter);
            }
            _consoleWriteLine(" Combinations: " + string.Format("{0:n0}", combinationCounter) + ". Subsets: " + string.Format("{0:n0}", subsetCounter) + ". No more sets");

            return numberOfJackpots;
        }

        /// <summary>
        /// Main loop - loops combinations of each set
        /// </summary>
        /// <param name="numberOfJackpots">a counter of found sentences</param>
        /// <param name="currentSetLength">e.g [2, 7, 9] - 3 list of words having length 2, 7 and 9 chars</param>
        /// <param name="combinationCounter">a counter of all combinations in all sets</param>
        /// <param name="subsetCounter">a counter of all valid subsets (of the anagram) in all sets</param>
        /// <returns>numberOfJackpots</returns>
        protected virtual int LoopWordCombinationsInCurrentSet(int numberOfJackpots, TCurrentSetOfXPos currentSetLength, ref ulong combinationCounter, ref ulong subsetCounter)
        {
            var noOfWordsInSet = currentSetLength.DictOfWordLengths.Count;
            // Create list with permutations for string.Format: "{0} {1}" from [0,1] to [1,0] = 2 permutations
            // Create list with permutations for string.Format: "{0} {1} {2}" from [0,1,2] to [2,1,0] = 6 permutations
            // Create list with permutations for string.Format: "{0} {1} {2} {3}" from [0,1,2,3] to [3,2,1,0] = 24 permutations
            // Create list with permutations for string.Format: "{0} {1} {2} {3} {4}" from [0,1,2,3,4] to [4,3,2,1,0] = 120 permutations
            string[] listOfWordPermutationsReplacementString = PermutationsCreator.CreateListOfWordPermutationsReplacementStrings(noOfWordsInSet);

            var listOflistOfPointersToWords = new int[noOfWordsInSet][];
            ulong currentSetCombinations = 1;
            for (int i = 0; i < noOfWordsInSet; i++)
            {
                // for each word in the set put each listOfPointersToWords in a list
                int wordNumberInSet = i + 1;
                listOflistOfPointersToWords[i] = _wordlistCtrl.TableByWordLength[currentSetLength.DictOfWordLengths[(short)wordNumberInSet] - 1].ToArray();
                // and calculate number of combinations
                currentSetCombinations = currentSetCombinations * (ulong)(listOflistOfPointersToWords[i].Length);
            }

            _consoleWriteLine(" Combinations: " + string.Format("{0:n0}", combinationCounter) + ". Subsets: " + string.Format("{0:n0}", subsetCounter) + ". NextSet: " + currentSetLength.ToString() + " having " + string.Format("{0:n0}", currentSetCombinations) + " combinations");

            var currentSentenceIndex = -1;
            var currentSentence = new int[noOfWordsInSet];
            // Since we know that there won't be any long words before len = 11, then we make the outer loop pass those 0 values first
            recurseCombinations(currentSentence, currentSentenceIndex, ref numberOfJackpots, currentSetLength, ref combinationCounter, ref subsetCounter, 
                listOfWordPermutationsReplacementString, listOflistOfPointersToWords);

            return numberOfJackpots;
        }

        private void recurseCombinations(int[] currentSentence, int currentSentenceIndex, ref int numberOfJackpots, TCurrentSetOfXPos currentSetLength, 
            ref ulong combinationCounter, ref ulong subsetCounter, string[] listOfWordPermutationsReplacementString, int[][] listOflistOfPointersToWords)
        {
            currentSentenceIndex++;
            foreach (var wordPointer in listOflistOfPointersToWords[currentSentenceIndex])
            {
                currentSentence[currentSentenceIndex] = wordPointer;
                if (currentSentenceIndex < currentSentence.Length - 1)
                {
                    recurseCombinations(currentSentence, currentSentenceIndex, ref numberOfJackpots, currentSetLength, ref combinationCounter, ref subsetCounter, 
                        listOfWordPermutationsReplacementString, listOflistOfPointersToWords);
                }
                else
                {
                    PleaseEnterTheDeepHoleInsideLoop(currentSentence, ref numberOfJackpots, currentSetLength, ref subsetCounter, 
                        listOfWordPermutationsReplacementString);
                    combinationCounter++;
                }
            }
        }

        /// <summary>
        /// Beware - cpu time is very expensive down here, since 
        /// imagine if we have 5 words each having 100 words to loop through = 100*100*100*100*100 
        /// = 10.000.000.000 times to enter this code
        /// And that is just for one currentSetLength of which we can have a lot.
        /// 
        /// Then comes all the permutations below. For 5 words that is 5! = 120 times we have to swap the words to test if the sentence match these 3 MD5.
        /// 
        /// So to limit work down here we start out with doing a check to see if all the words in the sentence is a subset of the anagram.
        /// </summary>
        /// <param name="currentSentence"></param>
        /// <param name="numberOfJackpots"></param>
        /// <param name="currentSetLength"></param>
        /// <param name="subsetCounter"></param>
        /// <param name="listOfWordPermutationsReplacementString"></param>
        private void PleaseEnterTheDeepHoleInsideLoop(int[] currentSentence, ref int numberOfJackpots, TCurrentSetOfXPos currentSetLength, 
            ref ulong subsetCounter, string[] listOfWordPermutationsReplacementString)
        {
            // ConsoleWriteLine(" Combinations: " + combinationCounter + ". Subsets: " + subsetCounter);
            // Use rows in the TableFilter2_WordMatrix to calculate number of each char in the sentence, so we cen check if the sentence is subset of the anagram
            var rows = new int[currentSentence.Length][];
            for (int i = 0; i < currentSentence.Length; i++)
            {
                rows[i] = _wordlistCtrl.TableFilter2_WordMatrix[currentSentence[i]];
            }
            var combinedWordToValidate = CombineRows(rows);
            var isSubset = _anagramCtrl.IsSubset(combinedWordToValidate);

            // Do MD5 check if the words combined is still a subset of anagram
            bool gotJackpot = false;
            if (isSubset)
            {
                subsetCounter++;
                gotJackpot = FetchWordsAndCheckMd5RemoveFoundHash(ref numberOfJackpots, currentSentence, listOfWordPermutationsReplacementString);
            }
        }
    }

    public class LoopSetsBase
    {
        protected Action<string> _consoleWriteLine;
        protected MD5 _md5HashComputer;
        protected Md5Helper _md5Hlpr;
        public Md5Helper Md5Checker { get { return _md5Hlpr; } }

        protected IAnagramContainer _anagramCtrl;
        protected IWordlistContainer _wordlistCtrl;

        public LoopSetsBase(Action<string> ConsoleWriteLine, MD5 Md5HashComputer,
            IAnagramContainer AnagramCtrl, IWordlistContainer WordlistCtrl)
        {
            _consoleWriteLine = ConsoleWriteLine;
            _md5HashComputer = Md5HashComputer;
            _anagramCtrl = AnagramCtrl;
            _wordlistCtrl = WordlistCtrl;
            _md5Hlpr = new Md5Helper(_md5HashComputer, _anagramCtrl.Md5Hashes);
        }

        /// <summary>
        /// Fetch real words from ListFilter1_WorddictHavingAllowedChars
        /// And check for MD5
        /// </summary>
        /// <param name="numberOfJackpots"></param>
        /// <param name="wordPointers"></param>
        /// <param name="listOfWordPermutationsReplacementString"></param>
        /// <returns></returns>
        protected bool FetchWordsAndCheckMd5RemoveFoundHash(ref int numberOfJackpots, int[] wordPointers, string[] listOfWordPermutationsReplacementString)
        {
            var words = new string[wordPointers.Length];
            for (int i = 0; i < wordPointers.Length; i++)
            {
                words[i] = _wordlistCtrl.ListFilter1_WorddictHavingAllowedChars.Keys.ElementAt(wordPointers[i]);
                //words[i] = _wordlistCtrl.ListFilter1_WordArrayHavingAllowedChars[wordPointers[i]];
            }
            bool gotJackpot = LoopPermutationsAndCheckMd5RemoveFoundHash(ref numberOfJackpots, words, listOfWordPermutationsReplacementString);
            return gotJackpot;
        }

        /// <summary>
        /// When this method is called we know that the characters in the sentence match (isSubset of) anagram
        /// In here we loop through the order of the words and check md5
        /// </summary>
        /// <param name="numberOfJackpots"></param>
        /// <param name="words"></param>
        /// <param name="listOfWordPermutationsReplacementString"></param>
        /// <returns></returns>
        public bool LoopPermutationsAndCheckMd5RemoveFoundHash(ref int numberOfJackpots, string[] words, string[] listOfWordPermutationsReplacementString)
        {
            bool gotJackpot = false;
            // did we get lucky? - loop permutations of the words in the sentence
            foreach (var permutationReplacementString in listOfWordPermutationsReplacementString)
            {
                if (!gotJackpot)
                {
                    var gotJackpotTest = checkMd5RemoveFoundHash(ref numberOfJackpots, string.Format(permutationReplacementString, words));
                    if (gotJackpotTest.HasValue)
                    {
                        gotJackpot = gotJackpotTest.Value;
                        if (gotJackpot)
                        {
                            // Break if sentence was found
                            break;
                        }
                    }
                    else
                    {
                        // Break if no more md5 hashes in list
                        break;
                    }
                }
                else
                {
                    // Break if sentence was found
                    break;
                }
            }
            return gotJackpot;
        }

        public bool? checkMd5RemoveFoundHash(ref int numberOfJackpots, string sentenceToCheck)
        {
            bool? gotJackpot = _md5Hlpr.VerifyMd5HashRemoveHashIfFound(sentenceToCheck);
            if (gotJackpot.HasValue && gotJackpot.Value)
            {
                numberOfJackpots++;
                _consoleWriteLine("  =============> JACKPOT number " + numberOfJackpots + " with '" + sentenceToCheck + "' <=============");
            }
            return gotJackpot;
        }

        /// <summary>
        /// Add number of each letter of three words, 
        /// so the sum can be compared with the sum in the anagram
        /// Rows contains no-of-chars in each word.
        /// CombineRows() sums up no-of-chars for all words in a sentence
        /// </summary>
        /// <param name="rows">No-of-chars in each word</param>
        /// <returns>Summed up no-of-chars for all words in a sentence</returns>
        protected int[] CombineRows(int[][] rows)
        {
            int[] combinedRow;
            var noOfRows = rows.Length;
            if (noOfRows > 0)
            {
                var lenOfaRow = rows[0].Length;
                // Assign space for result
                combinedRow = new int[lenOfaRow];

                // Word is stored from col3 onwards - loop it.
                // Col1 is number of chars
                for (int col = 1; col < lenOfaRow; col++)
                {
                    // Combine a column
                    for (int row = 0; row < noOfRows; row++)
                    {
                        combinedRow[col] += rows[row][col];
                    }
                }
            }
            else
            {
                throw new ApplicationException("CombineRows() needs two or more rows");
            }
            return combinedRow;
        }
    }
}

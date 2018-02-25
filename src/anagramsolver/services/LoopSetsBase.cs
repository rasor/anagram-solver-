using anagramsolver.containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace anagramsolver.services
{
    public class LoopSetsBase
    {
        protected Action<string> _consoleWriteLine;
        protected MD5 _md5HashComputer;
        protected Md5Helper _md5Hlpr;

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
        protected bool FetchWordsAndCheckMd5(ref int numberOfJackpots, int[] wordPointers, string[] listOfWordPermutationsReplacementString)
        {
            var words = new string[wordPointers.Length];
            for (int i = 0; i < wordPointers.Length; i++)
            {
                words[i] = _wordlistCtrl.ListFilter1_WorddictHavingAllowedChars.Keys.ElementAt(wordPointers[i]);
                //words[i] = _wordlistCtrl.ListFilter1_WordArrayHavingAllowedChars[wordPointers[i]];
            }
            bool gotJackpot = LoopPermutationsAndCheckMd5(ref numberOfJackpots, words, listOfWordPermutationsReplacementString);
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
        public bool LoopPermutationsAndCheckMd5(ref int numberOfJackpots, string[] words, string[] listOfWordPermutationsReplacementString)
        {
            bool gotJackpot = false;
            // did we get lucky? - loop permutations of the words in the sentence
            foreach (var permutationReplacementString in listOfWordPermutationsReplacementString)
            {
                if (!gotJackpot)
                {
                    var gotJackpotTest = checkMd5(ref numberOfJackpots, string.Format(permutationReplacementString, words));
                    if (gotJackpotTest.HasValue)
                    {
                        gotJackpot = gotJackpotTest.Value;
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

        public bool? checkMd5(ref int numberOfJackpots, string sentenceToCheck)
        {
            bool? gotJackpot = _md5Hlpr.VerifyMd5Hash(sentenceToCheck);
            if (gotJackpot.HasValue && gotJackpot.Value)
            {
                numberOfJackpots++;
                _consoleWriteLine(" JACKPOT number " + numberOfJackpots + " with '" + sentenceToCheck + "'");
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

using anagramsolver.containers;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace anagramsolver.helpers
{
    public abstract class LoopSetsBase
    {
        protected Action<string> _consoleWriteLine;
        protected MD5 _md5HashComputer;
        protected Md5Helper _md5Hlpr;

        protected AnagramContainer _anagramCtrl;
        protected WordlistContainer _wordlistCtrl;
        protected List<List<int>> _tableByWordLength;

        public LoopSetsBase(Action<string> ConsoleWriteLine, MD5 Md5HashComputer,
            AnagramContainer AnagramCtrl, WordlistContainer WordlistCtrl)
        {
            _consoleWriteLine = ConsoleWriteLine;
            _md5HashComputer = Md5HashComputer;
            _anagramCtrl = AnagramCtrl;
            _wordlistCtrl = WordlistCtrl;
            _tableByWordLength = _wordlistCtrl.TableByWordLength;
            _md5Hlpr = new Md5Helper(_md5HashComputer, _anagramCtrl.Md5Hashes);
        }

        protected bool checkMd5(ref int numberOfJackpots, string sentenceToCheck)
        {
            bool gotJackpot = _md5Hlpr.VerifyMd5Hash(sentenceToCheck);
            if (gotJackpot)
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

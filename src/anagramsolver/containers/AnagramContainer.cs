using anagramsolver.helpers;
using anagramsolver.models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace anagramsolver.containers
{
    /// <summary>
    /// Container for the input anagram and some calculated representations of it
    /// </summary>
    public class AnagramContainer : IAnagramContainer
    {
        // The 3 input hashes
        private string[] _md5Hashes;
        public string[] Md5Hashes { get { return _md5Hashes; } }

        // The input anagram
        private StringBox _anagram;
        public StringBox Anagram { get { return _anagram; } }

        // Letters that are not in the _anagram
        private StringBox _lettersNotInAnagram;
        public StringBox LettersNotInAnagram { get { return _lettersNotInAnagram; } }

        // The HeaderRow
        private int[] _anagramRow;
        public int[] AnagramRow { get { return _anagramRow; } }

        public AnagramContainer(IConfigurationRoot config)
            : this(
                  new StringBox(config["AppSettings:Anagram"]),
                  new string[] {config["AppSettings:Md5Hashes:0"], config["AppSettings:Md5Hashes:1"], config["AppSettings:Md5Hashes:2"]})
        {}
        public AnagramContainer(StringBox anagram, string[] md5Hashes)
        {
            _anagram = anagram;
            _md5Hashes = md5Hashes;
        }

        /// <summary>
        /// Find all letters that are not in the anagram
        /// </summary>
        /// <returns>Set of letters not in the anagram</returns>
        public StringBox CreateSetOfLettersNotInAnagram()
        {
            string allLettersA2Z = "abcdefghijklmnopqrstuvxyz";
            string someMoreLettersSeenInWordList = "éÅöü'";
            string allLettersExpectedInWordList = allLettersA2Z + someMoreLettersSeenInWordList;
            var lettersNotInAnagram1 = allLettersExpectedInWordList.Except(_anagram.DistinctDataWithoutSpace);
            string lettersNotInAnagram = String.Concat(lettersNotInAnagram1);
            _lettersNotInAnagram = new StringBox(lettersNotInAnagram);
            return _lettersNotInAnagram;
        }

        /// <summary>
        /// Create a row - like the rows in the TableFilter2_WordMatrix - but for the anagram.
        /// This row is our result / sum up / conclusion
        /// </summary>
        public void CreateHeaderRow()
        {
            // - col0 is index to ListFilter1 - NOT IN USE
            // - col1 is length of word - NOT IN USE
            // - col2 is a bit telling if the word is a subset of the anagram - NOT IN USE
            // - col3 to approx col15 is count of each letter in the word
            // Word in ListFilter1     TableFilter2
            //                         "ailnoprstuwy" <- The letters in anagram - sorted - this is header line
            // "poultryoutwitsants"     111121124211  <- The anagram

            int col0 = 0, col1 = 0, col2 = 0; // - NOT IN USE - reason to add these is just to keep same columns as in TableFilter2_WordMatrix

            var hlpr = new TableHelper();
            // The "table header" - ailnoprstuwy
            var tableHeaderOfAnagramSorted = this.Anagram.DistinctDataWithoutSpaceSorted;
            // The anagram without spaces - poultryoutwitsants
            var word = _anagram.RawDataWithoutSpace;
            _anagramRow = hlpr.FillRowFromWord(tableHeaderOfAnagramSorted, col0, col1, col2, word);
        }

        public bool IsSubset(int[] row)
        {
            bool isSubset = true;
            // Word is stored from col3 onwards - loop it
            for (int i = 3; i < row.Length - 1; i++)
            {
                // Too many letters in word - it is not a subset
                if (row[i] > _anagramRow[i])
                {
                    isSubset = false;
                }
            }

            return isSubset;
        }

    }
}

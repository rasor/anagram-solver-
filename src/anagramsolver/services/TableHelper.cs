using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace anagramsolver.services
{
    /// <summary>
    /// Various helper Methods
    /// </summary>
    public class TableHelper
    {
        /// <summary>
        /// Convert a word to an array (called row) telling how many of each letter is in the word
        /// </summary>
        /// <param name="tableHeaderOfAnagramSorted">Anagram Distinct and Sorted e.g. ailnoprstuwy</param>
        /// <param name="col0">Index to ListFilter1</param>
        /// <param name="col1">Length of word</param>
        /// <param name="col2">A bit telling if the word is a subset of the anagram</param>
        /// <param name="word">A word to encode e.g. poultryoutwitsants</param>
        /// <returns>An array including number of each letter in a word e.g. 000111121124211</returns>
        public int[] FillRowFromWord(IEnumerable<char> tableHeaderOfAnagramSorted, int col0, int col1, int col2, string word)
        {
            // Prefill letter counts with 0's
            var row = new int[] { col0, col1, col2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            // https://stackoverflow.com/questions/1403493/what-is-the-point-of-lookuptkey-telement
            var lookupByChar = word.ToLookup(c => c);
            // Loop letters in "table header" (ailnoprstuwy) and count number of letters of each in current row/word
            int j = 3; // Col3 - first col having count of numbers
            foreach (var letter in tableHeaderOfAnagramSorted)
            {
                row[j] = lookupByChar[letter].Count();
                j++;
            }
            return row;
        }

        /// <summary>
        /// Print content of a list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string ListToString(List<int> list)
        {
            string result = "[";
            list.ForEach(count => result += count + ",");
            result = result.Substring(0, result.Length - 1) + "]";
            return result;
        }

        internal int LastIndexHavingValueGreaterThan0(List<int> list)
        {
            int lastIndexHavingValueGreaterThan0 = -1;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] > 0)
                {
                    lastIndexHavingValueGreaterThan0 = i;
                    break;
                }
            }
            return lastIndexHavingValueGreaterThan0;
        }
    }
}

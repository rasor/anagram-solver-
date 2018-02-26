using System;
using System.Collections.Generic;
using System.Text;

namespace anagramsolver.models
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CurrentSetBase
    {
        protected Dictionary<short, int> _dictOfWordLengths = new Dictionary<short, int>();
        /// <summary>
        /// Lenght of words in set - Dictionary<WordNo, WordLength>
        /// Example - 4 words: keys: WordNo     [1, 2, 3, 4]
        ///                  values: WordLength [2, 4, 4, 8] - sum = 18 chars
        /// </summary>
        public Dictionary<short, int> DictOfWordLengths { get { return _dictOfWordLengths; } }

        /// <summary>
        /// Number of words by length in set - Dictionary<WordLength, NoOfWords>
        /// Example - 4 words: keys: Lengths         [1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,12,13,14,15,16,17] - max key = anagramlength - 1
        ///                  values: Number of words [0, 1, 0, 2, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0] - sum = 4 words
        /// </summary>
        //public Dictionary<int, short> DictCountOfWordLengths { get { return null; } }

        /// <summary>
        /// Number of words by length in set - Dictionary<WordLength, NoOfWords> - sorted
        /// Example - 4 words: keys: Lengths         [4, 8, 2] 
        ///                  values: Number of words [2, 1, 1] - sum = 4 words - sorted desc
        /// </summary>
        //public Dictionary<int, short> SortedDictCountOfWordLengths { get { return null; } }

        /// <summary>
        /// Iterator of sets - return false, when no more sets
        /// </summary>
        /// <returns></returns>
        public abstract bool SetNextSet();

        public override string ToString()
        {
            string result = "[" + String.Join(", ",_dictOfWordLengths.Values) + "]";
            return result;
        }
    }
}

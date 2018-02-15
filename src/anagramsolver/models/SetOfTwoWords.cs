using System;
using System.Collections.Generic;
using System.Text;

namespace anagramsolver.models
{
    /// <summary>
    /// Contains pointers to two words
    /// </summary>
    public struct SetOfTwoWords
    {
        public int Word1, Word2;

        public SetOfTwoWords(int w1, int w2)
        {
            Word1 = w1;
            Word2 = w2;
        }
    }
}

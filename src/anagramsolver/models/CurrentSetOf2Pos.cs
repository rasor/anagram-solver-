using System;
using System.Collections.Generic;
using System.Text;

namespace anagramsolver.models
{
    /// <summary>
    /// Contains length of two words, which combined is length of anagram
    /// </summary>
    public class CurrentSetOf2Pos
    {
        // Lenght of words in set
        protected int _word1Length, _word2Length;
        public int Word1Length { get { return _word1Length - 1; } } //subtract 1 to match index in table
        public int Word2Length { get { return _word2Length - 1; } } //subtract 1 to match index in table

        // Does any words in the set have same length?
        protected bool _anyOfSameLength;
        public bool AnyOfSameLength { get { return _anyOfSameLength; } }

        protected int _anagramLength;
        //protected int _hasUnEvenChars; //if even the then the middle words are both first and last word - so that row in the table needs special looping
        protected bool _isEven;
        protected int _lowestMiddleWordLetters;

        public CurrentSetOf2Pos() { }
        public CurrentSetOf2Pos(int AnagramLength)
        {
            _word1Length = 0; // initial value to indicate looping has nnot started yet
            _word2Length = AnagramLength -1;
            _anagramLength = AnagramLength;
        }

        public bool IsEvenAndIsMiddleLength() {
            bool result = (_isEven && (_word1Length == _lowestMiddleWordLetters));
            return result;
        }

        public virtual bool SetNextSet() {
            bool nextSetWasSet = true;
            // increment number
            _word1Length++;
            // sum of all wordlenghts reamins 18
            _word2Length = _anagramLength - _word1Length;

            // Stop if any left side numbers are larger than numbers to its rigth side
            if (_word1Length > _word2Length)
            {
                nextSetWasSet = false;
            }

            // Calculate AnyOfSameLength
            _anyOfSameLength = (_word1Length == _word2Length);

            return nextSetWasSet;
        }

        public override string ToString() {
            string result = "[" + _word1Length + ", " + _word2Length + "]";
            return result;
        }
    }
}

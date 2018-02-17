using System;
using System.Collections.Generic;
using System.Text;

namespace anagramsolver.models
{
    /// <summary>
    /// Contains length of two words, which combined is length of anagram
    /// </summary>
    public class CurrentSetOfTwoPos
    {
        protected int _word1Length, _word2Length;
        public int Word1Length { get { return _word1Length - 1; } } //subtract 1 to match index in table
        public int Word2Length { get { return _word2Length - 1; } } //subtract 1 to match index in table

        protected int _anagramLength;
        protected int _hasUnEvenChars; //if even the then the middle words are both first and last word - so that row in the table needs special looping
        protected bool _isEven;
        protected int _lowestMiddleWordLetters;

        public CurrentSetOfTwoPos(int AnagramLength)
        {
            _word1Length = 1;
            _word2Length = AnagramLength -1;
            _anagramLength = AnagramLength;
            _hasUnEvenChars = _anagramLength % 2;
            _isEven = (_hasUnEvenChars == 0);
            _lowestMiddleWordLetters = (_anagramLength - _hasUnEvenChars) / 2;
        }

        public bool IsEvenAndIsMiddleLength() {
            bool result = (_isEven && (_word1Length == _lowestMiddleWordLetters));
            return result;
        }

        public virtual bool SetNextSet() {
            bool nextSetWasSet = true;
            if (_word1Length == _lowestMiddleWordLetters)
            {
                nextSetWasSet = false;
            }
            // sum of both wordlenghts reamins 18
            _word1Length++;
            _word2Length--;
            return nextSetWasSet;
        }

        public override string ToString() {
            string result = "[" + _word1Length + ", " + _word2Length + "]";
            return result;
        }
    }
}

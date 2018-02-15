using System;
using System.Collections.Generic;
using System.Text;

namespace anagramsolver.containers
{
    /// <summary>
    /// Contains length of two words, which combined is length of anagram
    /// </summary>
    public class CurrentSetOfTwoPos
    {
        private int _word1Length, _word2Length;
        public int Word1Length { get { return _word1Length - 1; } } //subtract 1 to match index in table
        public int Word2Length { get { return _word2Length - 1; } } //subtract 1 to match index in table

        private int _anagramLength;
        private int _hasUnEvenChars; //if even the then the middle words are both first and last word - so that row in the table needs special looping
        private bool _isEven;
        private int _lowestMiddleWordLetters;

        public CurrentSetOfTwoPos(int AnagramLenght)
        {
            _word1Length = 1;
            _word2Length = AnagramLenght -1;
            _anagramLength = AnagramLenght;
            _hasUnEvenChars = _anagramLength % 2;
            _isEven = (_hasUnEvenChars == 0);
            _lowestMiddleWordLetters = (_anagramLength - _hasUnEvenChars) / 2;
        }

        public bool IsEvenAndIsMiddleLength() {
            bool result = (_isEven && (_word1Length == _lowestMiddleWordLetters));
            return result;
        }

        public bool SetNextSet() {
            bool nextSetWasSet = true;
            if (_word1Length == _lowestMiddleWordLetters)
            {
                nextSetWasSet = false;
            }
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

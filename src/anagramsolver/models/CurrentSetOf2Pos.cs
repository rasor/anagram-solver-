using System;
using System.Collections.Generic;
using System.Text;

namespace anagramsolver.models
{
    /// <summary>
    /// Contains length of 2 words, which combined is length of anagram
    /// </summary>
    public class CurrentSetOf2Pos: CurrentSetBase
    {
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
            _dictOfWordLengths[1] = 0; // initial value to indicate looping has nnot started yet
            _dictOfWordLengths[2] = AnagramLength -1;
            _anagramLength = AnagramLength;
        }

        public bool IsEvenAndIsMiddleLength() {
            bool result = (_isEven && (_dictOfWordLengths[1] == _lowestMiddleWordLetters));
            return result;
        }

        public override bool SetNextSet() {
            bool nextSetWasSet = true;
            // increment number
            _dictOfWordLengths[1]++;
            // sum of all wordlenghts reamins 18
            _dictOfWordLengths[2] = _anagramLength - _dictOfWordLengths[1];

            // Stop if any left side numbers are larger than numbers to its rigth side
            if (_dictOfWordLengths[1] > _dictOfWordLengths[2])
            {
                nextSetWasSet = false;
            }

            // Calculate AnyOfSameLength
            _anyOfSameLength = (_dictOfWordLengths[1] == _dictOfWordLengths[2]);

            return nextSetWasSet;
        }

        public override string ToString() {
            string result = "[" + _dictOfWordLengths[1] + ", " + _dictOfWordLengths[2] + "]";
            return result;
        }
    }
}

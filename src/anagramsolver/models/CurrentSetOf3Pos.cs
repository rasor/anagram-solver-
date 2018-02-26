using System;
using System.Collections.Generic;
using System.Text;

namespace anagramsolver.models
{
    /// <summary>
    /// Contains length of three words, which combined is length of anagram
    /// </summary>
    public class CurrentSetOf3Pos: CurrentSetOf2Pos
    {
        protected int _word3Length;
        public int Word3Length { get { return _word3Length - 1; } } //subtract 1 to match index in table

        public CurrentSetOf3Pos() {}
        public CurrentSetOf3Pos(int AnagramLength): 
            base(AnagramLength)
        {
            _word2Length = 0; // initial value to indicate looping has nnot started yet
            _word3Length = AnagramLength - 2;
        }

        public override bool SetNextSet()
        {
            bool nextSetWasSet = true;
            // If we haven't started looping then _word1Length == 0
            if (_word1Length == 0)
            {
                _word1Length = 1;
            }
            int diffWord3_2Len = _word3Length - _word2Length;
            // if last cannot be decremented more, then increment other numbers
            if (diffWord3_2Len == 0 || diffWord3_2Len == 1)
            {
                _word1Length++;
                _word2Length = _word1Length;
            }
            else
            {
                _word2Length++;
            }
            // sum of all wordlenghts reamins 18
            _word3Length = _anagramLength - (_word1Length + _word2Length);

            // Stop if any left side numbers are larger than numbers to its rigth side
            if (_word1Length > _word2Length || _word2Length > _word3Length)
            {
                nextSetWasSet = false;
            }

            // Calculate AnyOfSameLength
            _anyOfSameLength = (_word1Length == _word2Length || _word1Length == _word3Length || _word2Length == _word3Length);

            return nextSetWasSet;
        }

        public override string ToString()
        {
            string result = "[" + _word1Length + ", " + _word2Length + ", " + _word3Length + "]";
            return result;
        }
    }
}

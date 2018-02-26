using System;
using System.Collections.Generic;
using System.Text;

namespace anagramsolver.models
{
    /// <summary>
    /// Contains length of 5 words, which combined is length of anagram
    /// </summary>
    public class CurrentSetOf5Pos: CurrentSetOf4Pos
    {
        protected int _word5Length;
        public int Word5Length { get { return _word5Length - 1; } } //subtract 1 to match index in table

        public CurrentSetOf5Pos() { }
        public CurrentSetOf5Pos(int AnagramLength): 
            base(AnagramLength)
        {
            _word2Length = 1;
            _word3Length = 1;
            _word4Length = 0; // initial value to indicate looping has nnot started yet
            _word5Length = AnagramLength - 4;
        }

        public override bool SetNextSet()
        {
            bool nextSetWasSet = true;
            // If we haven't started looping then _word1Length == 0
            if (_word1Length == 0)
            {
                _word1Length = 1;
            }
            int diffWord5_4Len = _word5Length - _word4Length;
            int diffWord4_3Len = _word4Length - _word3Length;
            int diffWord3_2Len = _word3Length - _word2Length;
            // if last cannot be decremented more, then increment other numbers
            if ((diffWord5_4Len == 0 || diffWord5_4Len == 1) && 
                (diffWord4_3Len == 0 || diffWord4_3Len == 1) && 
                (diffWord3_2Len == 0 || diffWord3_2Len == 1))
            {
                _word1Length++;
                _word2Length = _word1Length;
                _word3Length = _word1Length;
                _word4Length = _word1Length;
            }
            else
            // if 2. last has reached max then inc 2. onwards
            if ((diffWord5_4Len == 0 || diffWord5_4Len == 1) &&
                (diffWord4_3Len == 0 || diffWord4_3Len == 1))
            {
                _word2Length++;
                _word3Length = _word2Length;
                _word4Length = _word2Length;
            }
            else
            // if 3. last has reached max then inc 3. onwards
            if (diffWord5_4Len == 0 || diffWord5_4Len == 1)
            {
                _word3Length++;
                _word4Length = _word3Length;
            }
            // inc last digit to increment
            else
            {
                _word4Length++;
            }
            // sum of all wordlenghts reamins 18
            _word5Length = _anagramLength - (_word1Length + _word2Length + _word3Length + _word4Length);

            // Stop if any left side numbers are larger than numbers to its rigth side
            if (_word1Length > _word2Length || _word2Length > _word3Length || _word3Length > _word4Length || _word4Length > _word5Length)
            {
                nextSetWasSet = false;
            }

            // Calculate AnyOfSameLength
            _anyOfSameLength = (_word1Length == _word2Length || _word1Length == _word3Length || _word2Length == _word3Length);
            _anyOfSameLength = _anyOfSameLength || (_word1Length == _word4Length || _word2Length == _word4Length || _word3Length == _word4Length);
            _anyOfSameLength = _anyOfSameLength || (_word1Length == _word5Length || _word2Length == _word5Length || _word3Length == _word5Length || _word4Length == _word5Length);

            return nextSetWasSet;
        }

        public override string ToString()
        {
            string result = "[" + _word1Length + ", " + _word2Length + ", " + _word3Length + ", " + _word4Length + ", " + _word5Length + "]";
            return result;
        }
    }
}

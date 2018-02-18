using System;
using System.Collections.Generic;
using System.Text;

namespace anagramsolver.models
{
    /// <summary>
    /// Contains length of three words, which combined is length of anagram
    /// </summary>
    public class CurrentSetOf4Pos: CurrentSetOfThreePos
    {
        protected int _word4Length;
        public int Word4Length { get { return _word4Length - 1; } } //subtract 1 to match index in table

        public CurrentSetOf4Pos(int AnagramLength): 
            base(AnagramLength)
        {
            _word2Length = 1;
            _word3Length = 1;
            _word4Length = AnagramLength - 3;
        }

        public override bool SetNextSet()
        {
            bool nextSetWasSet = true;
            int diffWord4_3Len = _word4Length - _word3Length;
            int diffWord3_2Len = _word3Length - _word2Length;
            // if last cannot be decremented more, then increment other numbers
            if ((diffWord4_3Len == 0 || diffWord4_3Len == 1) && (diffWord3_2Len == 0 || diffWord3_2Len == 1))
            {
                _word1Length++;
                _word2Length = _word1Length;
                _word3Length = _word1Length;
            }
            else
            if (diffWord4_3Len == 0 || diffWord4_3Len == 1)
            {
                _word2Length++;
                _word3Length = _word2Length;
            }
            else
            {
                _word3Length++;
            }
            // sum of all wordlenghts reamins 18
            _word4Length = _anagramLength - (_word1Length + _word2Length + _word3Length);

            // Stop if any left side numbers are larger than numbers to its rigth side
            if (_word1Length > _word2Length || _word2Length > _word3Length || _word3Length > _word4Length)
            {
                nextSetWasSet = false;
            }
            return nextSetWasSet;
        }

        public override string ToString()
        {
            string result = "[" + _word1Length + ", " + _word2Length + ", " + _word3Length + ", " + _word4Length + "]";
            return result;
        }
    }
}

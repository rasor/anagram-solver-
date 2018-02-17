using System;
using System.Collections.Generic;
using System.Text;

namespace anagramsolver.models
{
    /// <summary>
    /// Contains length of three words, which combined is length of anagram
    /// </summary>
    public class CurrentSetOfThreePos: CurrentSetOfTwoPos
    {
        protected int _word3Length;
        public int Word3Length { get { return _word3Length - 1; } } //subtract 1 to match index in table

        public CurrentSetOfThreePos(int AnagramLength): 
            base(AnagramLength)
        {
            _word2Length = 1;
            _word3Length = AnagramLength - 2;
        }

        public override bool SetNextSet()
        {
            bool nextSetWasSet = true;
            int diffWord3_2Len = _word3Length - _word2Length;
            int diffWord2_1Len = _word2Length - _word1Length;
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
            return nextSetWasSet;
        }

        public override string ToString()
        {
            string result = "[" + _word1Length + ", " + _word2Length + ", " + _word3Length + "]";
            return result;
        }
    }
}

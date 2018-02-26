using System;
using System.Collections.Generic;
using System.Text;

namespace anagramsolver.models
{
    /// <summary>
    /// Contains length of 4 words, which combined is length of anagram
    /// </summary>
    public class CurrentSetOf4Pos: CurrentSetOf3Pos
    {
        public CurrentSetOf4Pos() { }
        public CurrentSetOf4Pos(int AnagramLength): 
            base(AnagramLength)
        {
            _dictOfWordLengths[2] = 1;
            _dictOfWordLengths[3] = 0; // initial value to indicate looping has nnot started yet
            _dictOfWordLengths[4] = AnagramLength - 3;
        }

        public override bool SetNextSet()
        {
            bool nextSetWasSet = true;
            // If we haven't started looping then _word1Length == 0
            if (_dictOfWordLengths[1] == 0)
            {
                _dictOfWordLengths[1] = 1;
            }
            int diffWord4_3Len = _dictOfWordLengths[4] - _dictOfWordLengths[3];
            int diffWord3_2Len = _dictOfWordLengths[3] - _dictOfWordLengths[2];
            // if last cannot be decremented more, then increment other numbers
            if ((diffWord4_3Len == 0 || diffWord4_3Len == 1) && (diffWord3_2Len == 0 || diffWord3_2Len == 1))
            {
                _dictOfWordLengths[1]++;
                _dictOfWordLengths[2] = _dictOfWordLengths[1];
                _dictOfWordLengths[3] = _dictOfWordLengths[1];
            }
            else
            // if 2. last has reached max then inc 2. onwards
            if (diffWord4_3Len == 0 || diffWord4_3Len == 1)
            {
                _dictOfWordLengths[2]++;
                _dictOfWordLengths[3] = _dictOfWordLengths[2];
            }
            // inc last digit to increment
            else
            {
                _dictOfWordLengths[3]++;
            }
            // sum of all wordlenghts reamins 18
            _dictOfWordLengths[4] = _anagramLength - (_dictOfWordLengths[1] + _dictOfWordLengths[2] + _dictOfWordLengths[3]);

            // Stop if any left side numbers are larger than numbers to its rigth side
            if (_dictOfWordLengths[1] > _dictOfWordLengths[2] || _dictOfWordLengths[2] > _dictOfWordLengths[3] || _dictOfWordLengths[3] > _dictOfWordLengths[4])
            {
                nextSetWasSet = false;
            }

            // Calculate AnyOfSameLength
            _anyOfSameLength = (_dictOfWordLengths[1] == _dictOfWordLengths[2] || _dictOfWordLengths[1] == _dictOfWordLengths[3] || _dictOfWordLengths[2] == _dictOfWordLengths[3]);
            _anyOfSameLength = _anyOfSameLength || (_dictOfWordLengths[1] == _dictOfWordLengths[4] || _dictOfWordLengths[2] == _dictOfWordLengths[4] || _dictOfWordLengths[3] == _dictOfWordLengths[4]);

            return nextSetWasSet;
        }
    }
}

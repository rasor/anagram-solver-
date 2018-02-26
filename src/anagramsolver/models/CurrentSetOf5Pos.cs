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
        public CurrentSetOf5Pos() { }
        public CurrentSetOf5Pos(int AnagramLength): 
            base(AnagramLength)
        {
            _dictOfWordLengths[2] = 1;
            _dictOfWordLengths[3] = 1;
            _dictOfWordLengths[4] = 0; // initial value to indicate looping has nnot started yet
            _dictOfWordLengths[5] = AnagramLength - 4;
        }

        public override bool SetNextSet()
        {
            bool nextSetWasSet = true;
            // If we haven't started looping then _word1Length == 0
            if (_dictOfWordLengths[1] == 0)
            {
                _dictOfWordLengths[1] = 1;
            }
            int diffWord5_4Len = _dictOfWordLengths[5] - _dictOfWordLengths[4];
            int diffWord4_3Len = _dictOfWordLengths[4] - _dictOfWordLengths[3];
            int diffWord3_2Len = _dictOfWordLengths[3] - _dictOfWordLengths[2];
            // if last cannot be decremented more, then increment other numbers
            if ((diffWord5_4Len == 0 || diffWord5_4Len == 1) && 
                (diffWord4_3Len == 0 || diffWord4_3Len == 1) && 
                (diffWord3_2Len == 0 || diffWord3_2Len == 1))
            {
                _dictOfWordLengths[1]++;
                _dictOfWordLengths[2] = _dictOfWordLengths[1];
                _dictOfWordLengths[3] = _dictOfWordLengths[1];
                _dictOfWordLengths[4] = _dictOfWordLengths[1];
            }
            else
            // if 2. last has reached max then inc 2. onwards
            if ((diffWord5_4Len == 0 || diffWord5_4Len == 1) &&
                (diffWord4_3Len == 0 || diffWord4_3Len == 1))
            {
                _dictOfWordLengths[2]++;
                _dictOfWordLengths[3] = _dictOfWordLengths[2];
                _dictOfWordLengths[4] = _dictOfWordLengths[2];
            }
            else
            // if 3. last has reached max then inc 3. onwards
            if (diffWord5_4Len == 0 || diffWord5_4Len == 1)
            {
                _dictOfWordLengths[3]++;
                _dictOfWordLengths[4] = _dictOfWordLengths[3];
            }
            // inc last digit to increment
            else
            {
                _dictOfWordLengths[4]++;
            }
            // sum of all wordlenghts reamins 18
            _dictOfWordLengths[5] = _anagramLength - (_dictOfWordLengths[1] + _dictOfWordLengths[2] + _dictOfWordLengths[3] + _dictOfWordLengths[4]);

            // Stop if any left side numbers are larger than numbers to its rigth side
            if (_dictOfWordLengths[1] > _dictOfWordLengths[2] || _dictOfWordLengths[2] > _dictOfWordLengths[3] || _dictOfWordLengths[3] > _dictOfWordLengths[4] || _dictOfWordLengths[4] > _dictOfWordLengths[5])
            {
                nextSetWasSet = false;
            }

            // Calculate AnyOfSameLength
            _anyOfSameLength = (_dictOfWordLengths[1] == _dictOfWordLengths[2] || _dictOfWordLengths[1] == _dictOfWordLengths[3] || _dictOfWordLengths[2] == _dictOfWordLengths[3]);
            _anyOfSameLength = _anyOfSameLength || (_dictOfWordLengths[1] == _dictOfWordLengths[4] || _dictOfWordLengths[2] == _dictOfWordLengths[4] || _dictOfWordLengths[3] == _dictOfWordLengths[4]);
            _anyOfSameLength = _anyOfSameLength || (_dictOfWordLengths[1] == _dictOfWordLengths[5] || _dictOfWordLengths[2] == _dictOfWordLengths[5] || _dictOfWordLengths[3] == _dictOfWordLengths[5] || _dictOfWordLengths[4] == _dictOfWordLengths[5]);

            return nextSetWasSet;
        }

        public override string ToString()
        {
            string result = "[" + _dictOfWordLengths[1] + ", " + _dictOfWordLengths[2] + ", " + _dictOfWordLengths[3] + ", " + _dictOfWordLengths[4] + ", " + _dictOfWordLengths[5] + "]";
            return result;
        }
    }
}

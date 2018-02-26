using System;
using System.Collections.Generic;
using System.Text;

namespace anagramsolver.models
{
    /// <summary>
    /// Contains length of 3 words, which combined is length of anagram
    /// </summary>
    public class CurrentSetOf3Pos: CurrentSetOf2Pos
    {
        public CurrentSetOf3Pos() {}
        public CurrentSetOf3Pos(int AnagramLength): 
            base(AnagramLength)
        {
            _dictOfWordLengths[2] = 0; // initial value to indicate looping has nnot started yet
            _dictOfWordLengths[3] = AnagramLength - 2;
        }

        public override bool SetNextSet()
        {
            bool nextSetWasSet = true;
            // If we haven't started looping then _word1Length == 0
            if (_dictOfWordLengths[1] == 0)
            {
                _dictOfWordLengths[1] = 1;
            }
            int diffWord3_2Len = _dictOfWordLengths[3] - _dictOfWordLengths[2];
            // if last cannot be decremented more, then increment other numbers
            if (diffWord3_2Len == 0 || diffWord3_2Len == 1)
            {
                _dictOfWordLengths[1]++;
                _dictOfWordLengths[2] = _dictOfWordLengths[1];
            }
            else
            {
                _dictOfWordLengths[2]++;
            }
            // sum of all wordlenghts reamins 18
            _dictOfWordLengths[3] = _anagramLength - (_dictOfWordLengths[1] + _dictOfWordLengths[2]);

            // Stop if any left side numbers are larger than numbers to its rigth side
            if (_dictOfWordLengths[1] > _dictOfWordLengths[2] || _dictOfWordLengths[2] > _dictOfWordLengths[3])
            {
                nextSetWasSet = false;
            }

            // Calculate AnyOfSameLength
            _anyOfSameLength = (_dictOfWordLengths[1] == _dictOfWordLengths[2] || _dictOfWordLengths[1] == _dictOfWordLengths[3] || _dictOfWordLengths[2] == _dictOfWordLengths[3]);

            return nextSetWasSet;
        }

        public override string ToString()
        {
            string result = "[" + _dictOfWordLengths[1] + ", " + _dictOfWordLengths[2] + ", " + _dictOfWordLengths[3] + "]";
            return result;
        }
    }
}

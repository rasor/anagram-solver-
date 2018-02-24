using System.Collections.Generic;

namespace anagramsolver.containers
{
    public interface IWordlistContainer
    {
        Dictionary<string, int> ListFilter1_WorddictHavingAllowedChars { get; }
        List<string> ListUnfiltered0_Wordlist { get; }
        List<List<int>> TableByWordLength { get; }
        int[][] TableFilter2_WordMatrix { get; }
    }
}
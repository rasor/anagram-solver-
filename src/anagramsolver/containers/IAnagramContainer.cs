﻿using anagramsolver.models;

namespace anagramsolver.containers
{
    public interface IAnagramContainer
    {
        StringBox Anagram { get; }
        int[] AnagramRow { get; }
        StringBox LettersNotInAnagram { get; }
        string[] Md5Hashes { get; }

        bool IsSubset(int[] row);
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace anagramsolver.services
{
    public static class PermutationsCreator
    {
        /// <summary>
        /// https://www.mathsisfun.com/combinatorics/combinations-permutations.html
        /// https://stackoverflow.com/questions/1952153/what-is-the-best-way-to-find-all-combinations-of-items-in-an-array/10629938#10629938
        /// https://stackoverflow.com/questions/33312488/permutations-of-numbers-in-array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> GetPermutationsWithRept<T>(IEnumerable<T> list, int length)
        {
            if (length == 1)
            {
                return list.Select(t => new T[] { t });
            }
            return GetPermutationsWithRept(list, length - 1)
                .SelectMany(t => list,
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(o => !t.Contains(o)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public static IEnumerable<IEnumerable<T>> GetKCombsWithRept<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetKCombsWithRept(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) >= 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public static IEnumerable<IEnumerable<T>> GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetKCombs(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public static IEnumerable<string> ToReplacementString<T>(IEnumerable<IEnumerable<T>> listOfPermutations) where T : IComparable
        {
            var permutationsList = listOfPermutations.Select(wordPos => "{" + string.Join("} {", wordPos) + "}");
            return permutationsList;
        }

        /// <summary>
        /// Create list with permutations for string.Format: "{0} {1} {2}"
        /// </summary>
        /// <param name="numberOfWordsInSentence">Number of words that should be replaced in string.Format</param>
        /// <returns>A list with a line foreach permutation</returns>
        public static string[] CreateListOfWordPermutationsReplacementStrings(int numberOfWordsInSentence)
        {
            int[] permutationValues = new int[numberOfWordsInSentence];
            for (int i = 0; i < numberOfWordsInSentence; i++)
            {
                permutationValues[i] = i;
            }

            // List from [0,1,2] to [2,1,0] = 6 permutations - used for swapping order of words in sentence
            //int[] permutationValues = new int[] { 0, 1, 2 };
            var listOfWordPermutations = PermutationsCreator.GetPermutations(permutationValues, permutationValues.Length);
            // Convert to a list for string.Format: "{0} {1} {2}"
            var listOfWordPermutationsReplacementStrings = PermutationsCreator.ToReplacementString(listOfWordPermutations).ToArray(); ;
            return listOfWordPermutationsReplacementStrings;
        }
    }
}

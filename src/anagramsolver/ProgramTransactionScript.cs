using System;
using anagramsolver.containers;
using anagramsolver.models;
using System.Linq;
using System.Security.Cryptography;
using anagramsolver.services;
using Microsoft.Extensions.Configuration;

namespace anagramsolver
{
    public class ProgramTransactionScript
    {
        private static IAnagramContainer _injectedAnagramContainer;
        private static IWordlistContainer _injectedWordlistContainer;
        private static ConsoleLogger _injectedLogger;
        private static LoopSetsOf2WordsHelper _injectedSetsOf2WordsLooper;
        private static LoopSetsOf3WordsHelper _injectedSetsOf3WordsLooper;
        private static LoopSetsOf4WordsHelper _injectedSetsOf4WordsLooper;
        private static LoopSetsOf5WordsHelper _injectedSetsOf5WordsLooper;

        // Pseudo:
        // A. Load Data
        // B. Decrease the dataset
        // C. Find valid words in dataset

        public ProgramTransactionScript(IConfigurationRoot config, ConsoleLogger logger,
            IAnagramContainer anagramContainer, IWordlistContainer wordlistContainer,
            LoopSetsOf2WordsHelper setsOf2WordsLooper, LoopSetsOf3WordsHelper setsOf3WordsLooper,
            LoopSetsOf4WordsHelper setsOf4WordsLooper, LoopSetsOf5WordsHelper setsOf5WordsLooper)
        {
            _injectedLogger = logger;
            Console.WriteLine("Hello from AnagramSolver!");
            Console.WriteLine("");

            // A. Load Data

            // A1. Show loaded anagram data
            _injectedAnagramContainer = anagramContainer;
            _injectedLogger.ConsoleWriteLine("A1_LoadAnagram()");
            _injectedLogger.ConsoleWriteLine(" This is the input anagram: '" + _injectedAnagramContainer.Anagram.RawData + "'");
            _injectedLogger.ConsoleWriteLine(" These distinct letters does the anagram contain: '" + _injectedAnagramContainer.Anagram.DistinctDataWithoutSpaceAsString + "'");
            _injectedLogger.ConsoleWriteLine(" As above, but sorted: '" + _injectedAnagramContainer.Anagram.DistinctDataWithoutSpaceSortedAsString + "' - also called TableHeader");
            Console.WriteLine("");

            // A2. Show loaded wordlistdata
            _injectedWordlistContainer = wordlistContainer;
            _injectedLogger.ConsoleWriteLine("A2_LoadWordlist()");
            _injectedLogger.ConsoleWriteLine(" The unfiltered input wordlist contains " + _injectedWordlistContainer.ListUnfiltered0_Wordlist.Count + " lines");
            Console.WriteLine("");

            _injectedSetsOf2WordsLooper = setsOf2WordsLooper;
            _injectedSetsOf3WordsLooper = setsOf3WordsLooper;
            _injectedSetsOf4WordsLooper = setsOf4WordsLooper;
            _injectedSetsOf5WordsLooper = setsOf5WordsLooper;
        }

        public void Main(string[] args)
        {
            // B1. Decrease anagram the dataset
            _injectedLogger.ConsoleWriteLine("B1_ReduceTheAnagramDataset()");
            B1_ReduceTheAnagramDataset();
            Console.WriteLine("");

            // B2. Reduce wordlist the dataset
            _injectedLogger.ConsoleWriteLine("B2_ReduceTheWordlistDataset()");
            B2_ReduceTheWordlistDataset();
            Console.WriteLine("");

            // C1. Find valid words in dataset being subset of anagram
            _injectedLogger.ConsoleWriteLine("C1_FindValidWordsInDataset()");
            C1_FindValidWordsInDataset();
            Console.WriteLine("");

            // C2. Order Words In Dataset By Lenght
            _injectedLogger.ConsoleWriteLine("C2_OrderWordsInDatasetByLenght()");
            var longestWord = C2_OrderWordsInDatasetByLenght();
            Console.WriteLine("");

            // D. Find valid combinations with 2 words
            _injectedLogger.ConsoleWriteLine("D1_FindValidCombinations()");
            D1_FindValidCombinations();
            Console.WriteLine("");

            _injectedLogger.ConsoleWriteLine("Done AnagramSolver! - Press any key");
            Console.ReadKey();
        }

        static void B1_ReduceTheAnagramDataset()
        {
            // B1A Create a set of letters not in the anagram
            // - This will will make it possible to remove words from the list containing any of those letters
            _injectedAnagramContainer.CreateSetOfLettersNotInAnagram();
            _injectedLogger.ConsoleWriteLine(" These distinct letters does the anagram NOT contain: '" + _injectedAnagramContainer.LettersNotInAnagram.RawData + "'");
        }

        static void B2_ReduceTheWordlistDataset()
        {
            // B2A Create a list of words only containing letters from the anagram
            // - This will reduce the list to approx 2500 words - duration: approx 2 secs
            _injectedWordlistContainer.Filter1_CreateListOfWordsHavingLettersFromAnagram(_injectedAnagramContainer);
            _injectedLogger.ConsoleWriteLine(" List_Filter1 - List only having chars present in Anagram: The list contains " + _injectedWordlistContainer.ListFilter1_WorddictHavingAllowedChars.Count + " unique lines");
            // PrintListFilter1(WordlistCtrl);

            // B2B Create a table of words being subset of the the anagram
            // - This will enable fast sum up of letters i words chosen in an arbitrary combination
            _injectedWordlistContainer.Filter2_CreateTableOfWordsBeingSubsetOfAnagram(_injectedAnagramContainer);
            _injectedLogger.ConsoleWriteLine(" Table_Filter2 - created - with same number of rows as in List_Filter1");
        }

        static void C1_FindValidWordsInDataset()
        {
            // C1A As in the Matrix count letters in the anagram
            _injectedAnagramContainer.CreateHeaderRow();
            _injectedLogger.ConsoleWriteLine(" Anagram Distinct and Sorted - TableHeader    :     " + _injectedAnagramContainer.Anagram.DistinctDataWithoutSpaceSortedAsString);
            _injectedLogger.ConsoleWriteLine(" AnagramRow - number of each letter in anagram: {" + string.Concat(_injectedAnagramContainer.AnagramRow) + "}");

            // C1B Foreach row in table Calculate if word is subset of anagram.
            // Store the result (of word being a subset) in col2 in the Table_Filter2
            var noOfWordsBeingSubset = _injectedWordlistContainer.UpdateCol2InTableFilter2(_injectedAnagramContainer);
            _injectedLogger.ConsoleWriteLine(" Table_Filter2 - col2 updated with whether or not word is subset of anagram");
            _injectedLogger.ConsoleWriteLine(" Table_Filter2 - contains " + noOfWordsBeingSubset + " words being subsets of anagram");
        }

        static int C2_OrderWordsInDatasetByLenght()
        {
            // C2B Create a table of valid words having a List of words with same length as rows
            var listOfWordLenghts = _injectedWordlistContainer.CreateTableByWordLength(_injectedAnagramContainer);
            var tableHlpr = new TableHelper();
            _injectedLogger.ConsoleWriteLine(" Table_ByWordLength created.    1, 2,  3,  4,  5,  6,  7, 8, 9,10,11,12");
            _injectedLogger.ConsoleWriteLine(" Number of words in each row: " + tableHlpr.ListToString(listOfWordLenghts));
            var longestWord = tableHlpr.LastIndexHavingValueGreaterThan0(listOfWordLenghts) + 1;
            return longestWord;
        }

        /// <summary>
        /// Loop through combinations
        /// https://www.mathsisfun.com/combinatorics/combinations-permutations.html
        /// </summary>
        static void D1_FindValidCombinations()
        {
            // Create permutationsets-loop-algoritm.
            // In the loop do
            // - Foreach set (of two words)
            // -- If set 1000 has been reached print the set number and the set words
            // -- Loop permuatations (AB and BA, when words are only two)
            // --- Validate A+B against anagram
            // --- If valid then check "A B" md5 against all 3 md5 solutions
            // ---- If found then remove the md5 from the list, so there only will be two to check against
            // ----- and return the found sentense ("A B")

            int numberOfJackpots = 0;
            // D1A LoopSetsOf2Words
            numberOfJackpots = _injectedSetsOf2WordsLooper.LoopSetsOf2WordsDoValidateAndCheckMd5();
            string[] remainingHashes = _injectedSetsOf2WordsLooper.Md5Checker.Md5Hashes;
            Console.WriteLine("");

            // D1B LoopSetsOf3Words
            numberOfJackpots = _injectedSetsOf3WordsLooper.LoopSetsOf3WordsDoValidateAndCheckMd5(numberOfJackpots, remainingHashes);
            remainingHashes = _injectedSetsOf3WordsLooper.Md5Checker.Md5Hashes;
            Console.WriteLine("");

            // D1C LoopSetsOf4Words
            numberOfJackpots = _injectedSetsOf4WordsLooper.LoopSetsOf4WordsDoValidateAndCheckMd5(numberOfJackpots, remainingHashes);
            remainingHashes = _injectedSetsOf4WordsLooper.Md5Checker.Md5Hashes;
            Console.WriteLine("");

            // D1D LoopSetsOf5Words
            numberOfJackpots = _injectedSetsOf5WordsLooper.LoopSetsOf5WordsDoValidateAndCheckMd5(numberOfJackpots, remainingHashes);
            remainingHashes = _injectedSetsOf5WordsLooper.Md5Checker.Md5Hashes;
            Console.WriteLine("");
        }

        static void PrintListFilter1()
        {
            foreach (var kvp in _injectedWordlistContainer.ListFilter1_WorddictHavingAllowedChars)
            {
                Console.WriteLine("                   " + kvp.Key);
            }
        }
    }
}

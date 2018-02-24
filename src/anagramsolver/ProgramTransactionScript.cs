using System;
using anagramsolver.containers;
using anagramsolver.models;
using System.Linq;
using System.Security.Cryptography;
using anagramsolver.helpers;
using Microsoft.Extensions.Configuration;

namespace anagramsolver
{
    public class ProgramTransactionScript
    {
        private static IAnagramContainer _injectedAnagramContainer;
        private static IWordlistContainer _injectedWordlistContainer;

        // Pseudo:
        // A. Load Data
        // B. Decrease the dataset
        // C. Find valid words in dataset

        public ProgramTransactionScript(IConfigurationRoot config,
            IAnagramContainer anagramContainer, IWordlistContainer wordlistContainer)
        {
            Console.WriteLine("Hello from AnagramSolver!");
            Console.WriteLine("");

            // A. Load Data

            // A1. Show loaded anagram data
            _injectedAnagramContainer = anagramContainer;
            ConsoleWriteLine("A1_LoadAnagram()");
            ConsoleWriteLine(" This is the input anagram: '" + _injectedAnagramContainer.Anagram.RawData + "'");
            ConsoleWriteLine(" These distinct letters does the anagram contain: '" + _injectedAnagramContainer.Anagram.DistinctDataWithoutSpaceAsString + "'");
            ConsoleWriteLine(" As above, but sorted: '" + _injectedAnagramContainer.Anagram.DistinctDataWithoutSpaceSortedAsString + "' - also called TableHeader");
            Console.WriteLine("");

            // A2. Show loaded wordlistdata
            _injectedWordlistContainer = wordlistContainer;
            ConsoleWriteLine("A2_LoadWordlist()");
            ConsoleWriteLine(" The unfiltered input wordlist contains " + _injectedWordlistContainer.ListUnfiltered0_Wordlist.Count + " lines");
            Console.WriteLine("");
        }

        public void Main(string[] args)
        {
            // B1. Decrease anagram the dataset
            ConsoleWriteLine("B1_ReduceTheAnagramDataset()");
            B1_ReduceTheAnagramDataset(_injectedAnagramContainer);
            Console.WriteLine("");

            // B2. Reduce wordlist the dataset
            ConsoleWriteLine("B2_ReduceTheWordlistDataset()");
            B2_ReduceTheWordlistDataset(_injectedAnagramContainer, _injectedWordlistContainer);
            Console.WriteLine("");

            // C1. Find valid words in dataset being subset of anagram
            ConsoleWriteLine("C1_FindValidWordsInDataset()");
            C1_FindValidWordsInDataset(_injectedAnagramContainer, _injectedWordlistContainer);
            Console.WriteLine("");

            // C2. Order Words In Dataset By Lenght
            ConsoleWriteLine("C2_OrderWordsInDatasetByLenght()");
            var longestWord = C2_OrderWordsInDatasetByLenght(_injectedAnagramContainer, _injectedWordlistContainer);
            Console.WriteLine("");

            using (MD5 md5HashComputer = MD5.Create())
            {
                // D. Find valid combinations with 2 words
                ConsoleWriteLine("D1_FindValidCombinations()");
                D1_FindValidCombinations(md5HashComputer, longestWord, _injectedAnagramContainer, _injectedWordlistContainer);
                Console.WriteLine("");
            }

            ConsoleWriteLine("Done AnagramSolver! - Press any key");
            Console.ReadKey();
        }

        static void B1_ReduceTheAnagramDataset(IAnagramContainer AnagramCtrl)
        {
            // B1A Create a set of letters not in the anagram
            // - This will will make it possible to remove words from the list containing any of those letters
            AnagramCtrl.CreateSetOfLettersNotInAnagram();
            ConsoleWriteLine(" These distinct letters does the anagram NOT contain: '" + AnagramCtrl.LettersNotInAnagram.RawData + "'");
        }

        static void B2_ReduceTheWordlistDataset(IAnagramContainer AnagramCtrl, IWordlistContainer WordlistCtrl)
        {
            // B2A Create a list of words only containing letters from the anagram
            // - This will reduce the list to approx 2500 words - duration: approx 2 secs
            WordlistCtrl.Filter1_CreateListOfWordsHavingLettersFromAnagram(AnagramCtrl);
            ConsoleWriteLine(" List_Filter1 - List only having chars present in Anagram: The list contains " + WordlistCtrl.ListFilter1_WorddictHavingAllowedChars.Count + " unique lines");
            // PrintListFilter1(WordlistCtrl);

            // B2B Create a table of words being subset of the the anagram
            // - This will enable fast sum up of letters i words chosen in an arbitrary combination
            WordlistCtrl.Filter2_CreateTableOfWordsBeingSubsetOfAnagram(AnagramCtrl);
            ConsoleWriteLine(" Table_Filter2 - created - with same number of rows as in List_Filter1");
        }

        static void C1_FindValidWordsInDataset(IAnagramContainer AnagramCtrl, IWordlistContainer WordlistCtrl)
        {
            // C1A As in the Matrix count letters in the anagram
            AnagramCtrl.CreateHeaderRow();
            ConsoleWriteLine(" Anagram Distinct and Sorted - TableHeader    :     " + AnagramCtrl.Anagram.DistinctDataWithoutSpaceSortedAsString);
            ConsoleWriteLine(" AnagramRow - number of each letter in anagram: {" + string.Concat(AnagramCtrl.AnagramRow) + "}");

            // C1B Foreach row in table Calculate if word is subset of anagram.
            // Store the result (of word being a subset) in col2 in the Table_Filter2
            var noOfWordsBeingSubset = WordlistCtrl.UpdateCol2InTableFilter2(AnagramCtrl);
            ConsoleWriteLine(" Table_Filter2 - col2 updated with whether or not word is subset of anagram");
            ConsoleWriteLine(" Table_Filter2 - contains " + noOfWordsBeingSubset + " words being subsets of anagram");
        }

        static int C2_OrderWordsInDatasetByLenght(IAnagramContainer AnagramCtrl, IWordlistContainer WordlistCtrl)
        {
            // C2B Create a table of valid words having a List of words with same length as rows
            var listOfWordLenghts = WordlistCtrl.CreateTableByWordLength(AnagramCtrl);
            var tableHlpr = new TableHelper();
            ConsoleWriteLine(" Table_ByWordLength created.    1, 2,  3,  4,  5,  6,  7, 8, 9,10,11,12");
            ConsoleWriteLine(" Number of words in each row: " + tableHlpr.ListToString(listOfWordLenghts));
            var longestWord = tableHlpr.LastIndexHavingValueGreaterThan0(listOfWordLenghts) + 1;
            return longestWord;
        }

        /// <summary>
        /// Loop through combinations
        /// https://www.mathsisfun.com/combinatorics/combinations-permutations.html
        /// </summary>
        /// <param name="Md5HashComputer"></param>
        /// <param name="longestWord"></param>
        /// <param name="AnagramCtrl"></param>
        /// <param name="WordlistCtrl"></param>
        static void D1_FindValidCombinations(MD5 Md5HashComputer, int longestWord, IAnagramContainer AnagramCtrl, IWordlistContainer WordlistCtrl)
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
            var looper2 = new LoopSetsOf2WordsHelper(ConsoleWriteLine, Md5HashComputer, AnagramCtrl, WordlistCtrl);
            //numberOfJackpots = looper2.LoopSetsOf2WordsDoValidateAndCheckMd5();
            Console.WriteLine("");

            // D1B LoopSetsOf3Words
            var looper3 = new LoopSetsOf3WordsHelper(ConsoleWriteLine, Md5HashComputer, AnagramCtrl, WordlistCtrl);
            numberOfJackpots = looper3.LoopSetsOf3WordsDoValidateAndCheckMd5(numberOfJackpots);
            Console.WriteLine("");

            // D1C LoopSetsOf4Words
            var looper4 = new LoopSetsOf4WordsHelper(ConsoleWriteLine, Md5HashComputer, AnagramCtrl, WordlistCtrl);
            //numberOfJackpots = looper4.LoopSetsOf4WordsDoValidateAndCheckMd5(numberOfJackpots);
            Console.WriteLine("");

            // D1C LoopSetsOf5Words
            var looper5 = new LoopSetsOf5WordsHelper(ConsoleWriteLine, Md5HashComputer, AnagramCtrl, WordlistCtrl);
            //numberOfJackpots = looper5.LoopSetsOf5WordsDoValidateAndCheckMd5(numberOfJackpots);
            Console.WriteLine("");
        }

        static void ConsoleWriteLine(string stringToLog)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " " + stringToLog);
        }

        static void PrintListFilter1(IWordlistContainer WordlistCtrl)
        {
            foreach (var kvp in WordlistCtrl.ListFilter1_WorddictHavingAllowedChars)
            {
                Console.WriteLine("                   " + kvp.Key);
            }
        }
    }
}

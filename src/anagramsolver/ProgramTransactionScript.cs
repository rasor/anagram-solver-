﻿using System;
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
        static string _configAnagram;
        static string[] _configMd5Hashes;
        static string _configWordlistPath;

        public ProgramTransactionScript(IConfigurationRoot config)
        {
            // Input data
            _configAnagram = config["AppSettings:Anagram"];
            _configMd5Hashes = new string[] 
            { config["AppSettings:Md5Hashes:0"], config["AppSettings:Md5Hashes:1"], config["AppSettings:Md5Hashes:2"] };
            _configWordlistPath = config["AppSettings:WordlistPath"];
        }

        public void Main(string[] args)
        {
            Console.WriteLine("Hello from AnagramSolver!");
            Console.WriteLine("");

            // Pseudo:
            // A. Load Data
            // B. Decrease the dataset
            // C. Find valid words in dataset

            // A1. Load anagram data
            ConsoleWriteLine("A1_LoadAnagram()");
            var anagramCtrl = A1_LoadAnagram();
            Console.WriteLine("");

            // A2. Load wordlistdata
            ConsoleWriteLine("A2_LoadWordlist()");
            var wordlistCtrl = A2_LoadWordlist();
            Console.WriteLine("");

            // B1. Decrease anagram the dataset
            ConsoleWriteLine("B1_ReduceTheAnagramDataset()");
            B1_ReduceTheAnagramDataset(anagramCtrl);
            Console.WriteLine("");

            // B2. Reduce wordlist the dataset
            ConsoleWriteLine("B2_ReduceTheWordlistDataset()");
            B2_ReduceTheWordlistDataset(anagramCtrl, wordlistCtrl);
            Console.WriteLine("");

            // C1. Find valid words in dataset being subset of anagram
            ConsoleWriteLine("C1_FindValidWordsInDataset()");
            C1_FindValidWordsInDataset(anagramCtrl, wordlistCtrl);
            Console.WriteLine("");

            // C2. Order Words In Dataset By Lenght
            ConsoleWriteLine("C2_OrderWordsInDatasetByLenght()");
            var longestWord = C2_OrderWordsInDatasetByLenght(anagramCtrl, wordlistCtrl);
            Console.WriteLine("");

            using (MD5 md5HashComputer = MD5.Create())
            {

                // D. Find valid combinations with 2 words
                ConsoleWriteLine("D1_FindValidCombinations()");
                D1_FindValidCombinations(md5HashComputer, longestWord, anagramCtrl, wordlistCtrl);
                Console.WriteLine("");
            }

            ConsoleWriteLine("Done AnagramSolver! - Press any key");
            Console.ReadKey();
        }

        static AnagramContainer A1_LoadAnagram()
        {
            // Put data in a model, where it can be represented in various ways
            var anagram = new StringBox(_configAnagram);
            // Put data in a controller that can manage it
            var anagramCtrl = new AnagramContainer(anagram, _configMd5Hashes);
            ConsoleWriteLine(" This is the input anagram: '" + anagramCtrl.Anagram.RawData + "'");
            ConsoleWriteLine(" These distinct letters does the anagram contain: '" + anagramCtrl.Anagram.DistinctDataWithoutSpaceAsString + "'");
            ConsoleWriteLine(" As above, but sorted: '" + anagramCtrl.Anagram.DistinctDataWithoutSpaceSortedAsString + "' - also called TableHeader");
            return anagramCtrl;
        }

        static WordlistContainer A2_LoadWordlist()
        {
            var wordlistCtrl = new WordlistContainer(_configWordlistPath);
            ConsoleWriteLine(" The unfiltered input wordlist contains " + wordlistCtrl.ListUnfiltered0_Wordlist.Count + " lines");
            return wordlistCtrl;
        }

        static void B1_ReduceTheAnagramDataset(AnagramContainer AnagramCtrl)
        {
            // B1A Create a set of letters not in the anagram
            // - This will will make it possible to remove words from the list containing any of those letters
            AnagramCtrl.CreateSetOfLettersNotInAnagram();
            ConsoleWriteLine(" These distinct letters does the anagram NOT contain: '" + AnagramCtrl.LettersNotInAnagram.RawData + "'");
        }

        static void B2_ReduceTheWordlistDataset(AnagramContainer AnagramCtrl, WordlistContainer WordlistCtrl)
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

        static void C1_FindValidWordsInDataset(AnagramContainer AnagramCtrl, WordlistContainer WordlistCtrl)
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

        static int C2_OrderWordsInDatasetByLenght(AnagramContainer AnagramCtrl, WordlistContainer WordlistCtrl)
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
        static void D1_FindValidCombinations(MD5 Md5HashComputer, int longestWord, AnagramContainer AnagramCtrl, WordlistContainer WordlistCtrl)
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

        static void PrintListFilter1(WordlistContainer WordlistCtrl)
        {
            foreach (var kvp in WordlistCtrl.ListFilter1_WorddictHavingAllowedChars)
            {
                Console.WriteLine("                   " + kvp.Key);
            }
        }
    }
}

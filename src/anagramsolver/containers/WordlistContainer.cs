using anagramsolver.helpers;
using anagramsolver.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace anagramsolver.containers
{
    /// <summary>
    /// Container for the input wordlist and some list and tables created from it
    /// </summary>
    public class WordlistContainer
    {
        // Orginal list
        private List<string> _listUnfiltered0_Wordlist;
        public List<string> ListUnfiltered0_Wordlist { get { return _listUnfiltered0_Wordlist; } }

        // Filtered Lists
        private Dictionary<string, int> _listFilter1_WorddictHavingAllowedChars;
        public Dictionary<string, int> ListFilter1_WorddictHavingAllowedChars { get { return _listFilter1_WorddictHavingAllowedChars; } }
        private int[][] _tableFilter2_WordMatrix;
        public int[][] TableFilter2_WordMatrix { get { return _tableFilter2_WordMatrix; } }
        // Ordered Tables
        private List<List<int>> _tableByWordLength;
        public List<List<int>> TableByWordLength { get { return _tableByWordLength; } }

        public WordlistContainer(string WordlistPath)
        {
            // Orginal list
            var wordArray = File.ReadAllLines(WordlistPath);
            //_wordOfCharList = wordArray;
            _listUnfiltered0_Wordlist = wordArray.ToList();
        }

        internal void Filter1_CreateListOfWordsHavingLettersFromAnagram(AnagramContainer anagramCtrl)
        {
            // preapare lists
            _listFilter1_WorddictHavingAllowedChars = new Dictionary<string, int>();

            // Create new list having letters that are contained anagram - the words might still have too many of the allowed chars
            var lettersNotInAnagram = anagramCtrl.LettersNotInAnagram.DistinctDataWithoutSpace;
            _listUnfiltered0_Wordlist.ForEach(word => {
                // If a word don't have any of the letters not in the anagram, then it is a possible match
                var inCommonCharsCount = lettersNotInAnagram.Intersect(word).Count();
                if (inCommonCharsCount == 0)
                {
                    try
                    {
                        // A dict that uses word as key - it will throw if duplicates - remove duplicates in the same operation
                        _listFilter1_WorddictHavingAllowedChars.Add(word, word.Length);
                    }
                    catch (Exception)
                    {
                    }
                }
            });
        }

        internal void Filter2_CreateTableOfWordsBeingSubsetOfAnagram(AnagramContainer anagramCtrl)
        {
            // What's the plan? ------------------------------------------------------
            // Create a table (of integers) having words as rows, where 
            // - col0 is index to ListFilter1
            // - col1 is length of word
            // - col2 is a bit telling if the word is a subset of the anagram (0 = Invalid, 1 = IsSubset)
            // - col3 to approx col15 is count of each letter in the word
            // Each row has same line number as in Filter1 to be able to easy lookup the word. The line number is also stored in col0.
            // Only letters from the anagram is stored, since other letters were removed before ListFilter1
            // Sample of table:
            // Word in ListFilter1     TableFilter2
            //                         "ailnoprstuwy" <- The letters in anagram - sorted - this is header line (not in table)
            // "poultryoutwitsants"     111121124211  <- The anagram (not in table)
            // -------------------------------------- (below this line is in table)
            // "airstrip"            080120001211000  <- Line0, Len: 8, Invalid, no of each char
            // "tyranosaurus"        1c0200110221201  <- Line1, Len: c, Invalid, no of each char

            // After creating this table we 
            // - calculate which rows are valid
            // - perhaps remove invalid rows
            // - calculate some combinations from the length of the words
            // Foreach combination sum the cols in the table - as soon as we reach a col with number differs from anagram then check failed and we goto next combination

            // If all match then it is time for md5 checksum match - if fail, then try out each different order of words

            // Back to work ------------------------------------------------------

            var hlpr = new TableHelper();
            // The "table header" - ailnoprstuwy
            var tableHeaderOfAnagramSorted = anagramCtrl.Anagram.DistinctDataWithoutSpaceSorted;
            // How many different letters in anagram? - 12
            var numberOfLettersInDistinctAnagram = tableHeaderOfAnagramSorted.Count();

            // we know size of table, so created it - we create it as array of arrays - this enables us to delete rows from the table easily
            _tableFilter2_WordMatrix = new int[_listFilter1_WorddictHavingAllowedChars.Count][]; 

            // Fill table - one row foreach word in ListFilter1
            int col0, col1, col2;
            int i = 0; // counters for loops
            foreach (var kvp in _listFilter1_WorddictHavingAllowedChars)
            {
                // Fill one row
                col0 = i; // Pointer to ListFilter1, where word is stored
                col1 = kvp.Value; // Lenght of word
                col2 = 0; // Invalid = Not a subset - initial value - will be calculated later
                var word = kvp.Key;
                int[] row = hlpr.FillRowFromWord(tableHeaderOfAnagramSorted, col0, col1, col2, word);

                // Add row to table
                _tableFilter2_WordMatrix[i] = row;
                i++;
            }
        }

        internal int UpdateCol2InTableFilter2(AnagramContainer anagramCtrl)
        {
            // Pseudo:
            // Loop through rows
            //   Loop through number of letters in each row
            //     If there are more letters than in anagram, then bail out
            //   If not bailed out, then word is subset, so update col2

            // No number of letters must be larger than in the anagram row
            var anagramRow = anagramCtrl.AnagramRow;
            // Count number of words being subset
            var noOfWordsBeingSubset = 0;

            var isSubset = true;
            foreach (var row in _tableFilter2_WordMatrix)
            {
                isSubset = true;
                // Word is stored from col3 onwards - loop it
                for (int i = 3; i < row.Length-1; i++)
                {
                    // Too many letters in word - it is not a subset
                    if (row[i] > anagramRow[i])
                    {
                        isSubset = false;
                    }
                }
                // Update col2
                if (isSubset)
                {
                    row[2] = 1;
                    noOfWordsBeingSubset++;
                }
            }
            return noOfWordsBeingSubset;
        }

        internal List<int> CreateTableByWordLength(AnagramContainer AnagramCtrl)
        {
            var anagramLenghtWithoutSpaces = AnagramCtrl.Anagram.RawDataWithoutSpace.Length;
            // Assuming the anagram is made up of minimum 2 words 
            // - and when shortest word is length = 1, then longest will be
            var longestWordWhen2WordsAreUsed = anagramLenghtWithoutSpaces - 1;
            // Now create a table having a row for each length from 1 to longestWordWhen2WordsAreUsed

            // We also want a list telling how many words there are of each length
            var listOfWordLenghts = new List<int>();

            bool isValid, lengthMatch;
            int pointerToWord, wordLength;
            List<int> row;
            var sourceTable = _tableFilter2_WordMatrix;
            _tableByWordLength = new List<List<int>>();
            for (int i = 0; i < longestWordWhen2WordsAreUsed; i++)
            {
                row = new List<int>();
                wordLength = i + 1;
                // Loop matrix and add pointer (col0) to valid (col2) words being wordLength (col1)
                foreach (var sourceRow in sourceTable)
                {
                    isValid = (sourceRow[2] > 0);
                    lengthMatch = (sourceRow[1] == wordLength);
                    if (isValid && lengthMatch)
                    {
                        pointerToWord = sourceRow[0];
                        row.Add(pointerToWord);
                    }
                }
                listOfWordLenghts.Add(row.Count);
                _tableByWordLength.Add(row);
            }
            return listOfWordLenghts;
        }

        internal void LoopSetsOf2WordsDoValidateAndCheckMd5(Action<string> ConsoleWriteLine, MD5 Md5HashComputer, AnagramContainer AnagramCtrl)
        {
            // Pseudo:
            // Create permutationsets-loop-algoritm.
            // In the loop do
            // - Foreach set (of two words)
            // -- If set 1000 has been reached print the set number and the set words
            // -- Loop permuatations (AB and BA, when words are only two)
            // --- Validate A+B against anagram
            // --- If valid then check "A B" md5 against all 3 md5 solutions
            // ---- If found then remove the md5 from the list, so there only will be two to check against
            // ----- and return the found sentense ("A B")

            var hlpr = new TableHelper();
            UInt64 combinationCounter = 0; // max 18.446.744.073.709.551.615 .... yarn
            var tableToLoopThrough = _tableByWordLength;
            var totalLetters = AnagramCtrl.Anagram.RawDataWithoutSpace.Length; //18
            var hasUnEvenChars = totalLetters % 2; //if even the then the middle words are both first and last word - so that row in the table needs special looping
            var middleWordLetters = (totalLetters + hasUnEvenChars) / 2;

            CurrentSetOfTwoPos currentSet =  new CurrentSetOfTwoPos(totalLetters);

            ConsoleWriteLine("CombinationCounter: " + combinationCounter + ". CurrentSet: " + currentSet.ToString());

            while (currentSet.SetNextSet())
            {
                ConsoleWriteLine("CombinationCounter: " + combinationCounter + ". CurrentSet: " + currentSet.ToString());
                //
            }
            ConsoleWriteLine("CombinationCounter: " + combinationCounter + ". No more sets");
        }
    }
}

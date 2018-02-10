using System;

namespace anagramsolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello from AnagramSolver!");

            // Pseudo:
            // A. Decrease the dataset
            A1_DecreaseTheDataset();

            // B. Find valid words in dataset
            B1_FindValidWordsInDataset();

            // C. Find valid combinations with 2 words
            C1_FindValidCombinationsWith2Words();

            // D. Test combinations towards md5
            D1_Test2WordCombinationsTowardsMd5();

            Console.WriteLine("Done AnagramSolver!");
        }

        static void A1_DecreaseTheDataset(){
            // A1 Create a set of letters not in the anagram
        }
        static void B1_FindValidWordsInDataset(){}
        static void C1_FindValidCombinationsWith2Words(){}
        static void D1_Test2WordCombinationsTowardsMd5(){}
    }
}

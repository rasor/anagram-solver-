using Xunit;
using anagramsolver.helpers;
using System.Security.Cryptography;
using anagramsolver.containers;
using anagramsolver.models;
using anagramsolver.helpers;
using System.Linq;

namespace anagramsolverTests
{
    public class LoopSetsBaseTests
    {
        // Input data
        const string ANAGRAM = "poultry outwits ants";
        const string WORDLISTPATH = @".\resources\smallwordlist.txt";
        static readonly string[] md5Hashes =
            { "e4820b45d2277f3844eac66c903e84be", "23170acc097c24edb98fc5488ab033fe", "665e5bcb0c20062fe8abaaf4628bb154" };

        private readonly LoopSetsBase _LoopSetsBase;

        public LoopSetsBaseTests()
        {
            // Put data in a model, where it can be represented in various ways
            var anagram = new StringBox(ANAGRAM);
            // Put data in a controller that can manage it
            var anagramCtrl = new AnagramContainer(anagram, md5Hashes);
            var wordlistCtrl = new WordlistContainer(WORDLISTPATH);

            _LoopSetsBase = new LoopSetsBase(Dummy, MD5.Create(), anagramCtrl, wordlistCtrl);
        }

        private void Dummy(string input) { }

        private static string[] CreateListOfWordPermutationsReplacementStrings()
        {
            // List from [0,1,2] to [2,1,0] = 6 permutations - used for swapping order of words in sentence
            int[] permutationValues = new int[] { 0, 1, 2 };
            var listOfWordPermutations = PermutationsCreator.GetPermutations(permutationValues, permutationValues.Length);
            // Convert to a list for string.Format: "{0} {1} {2}"
            var listOfWordPermutationsReplacementString = PermutationsCreator.ToReplacementString(listOfWordPermutations).ToArray(); ;
            return listOfWordPermutationsReplacementString;
        }

        [Fact]
        public void GivenWords_WhenWordsCorrect_ShouldReturnTrue()
        {
            int noOfJackpots = 0;
            var words = new string[] { "yawls", "stout", "printout" };
            string[] listOfWordPermutationsReplacementStrings = CreateListOfWordPermutationsReplacementStrings();

            bool expected = true;
            bool actual = _LoopSetsBase.LoopPermutationsAndCheckMd5(ref noOfJackpots, words, listOfWordPermutationsReplacementStrings);
            Assert.Equal(expected, actual);
        }
    }
}

using Xunit;
using anagramsolver.helpers;
using System.Security.Cryptography;
using anagramsolver.containers;
using anagramsolver.models;
using System.Diagnostics;

namespace anagramsolverTests
{
    public class LoopSetsBaseTests
    {
        // Input data
        const string ANAGRAM = "poultry outwits ants";
        const string WORDLISTPATH = @".\resources\smallwordlist.txt";
        static readonly string[] md5Hashes =
            { "e4820b45d2277f3844eac66c903e84be", "23170acc097c24edb98fc5488ab033fe", "665e5bcb0c20062fe8abaaf4628bb154" };

        /// <summary>
        /// System under test
        /// </summary>
        private readonly LoopSetsBase _Sut;

        public LoopSetsBaseTests()
        {
            // Put data in a model, where it can be represented in various ways
            var anagram = new StringBox(ANAGRAM);
            // Put data in a controller that can manage it
            var anagramCtrl = new AnagramContainer(anagram, md5Hashes);
            var wordlistCtrl = new WordlistContainer(WORDLISTPATH);

            _Sut = new LoopSetsBase(Dummy, MD5.Create(), anagramCtrl, wordlistCtrl);
        }

        private void Dummy(string input) {
            Debug.WriteLine(input);
        }

        [Fact]
        public void GivenWords_WhenWordsCorrect_ShouldReturnTrue()
        {
            int noOfJackpots = 0;
            var words = new string[] { "yawls", "stout", "printout" };
            string[] listOfWordPermutationsReplacementStrings = new string[] { "{2} {0} {1}", "{2} {1} {0}" };

            // Expect words to match one of the md5Hashes
            bool expected = true;
            bool actual = _Sut.LoopPermutationsAndCheckMd5(ref noOfJackpots, words, listOfWordPermutationsReplacementStrings);
            Assert.Equal(expected, actual);

            // Other order should also succeed
            words = new string[] { "stout", "yawls", "printout" };
            actual = _Sut.LoopPermutationsAndCheckMd5(ref noOfJackpots, words, listOfWordPermutationsReplacementStrings);
            Assert.Equal(expected, actual);

            // It should also work with auto-created permutations
            listOfWordPermutationsReplacementStrings = PermutationsCreator.CreateListOfWordPermutationsReplacementStrings(3);
            actual = _Sut.LoopPermutationsAndCheckMd5(ref noOfJackpots, words, listOfWordPermutationsReplacementStrings);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GivenWords_WhenWordsCorrect_ButPermutationListIsWrong_ShouldReturnFalse()
        {
            int noOfJackpots = 0;
            var words = new string[] { "yawls", "stout", "printout" };
            // Though words are correct then the selected permutations won't put sentence in right order
            string[] listOfWordPermutationsReplacementStrings = new string[] { "{0} {1} {2}", "{0} {2} {1}" };

            // Expect fail, when words are not put in right order
            bool expected = false;
            bool actual = _Sut.LoopPermutationsAndCheckMd5(ref noOfJackpots, words, listOfWordPermutationsReplacementStrings);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GivenWords_WhenWordsInCorrect_ShouldReturnFalse()
        {
            int noOfJackpots = 0;
            var words = new string[] { "pawls", "stout", "printout" };
            // Create list with permutations for string.Format: "{0} {1} {2}" from [0,1,2] to [2,1,0] = 6 permutations
            string[] listOfWordPermutationsReplacementStrings = PermutationsCreator.CreateListOfWordPermutationsReplacementStrings(3);

            // Expect words not to match one of the md5Hashes
            bool expected = false;
            bool actual = _Sut.LoopPermutationsAndCheckMd5(ref noOfJackpots, words, listOfWordPermutationsReplacementStrings);
            Assert.Equal(expected, actual);
        }

    }
}

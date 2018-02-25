using Xunit;
using Moq;
using anagramsolver.services;
using System.Security.Cryptography;
using anagramsolver.containers;
using anagramsolver.models;
using System.Diagnostics;

namespace anagramsolverTests
{
    public class LoopSetsBaseTests
    {
        /// <summary>
        /// System under test
        /// </summary>
        //private readonly LoopSetsBase _Sut0;

        // Mocking WordlistContainer with a total empty behavior - we don't need it
        private Mock<IAnagramContainer> _anagramCtrlMocker3 = new Mock<IAnagramContainer>();
        private Mock<IAnagramContainer> _anagramCtrlMocker0 = new Mock<IAnagramContainer>();
        private Mock<IWordlistContainer> _wordlistCtrlMocker = new Mock<IWordlistContainer>();

        public LoopSetsBaseTests()
        {
            // Create mocks and SUT
            // https://github.com/Moq/moq4/wiki/Quickstart

            // Mocking AnagramContainer with md5Hashes
            string[] _md5Hashes3 =
                { "e4820b45d2277f3844eac66c903e84be", "23170acc097c24edb98fc5488ab033fe", "665e5bcb0c20062fe8abaaf4628bb154" };
            _anagramCtrlMocker3.Setup(foo => foo.Md5Hashes).Returns(() => _md5Hashes3);

            // Mocking AnagramContainer with no md5Hashes left
            string[] md5Hashes0 = { };
            _anagramCtrlMocker0.Setup(foo => foo.Md5Hashes).Returns(() => md5Hashes0);
        }

        private void DummyPrinter(string input) {
            Debug.WriteLine(input);
        }

        [Fact]
        public void Given3Words_WhenWordsCorrect_ShouldReturnTrue()
        {
            int noOfJackpots = 0;
            var words = new string[] { "yawls", "stout", "printout" };
            string[] listOfWordPermutationsReplacementStrings = new string[] { "{2} {0} {1}", "{2} {1} {0}" };

            var sut3 = new LoopSetsBase(DummyPrinter, MD5.Create(), _anagramCtrlMocker3.Object, _wordlistCtrlMocker.Object);
            // Expect words to match one of the md5Hashes
            bool expected = true;
            bool actual = sut3.LoopPermutationsAndCheckMd5RemoveFoundHash(ref noOfJackpots, words, listOfWordPermutationsReplacementStrings);
            Assert.Equal(expected, actual);

            // Other order should also succeed, but since the found hash was removed, then it can't find it twice
            words = new string[] { "stout", "yawls", "printout" };
            expected = false;
            actual = sut3.LoopPermutationsAndCheckMd5RemoveFoundHash(ref noOfJackpots, words, listOfWordPermutationsReplacementStrings);
            Assert.Equal(expected, actual);

            // Same as above, but this time we refresh the hashlist - and then the sentence should be found
            sut3 = new LoopSetsBase(DummyPrinter, MD5.Create(), _anagramCtrlMocker3.Object, _wordlistCtrlMocker.Object);
            expected = true;
            actual = sut3.LoopPermutationsAndCheckMd5RemoveFoundHash(ref noOfJackpots, words, listOfWordPermutationsReplacementStrings);
            Assert.Equal(expected, actual);

            // It should also work with auto-created permutations - we remember to refresh the hashlist by renewing sut3
            listOfWordPermutationsReplacementStrings = PermutationsCreator.CreateListOfWordPermutationsReplacementStrings(3);
            sut3 = new LoopSetsBase(DummyPrinter, MD5.Create(), _anagramCtrlMocker3.Object, _wordlistCtrlMocker.Object);
            actual = sut3.LoopPermutationsAndCheckMd5RemoveFoundHash(ref noOfJackpots, words, listOfWordPermutationsReplacementStrings);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given3Words_WhenWordsCorrect_ButPermutationListIsWrong_ShouldReturnFalse()
        {
            int noOfJackpots = 0;
            var words = new string[] { "yawls", "stout", "printout" };
            // Though words are correct then the selected permutations won't put sentence in right order
            string[] listOfWordPermutationsReplacementStrings = new string[] { "{0} {1} {2}", "{0} {2} {1}" };

            // Expect fail, when words are not put in right order
            bool expected = false;
            var sut3 = new LoopSetsBase(DummyPrinter, MD5.Create(), _anagramCtrlMocker3.Object, _wordlistCtrlMocker.Object);
            bool actual = sut3.LoopPermutationsAndCheckMd5RemoveFoundHash(ref noOfJackpots, words, listOfWordPermutationsReplacementStrings);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given3Words_WhenWordsInCorrect_ShouldReturnFalse()
        {
            int noOfJackpots = 0;
            var words = new string[] { "pawls", "stout", "printout" };
            // Create list with permutations for string.Format: "{0} {1} {2}" from [0,1,2] to [2,1,0] = 6 permutations
            string[] listOfWordPermutationsReplacementStrings = PermutationsCreator.CreateListOfWordPermutationsReplacementStrings(3);

            // Expect words not to match one of the md5Hashes
            bool expected = false;
            var sut3 = new LoopSetsBase(DummyPrinter, MD5.Create(), _anagramCtrlMocker3.Object, _wordlistCtrlMocker.Object);
            bool actual = sut3.LoopPermutationsAndCheckMd5RemoveFoundHash(ref noOfJackpots, words, listOfWordPermutationsReplacementStrings);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Given3Words_WhenNoMd5Hashes_ShouldReturnNoValue()
        {
            int noOfJackpots = 0;

            // Expect neither true nor false, when no md5Hashes in list (in _Sut0)
            bool expected = false;
            var sut0 = new LoopSetsBase(DummyPrinter, MD5.Create(), _anagramCtrlMocker0.Object, _wordlistCtrlMocker.Object);
            bool? actualQ = sut0.checkMd5RemoveFoundHash(ref noOfJackpots, "a b c");
            bool actual = actualQ.HasValue;
            Assert.Equal(expected, actual);
        }
    }
}

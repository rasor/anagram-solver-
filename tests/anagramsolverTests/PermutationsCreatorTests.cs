using Xunit;
using anagramsolver.helpers;

namespace anagramsolverTests
{
    public class PermutationsCreatorTests
    {
        /// <summary>
        /// System under test
        /// </summary>
        //static PermutationsCreator;

        [Fact]
        public void GivenANumberOfWords_ShouldCreateAListOfPermutationsWithAnExpectedNumber()
        {
            var actual = PermutationsCreator.CreateListOfWordPermutationsReplacementStrings(2).Length;
            //Expect 2 permutations of 2 words
            var expected = 2;
            Assert.Equal(expected, actual);

            actual = PermutationsCreator.CreateListOfWordPermutationsReplacementStrings(3).Length;
            expected = expected * 3;
            Assert.Equal(expected, actual);

            actual = PermutationsCreator.CreateListOfWordPermutationsReplacementStrings(4).Length;
            expected = expected * 4;
            Assert.Equal(expected, actual);

            actual = PermutationsCreator.CreateListOfWordPermutationsReplacementStrings(5).Length;
            expected = expected * 5;
            Assert.Equal(expected, actual);
        }
    }
}

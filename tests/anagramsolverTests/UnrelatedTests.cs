using Xunit;
using System.Collections.Generic;

namespace anagramsolverTests
{
    public class UnrelatedTests
    {
        [Fact]
        public void TestingHashSet()
        {
            HashSet<int> intHashSetOne = new HashSet<int>()
            {
                1,2,6,5,7,5
            };
            HashSet<int> intHashSetThree = new HashSet<int>()
            {
                6,7,5,2,1
            };
            bool actual = intHashSetThree.SetEquals(intHashSetOne);
            bool expected = true;
            Assert.Equal(expected, actual);
        }
    }
}

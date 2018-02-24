using Xunit;
using anagramsolver.models;
using System.Collections.Generic;
using System.Linq;

namespace anagramsolverTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            string expected = "poultry";
            string actual = new StringBox("poultry abc").RawData;
            Assert.Contains(expected, actual);
        }

        [Fact]
        public void Test2()
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

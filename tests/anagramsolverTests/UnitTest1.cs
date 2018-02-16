using Xunit;
using anagramsolver.models;
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
    }
}

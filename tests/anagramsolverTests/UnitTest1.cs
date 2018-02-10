using System;
using Xunit;

namespace anagramsolverTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            string expected = "Woof!";
            string actual = "Wroof!"; //new Dog().TalkToOwner();
            Assert.NotEqual(expected, actual);
        }
    }
}

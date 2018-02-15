using System;
using System.Collections.Generic;
using System.Linq;

namespace anagramsolver.models
{
    /// <summary>
    /// Kind of Extension methods for a string, but not static
    /// Perhaps I'll change it tobe Extension methods
    /// </summary>
    public class StringBox
    {
        // Input data
        private string _rawData;
        private string _rawDataWithoutSpace;
        public StringBox(string data)
        {
            _rawData = data;
            _rawDataWithoutSpace = _rawData.Replace(" ", "");
        }

        // Expose data
        public string RawData => _rawData;
        public string RawDataWithoutSpace => _rawDataWithoutSpace;

        public IEnumerable<char> DistinctDataWithoutSpace => _rawDataWithoutSpace.Distinct();
        public string DistinctDataWithoutSpaceAsString => String.Concat(DistinctDataWithoutSpace);

        public IEnumerable<char> DistinctDataWithoutSpaceSorted => DistinctDataWithoutSpace.OrderBy(x => x);
        public string DistinctDataWithoutSpaceSortedAsString => String.Concat(DistinctDataWithoutSpaceSorted);
    }
}

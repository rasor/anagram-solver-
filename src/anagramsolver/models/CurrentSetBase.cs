using System;
using System.Collections.Generic;
using System.Text;

namespace anagramsolver.models
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CurrentSetBase
    {
        // Dictionary<WordNo, WordLength>
        protected Dictionary<short, int> _dictOfWordLengths = new Dictionary<short, int>();
        public Dictionary<short, int> DictOfWordLengths { get { return _dictOfWordLengths; } }

        public abstract bool SetNextSet();
    }
}

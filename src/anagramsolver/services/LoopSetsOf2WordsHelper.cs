using anagramsolver.containers;
using anagramsolver.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace anagramsolver.services
{
    public class LoopSetsOf2WordsHelper<TCurrentSetOfXPos> : LoopSetsBase<TCurrentSetOfXPos> where TCurrentSetOfXPos : CurrentSetOf2Pos, new()
    {
        public LoopSetsOf2WordsHelper(ConsoleLogger logger, MD5 Md5HashComputer,
            IAnagramContainer AnagramCtrl, IWordlistContainer WordlistCtrl) : 
            base(logger.ConsoleWriteLine, Md5HashComputer, AnagramCtrl, WordlistCtrl)
        { }

    }
}

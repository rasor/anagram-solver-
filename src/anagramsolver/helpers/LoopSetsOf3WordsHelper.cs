using anagramsolver.containers;
using anagramsolver.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace anagramsolver.helpers
{
    public class LoopSetsOf3WordsHelper: LoopSetsBase
    {
        public LoopSetsOf3WordsHelper(Action<string> ConsoleWriteLine, MD5 Md5HashComputer,
            AnagramContainer AnagramCtrl, WordlistContainer WordlistCtrl) :
            base(ConsoleWriteLine, Md5HashComputer, AnagramCtrl, WordlistCtrl)
        { }

        public int LoopSetsOf3WordsDoValidateAndCheckMd5(int numberOfJackpots)
        {
            return numberOfJackpots;
        }

    }
}

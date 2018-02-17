using anagramsolver.containers;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace anagramsolver.helpers
{
    public abstract class LoopSetsBase
    {
        protected Action<string> _consoleWriteLine;
        protected MD5 _md5HashComputer;
        protected Md5Helper _md5Hlpr;

        protected AnagramContainer _anagramCtrl;
        protected WordlistContainer _wordlistCtrl;
        protected List<List<int>> _tableByWordLength;

        public LoopSetsBase(Action<string> ConsoleWriteLine, MD5 Md5HashComputer,
            AnagramContainer AnagramCtrl, WordlistContainer WordlistCtrl)
        {
            _consoleWriteLine = ConsoleWriteLine;
            _md5HashComputer = Md5HashComputer;
            _anagramCtrl = AnagramCtrl;
            _wordlistCtrl = WordlistCtrl;
            _tableByWordLength = _wordlistCtrl.TableByWordLength;
            _md5Hlpr = new Md5Helper(_md5HashComputer, _anagramCtrl.Md5Hashes);
        }

        protected bool checkMd5(ref int numberOfJackpots, string sentenceToCheck)
        {
            bool gotJackpot = _md5Hlpr.VerifyMd5Hash(sentenceToCheck);
            if (gotJackpot)
            {
                numberOfJackpots++;
                _consoleWriteLine(" JACKPOT number " + numberOfJackpots + " with '" + sentenceToCheck + "'");
            }
            return gotJackpot;
        }


    }
}

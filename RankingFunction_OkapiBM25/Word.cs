using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RankingFunction_OkapiBM25
{
    public class Word
    {
        public string Value { get; private set; }

        public int SentenceCount { get; private set; }

        public Word(string value, int count)
        {
            Value = value;
            SentenceCount = count;
        }
    }
}

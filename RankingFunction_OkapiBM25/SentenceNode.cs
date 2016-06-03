using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingFunction_OkapiBM25
{
    public class SentenceNode
    {
        public string Value { get; private set; }
        public double Score { get; private set; }

        public SentenceNode(string value, double score)
        {
            Value = value;
            Score = score;
        }
    }
}

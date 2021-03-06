﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingFunction_OkapiBM25
{
    public class SentenceNode
    {
        public int Pos {get;set;}
        public string Value { get; private set; }
        public double Score { get; set; }
        public List<string> Words { get; set; }


        public SentenceNode(string value)
        {
            Value = value;
        }
    }
}

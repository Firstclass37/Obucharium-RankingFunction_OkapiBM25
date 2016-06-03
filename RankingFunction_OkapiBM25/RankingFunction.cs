using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace RankingFunction_OkapiBM25
{
    public class RankingFunction
    {
        private List<string> words;
        private List<string> sentences;

        private double K = 2.0;
        private double B = 0.75;
        private double avgdl;
        

        public IEnumerable<SentenceNode> Rank(string inputText)
        {
            SetStartState(inputText);
            var result = SortResult(CalcScore());
            return result;
        }

        private List<SentenceNode> CalcScore()
        {        
            List<SentenceNode> result = new List<SentenceNode>();   
            foreach (var sentence in sentences)
            {
                double score = 0;

                foreach (var word in words)
                {
                    score += CaclIF(word, sentence)*CalcIDF(word);
                }
                result.Add(new SentenceNode(sentence,score));

            }
            return result;
        }

        private double CaclIF(string word, string sentence)
        {
            double wordFrequency = GetTermFrequency(word,sentence);
            int sentenceCount = GetWords(sentence).Count;

            double result = (wordFrequency * (K + 1) ) / (wordFrequency + K*(1 - B + B*sentenceCount/avgdl ) );
            return result;
        }

        private double CalcIDF(string word)
        {
            int sentenceCount = sentences.Count;
            var wordMatchCount = sentences
                .Where(s => s.ToLower().Contains(word))
                .Count();

            double result = Math.Log10( (sentenceCount*1.0 - wordMatchCount + 0.5) / (wordMatchCount+0.5) );
            return result > 0 ? result : 0;
        }

        private double GetTermFrequency(string word, string text)
        {
            var currentWords = GetWords(text);
            if (currentWords.Contains(word))
            {
                var count = currentWords
                    .Select(w=>w.Equals(word))
                    .Count();
                return count*1.0/currentWords.Count;
            }
            else
            {
                return 0;
            }
        }

        private List<SentenceNode> SortResult(List<SentenceNode> result )
        {
            return result
                .OrderByDescending(r => r.Value)
                .ToList();
        }



        private void SetStartState(string inputText)
        {
            words = GetWords(inputText);
            sentences = GetSentences(inputText);
            avgdl = inputText.Length*1.0/sentences.Count;

        }

        private List<string> GetWords(string text)
        {
            List<string> result = new List<string>();
            var regex= new Regex("\\w+?\\b");
            result = regex.Matches(text)
                .OfType<Match>()
                .Select(m => m.Value)
                .Select(m => m.Trim())
                .Select(m => m.ToLower())
                .GroupBy(m=>m)
                .Select(m=>m.Key)
                .ToList();
            return result;
        }

        private List<string> GetSentences(string text)
        {
            List<string> result = new List<string>();
            //"(\\A|\\.|\\s*)(.*)(\\.)"
            var regex = new Regex("[A-Z А-Я].*?(?=[.!?])");
            result = regex.Matches(text)
                .OfType<Match>()
                .Select(m => m.Value)
                .ToList();
            return result;
        }

       
    }
}

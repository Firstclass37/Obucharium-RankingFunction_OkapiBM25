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

        /// <summary>
        /// free parameters
        /// </summary>
        private double K = 2.0;
        /// <summary>
        /// free parameters
        /// </summary>
        private double B = 0.75;
        /// <summary>
        /// the average document length
        /// </summary>
        private double avgdl;
        
        /// <summary>
        /// The method set start parameters ,calculate rank for every sentence and return sorted by rank result as collection
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        public IEnumerable<SentenceNode> Rank(string inputText)
        {
            SetStartState(inputText);
            var result = SortResult(CalcScore());
            return result;
        }
        /// <summary>
        /// The method calculate rank for every sentence and return result as collection
        /// </summary>
        private List<SentenceNode> CalcScore()
        {        
            List<SentenceNode> result = new List<SentenceNode>();   
            foreach (var sentence in sentences)
            {
                double score = 0;

                foreach (var word in words)
                {
                    score += CaclLeftPart(word, sentence)*CalcIDF(word);
                }
                result.Add(new SentenceNode(sentence,score));

            }
            return result;
        }
        /// <summary>
        /// /// The method calculate left part of function for current word and current sentence
        /// </summary>
        /// <param name="word"></param>
        /// <param name="sentence"></param>
        /// <returns></returns>
        private double CaclLeftPart(string word, string sentence)
        {
            double wordFrequency = GetTermFrequency(word,sentence);
            int sentenceCount = GetWords(sentence).Count;

            double result = (wordFrequency * (K + 1) ) / (wordFrequency + K*(1 - B + B*sentenceCount/avgdl ) );
            return result;
        }
        /// <summary>
        /// /// The method calculate inverse document frequency for current word
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private double CalcIDF(string word)
        {
            int sentenceCount = sentences.Count;
            var wordMatchCount = sentences
                .Where(s => s.ToLower().Contains(word))
                .Count();

            double result = Math.Log10( (sentenceCount*1.0 - wordMatchCount + 0.5) / (wordMatchCount+0.5) );
            return result > 0 ? result : 0;
        }
        /// <summary>
        /// The method calculate word frequency in the current sentence
        /// </summary>
        /// <param name="word"></param>
        /// <param name="text"></param>
        /// <returns></returns>
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
        /// <summary>
        /// The method sort result list by score value
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private List<SentenceNode> SortResult(List<SentenceNode> result )
        {
            return result
                .OrderByDescending(r => r.Score)
                .ToList();
        }


        /// <summary>
        /// The method set start paratemers
        /// </summary>
        /// <param name="inputText"></param>
        private void SetStartState(string inputText)
        {
            words = GetWords(inputText);
            sentences = GetSentences(inputText);
            avgdl = inputText.Length*1.0/sentences.Count;

        }
        /// <summary>
        /// The method return list of words in input text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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
        /// <summary>
        /// The method return list of sentences in target text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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

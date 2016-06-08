using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace RankingFunction_OkapiBM25
{
    public class RankingFunction
    {
        private List<Word> targetWords = new List<Word>();
        private List<SentenceNode> targetSentences;

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
        
        public IEnumerable<SentenceNode> Rank(string inputText)
        {
            SetStartState(inputText);
            CalcScore();
            var result = SortResult(targetSentences);
            return result;
        }

        private void CalcScore()
        {
            foreach (var sentence in targetSentences)
            {
                double score = 0;

                foreach (var word in targetWords)
                {
                    double idf = CalcIDF(word);
                    if (idf > 0)
                    {
                        score += CaclRightPart(word, sentence)* idf;
                    }
                }
                sentence.Score = score;

            }
        }

        private double CaclRightPart(Word word, SentenceNode sentence)
        {
            double wordFrequency = GetTermFrequency(word,sentence);
            int sentenceCount = sentence.Words.Count;
            double result = (wordFrequency * (K + 1) ) / (wordFrequency + K*(1 - B + B*sentenceCount/avgdl ) );
            return result;
        }

        private double CalcIDF(Word word)
        {
            int sentenceCount = targetSentences.Count;
            double result = Math.Log10( (sentenceCount*1.0 - word.SentenceCount + 0.5) / (word.SentenceCount+0.5) );
            return result > 0 ? result : 0;
        }

        private double GetTermFrequency(Word word, SentenceNode text)
        {
            if (text.Words.Contains(word.Value))
            {
                var count = text.Words
                    .Count(w => w.Equals(word.Value));
                return count*1.0/ text.Words.Count;
            }
            else
            {
                return 0;
            }
        }

        private void SetStartState(string inputText)
        {
            targetSentences = AnalizeText(inputText);
            targetWords = GetTargettWords();
            var allWordsCount = targetSentences
                .Select(s => s.Words.Count)
                .Sum();
            avgdl = allWordsCount*1.0/targetSentences.Count;
        }

        private List<Word> GetTargettWords()
        {
            List<string> tempWords = new List<string>();
            foreach (var item in targetSentences)
            {
                var temp = item.Words
                    .GroupBy(w => w)
                    .Select(w => w.Key)
                    .ToList();
                foreach (var word in temp)
                {
                    tempWords.Add(word);
                }
            }


            targetWords = tempWords.GroupBy(w => w)
                .Where(w => w.Key.Length > 3)
                .Select(w => new Word(w.Key, w.Count()))
                .ToList();


            return targetWords;
        }

        private List<string> GetSentenceWords(string text)
        {
            var regex= new Regex("\\w+?\\b");
            var result = regex.Matches(text)
                .OfType<Match>()
                .Select(m => m.Value)
                .Select(m => m.Trim())
                .Select(m => m.ToLower())
                .ToList();
            return result;
        }

        private List<SentenceNode> AnalizeText(string text)
        {
            int index = 0;
            var regex = new Regex("[A-Z А-Я](.|\n)*?(?=[.])");
            var result = regex.Matches(text)
                .OfType<Match>()
                .Select(m => new SentenceNode(m.Value) {Words = GetSentenceWords(m.Value), Pos = ++index})
                .Where(s=>s.Words.Count>5)
                .ToList();
            return result;
        }

        private List<SentenceNode> SortResult(List<SentenceNode> result)
        {
            return result
                .OrderByDescending(r => r.Score)
                .ToList();
        }
    }
}

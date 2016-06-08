using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RankingFunction_OkapiBM25
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Okapi BM25";
            RankingFunction func = new RankingFunction();

            do
            {
                Console.Clear();

                try
                {
                    string resultText = string.Empty;
                    string path = Dialog.GetStringAnswer("Path");
                    string text = GetTextFromPath(path);

                    int count = Dialog.GetValueAnswer("Input count of sentence that you need");
                    if (count < 2) count = 2;
                    Dialog.ShowMessage("In process..",ConsoleColor.Yellow);  
                    
                    DateTime start = DateTime.Now;
                    var resultList = func.Rank(text).Take(count).ToList().OrderBy(w => w.Pos);
                    foreach (var item in resultList)
                    {
                        resultText += String.Format("{0} - {1:f2}", item.Value, item.Score) + Environment.NewLine;
                    }

                    Dialog.ShowMessage(string.Format("Compete for {0}",(DateTime.Now - start).TotalSeconds),ConsoleColor.Green);

                    SaveText(CreateNewPath(path),resultText);
                    Dialog.ShowMessage("Complete!!!",ConsoleColor.Green);

                }
                catch (InvalidInputException e)
                {
                    Dialog.ShowMessage(String.Format("{0} : {1}", e.Message, e.ErrorString), ConsoleColor.DarkRed);
                    continue;
                }
                catch (Exception e)
                {
                    Dialog.ShowMessage(String.Format("{0}", e.Message), ConsoleColor.DarkRed);
                }
                

            } while (Dialog.YNAnswer("Continue?"));
        }

        private static string GetTextFromPath(string path)
        {

            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else
            {
                throw new FileNotFoundException("???");
            }
        }

        private static string CreateNewPath(string oldPath)
        {
            string path = Path.GetDirectoryName(oldPath);
            string name = "(Result)" + Path.GetFileName(oldPath);
            return Path.Combine(path, name);
        }

        private static void SaveText(string path,string text)
        {
            File.WriteAllText(path,text);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingFunction_OkapiBM25
{
    public static class Dialog
    {

        public static int GetValueAnswer(string message)
        {
            int result;
            Console.Write("{0}: ",message);
            var res = Console.ReadLine();
            Console.WriteLine();
            if (int.TryParse(res, out result))
            {
                return result;
            }
            else
            {
                throw new InvalidInputException(res);
            }
        }

        public static string GetStringAnswer(string message)
        {
            Console.Write("{0}: ", message);
            var result = Console.ReadLine();
            Console.WriteLine();
            if (result != string.Empty)
            {
                return result;
            }
            else
            {
                throw new InvalidInputException("Empty input!!!");
            }
        }

        public static bool YNAnswer(string message)
        {
            Console.Write("{0} (y/n): ", message);
            var res = Console.ReadKey().KeyChar;
            if (res == 'y')
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static void ShowMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}

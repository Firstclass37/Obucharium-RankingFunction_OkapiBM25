using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingFunction_OkapiBM25
{
    public class InvalidInputException:Exception
    {
        public string ErrorString { get; set; }

        public InvalidInputException(string error):base("Invalid Input!")
        {
            ErrorString = error;
        }
    }
}

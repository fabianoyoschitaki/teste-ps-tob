using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortoSeguroBOT.Helpers
{
    public class Formatters
    {
        public static string Capitalize(string str)
        { 
            IList<string> Words = new List<string>();
            str = str.Trim();
            Words = str.Split( );
            string FinalSentence ="";
            foreach(string Word in Words)
            {
                FinalSentence += Word.Substring(0,1).ToUpper();
                FinalSentence += Word.Substring(1, Word.Length-1).ToLower();
                FinalSentence += " ";
            }
            FinalSentence = FinalSentence.Trim();
            return FinalSentence;
        }
    }
}
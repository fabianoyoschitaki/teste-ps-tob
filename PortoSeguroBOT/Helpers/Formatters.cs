using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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

        /// <summary>
        /// Método que remove acentução. "hello" == RemoveAcentos("hélló")
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveAcentos(string text)
        {
            string formD = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char ch in formD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }        
    }
}
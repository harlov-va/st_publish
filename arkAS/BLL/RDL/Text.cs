using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;


namespace RDL
{
    public abstract class Text
    {
        public static string SetDefaultValueForString(string input, string def)
        {
            if (String.IsNullOrEmpty(input))
            {
                input = def;
            }
            return input;
        }
        public static string SetDivider(string input, string divider)
        {
            //все разделители слов замеяем на нужный символ
            input = Regex.Replace(input, "\\W", divider);
            return input;
        }

        public static string DeleteHtmlTags(string input)
        {
            string ret = Regex.Replace(input, "<.+?>", "");
            return ret;
        }

        public static string ConvertToHtml(string content)
        {
            content = HttpUtility.HtmlEncode(content);
            content = content.Replace("  ", "&nbsp;&nbsp;").Replace(
               "\t", "&nbsp;&nbsp;&nbsp;").Replace("\n", "<br>");
            return content;
        }


        private static List<Char> cyr = new List<Char>(new Char[] {'а','б','в',
            'г','д','е','ё','ж','з','и','й','к','л','м','н','о','п','р','с',
            'т','у','ф','х','ц','ч','ш','щ','ъ','ы','ь','э','ю','я' });
        private static List<String> lat = new List<string>(new string[] {"a","b","v",
            "g","d","e","jo","zh","z","i","j","k","l","m","n","o","p","r","s",
            "t","u","f","h","c","ch","sh","w","#","y","","je","ju","ja" });

        public static string TransliterateString(string input)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in input)
            {
                int index = cyr.IndexOf(char.ToLower(c));
                switch (char.GetUnicodeCategory(c))
                {
                    case UnicodeCategory.UppercaseLetter:
                        if (index > -1)
                        {
                            try
                            {
                                string translitChars = lat[index];
                                if (translitChars != "")
                                {
                                    builder.Append(char.ToUpper(translitChars[0]));
                                    if (translitChars.Length > 1)
                                        builder.Append(translitChars.Substring(1));
                                }
                            }
                            catch (Exception ex) { }
                        }
                        else
                            builder.Append(char.ToUpper(c));
                        break;
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.ModifierLetter:
                    case UnicodeCategory.OtherLetter:
                        if (index > -1)
                            builder.Append(lat[index]);
                        else
                            builder.Append(c);
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }
            return builder.ToString();
        }

      
        public static bool UseTransliteInQueryString = true;
        public static string PrepareQueryStringParameter(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            if (Text.UseTransliteInQueryString)
            {
                input = Text.TransliterateString(input);
            }
            input = input.Replace("%", "-").
                  Replace("&amp;", "and").
                        Replace("+", "-").
                        Replace(":", "-").
                        Replace("?", "").
                        Replace("#", "-").
                        Replace("*", "-").
                        Replace("_", "-").
                        Replace("&", "and").
                        Replace("!", "").
                        Replace("@", "-").
                        Replace("$", "-").
                        Replace("^", "-").
                        Replace("(", "-").
                        Replace(")", "-").
                        Replace(@"""", "").Replace(@"'", "").
                        Replace(" ", "-");
            return input;
        }
        public static string GetValidCrop(string input, int maxCount, string threeDots)
        {
            input = DeleteHtmlTags(input);
            string res = input;
            string crop = input;
            try
            {
                if (maxCount > 0 && input.Length > maxCount)
                {
                    crop = input.Substring(0, maxCount);

                    for (int i = crop.Length - 1; i >= 0; i--)
                    {
                        if (crop[i] == ' ' || crop[i] == '.' || crop[i] == ',' || crop[i] == '-')
                        {
                            res = crop.Substring(0, i) + threeDots;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = input;
            }
            return res;
        }
        public static bool SaveFile(string path, string str)
        {
            bool res = false;
            try
            {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(str);
                }
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
            }
            return res;
        }

        public static string GetContentFromFile(string path)
        {
            string res = "";
            StreamReader sr = null;
            try
            {
                sr = File.OpenText(path);
                res = sr.ReadToEnd();
            }
            catch (Exception ex)
            { }
            finally
            {
               if(sr!=null) sr.Close();

            }
            return res;
        }
        public static string GenerateRandomPassword(int count = 8)
        {
            string allChar = "2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,j,k,l,m,n,p,q,r,s,t,u,w,x,y,z";
            Random random = new Random();
            string[] allCharArray = allChar.Split(',');
            Int32 r = 0;
            string s = string.Empty;
            for (Int32 i = 0; i < count; i++)
            {
                r = random.Next(allCharArray.GetLength(0));
                s += allCharArray[r];
            }
            return s;
        }

       public static string GetPage(string strURL)
        {
            String strResult = "";
            try
            {
                WebResponse objResponse;
                WebRequest objRequest = HttpWebRequest.Create(strURL);
                objResponse = objRequest.GetResponse();
                using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                {
                    strResult = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception ex)
            { }
            return strResult;
        }
       public static string GetMD5Hash(string input)
       {
           string res = "";
           MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
           byte[] bSignature = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

           StringBuilder sbSignature = new StringBuilder();
           foreach (byte b in bSignature)
               sbSignature.AppendFormat("{0:x2}", b);

           res = sbSignature.ToString();
           return res;
       }

       /**
       * Функция возвращает окончание для множественного числа слова на основании числа и массива окончаний
       * @param  number Integer Число на основе которого нужно сформировать окончание
       * @param  words  Array Массив слов или окончаний для чисел (1, 4, 5),
       *         например array('яблоко', 'яблока', 'яблок')
       * @return String
       */
       public static string GetDeclensionWords(int number, string[] words)
       {
           var res = string.Empty;
           if (words.Length >= 3)
           {
               number = number % 100;

               if (number >= 11 && number <= 19)
               {
                   res = words[2];
               }
               else
               {
                   switch (number % 10)
                   {
                       case (1): res = words[0]; break;
                       case (2):
                       case (3):
                       case (4): res = words[1]; break;
                       default: res = words[2]; break;
                   }
               }
           }
           else
           {
               if (words.Length > 0)
                   res = words[0];
           }

           return res;
       }

       /**
         * Функция возвращает переданное число в виде строки с разделением на разряды
         * @param  number decimal Число на основе которого нужно сформировать строку с разрядами
         * @param  separator  string разделитель разрядов
         * @return String
         */
       public static string GetThousandSeparated(decimal number, string separator = " ")
       {
           if (number == 0) return "0";
           if (number < 1000) return Math.Truncate(number).ToString();

           var nfi = (System.Globalization.NumberFormatInfo)System.Globalization.CultureInfo.InvariantCulture.NumberFormat.Clone();
           nfi.NumberGroupSeparator = separator;

           return number.ToString("#,#", nfi);
       }
    } 
}
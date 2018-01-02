using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailSender;
using System.Data;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace EmailSenderClient
{
    class Program
    {
        static void Main(string[] args)
        {

            //Email email = new Email();
            DataTable dt = new DataTable();
            dt.Columns.Add("WildCard");
            dt.Columns.Add("Value");
            dt.Rows.Add("{{Name}}", "Ashutosh");
            // email.Send("ashutoshpareek4@gmail.com", "TestEmail","ThankYou",dt);
            // string data = CurrencyConvert("5.45", "USD", "INR");
            string empowerPath = @"C:\Program Files\Diebold\Agilis EmPower\";
         string pf = empowerPath.Substring(0,  empowerPath.IndexOf("Diebold") -1);
            string path1 = @"Diebold\Ahu";
            path1 = Path.Combine(pf, path1);
            string path = @"C:\\Program Files\\Diebold\\Agilis Power\\Fan\\D91xCardReaderStatusHandler.dll";
            int index = path.IndexOf("Program Files");
            if(index!=-1)
            {
                path = path.Substring(index);
                
                //path = path.Substring(index+1);
                path = Path.Combine(empowerPath, path.Substring(path.IndexOf('\\') + 1));
            }
            string result = string.Empty;
            string apiURL = @"https://finance.google.com/finance/converter?a=54.5&from=USD&to=INR&meta=ei%3DSGMIWqC8IMyKuQSyjpzgDw";
            try
            {
                result = USDtoINR("54");

                Console.WriteLine(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static string USDtoINR(string amount)
        {
            return CurrencyConvert(amount, "USD", "INR");
        }
        public static string CurrencyConvert(string amount, string fromCurrency, string toCurrency)
        {

            //Grab your values and build your Web Request to the API
            //string apiURL = string.Format("https://www.google.com/finance/converter?a={0}&from={1}&to={2}&meta={3}", amount, fromCurrency, toCurrency, Guid.NewGuid().ToString());

            string apiURL = string.Format("https://finance.google.com/finance/converter?a={0}&from={1}&to={2}&meta={3}", amount,fromCurrency,toCurrency,Guid.NewGuid().ToString());

            string linkData = GetLinkData(apiURL);

            //Grab your converted value (ie 2.45 USD)
            var result = Regex.Matches(linkData, "<span class=\"?bld\"?>([^<]+)</span>")[0].Groups[1].Value.Replace("INR", "").Trim();

            //Get the Result
            return result;
        }

        public static string GetLinkData(string link)
        {
            string data = string.Empty;
            try
            {
                string address = link;
                using (var webpage = new WebClient())
                {
                    try
                    {
                        data = webpage.DownloadString(address);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        //Logger.WriteLogs("Utility", LogLevel.Error, "Not able to read the data from link.\n Exception: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.WriteLogs("Utility", LogLevel.Error, "Not able to read the data from link.\n Exception: " + ex.Message);
            }

            return data;
        }

        //public static string CurrencyConvert(string amount, string fromCurrency, string toCurrency)
        //{
        //    string result = null;
        //    //Grab your values and build your Web Request to the API
        //    // string apiURL = String.Format(@"https://www.google.com/finance/converter?a={0}&from={1}&to={2}&meta={3}", amount, fromCurrency, toCurrency, Guid.NewGuid().ToString());
        //    string apiURL = @"https://finance.google.com/finance/converter?a=54.5&from=USD&to=INR&meta=ei%3DSGMIWqC8IMyKuQSyjpzgDw";
        //    //Make your Web Request and grab the results



        //    //Grab your converted value (ie 2.45 USD)
        //    // string result = Regex.Matches(GetLinkData(apiURL), "<span class=\"?bld\"?>([^<]+)</span>")[0].Groups[1].Value;

        //    //Get the Result
        //    return result;
        //}
    }
}

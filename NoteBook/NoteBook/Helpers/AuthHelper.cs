using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using NoteBook.Settings;

namespace NoteBook.Helpers
{
    public class AuthHelper
    {
        public static void SetAuthKey(string aspxauth)
        {
            if (string.IsNullOrEmpty(aspxauth)) return;

            UserSettings.AuthKey = aspxauth.Split('=')[0];
            UserSettings.AuthValue = aspxauth.Split('=')[1].Split(';')[0];
            string time = Regex.Match(aspxauth, @"(?<=expires=)(.*)(?= GMT;)").ToString();
            if (!string.IsNullOrEmpty(time))
            {
                var myDate = DateTime.ParseExact(time, "ddd, dd-MMM-yyyy HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture).AddHours(2);
                UserSettings.Expiress = myDate.ToString("G");
            }
            else
            {
                var tempTime = DateTime.Now.AddDays(7);
                UserSettings.Expiress = tempTime.ToString("G");
            }
        }

        public static void ClearAll()
        {
            UserSettings.AuthValue = string.Empty;
            UserSettings.Expiress = string.Empty;
            UserSettings.SyncDate = string.Empty;
            UserSettings.UserName = string.Empty;
        }

        public static HttpClient GetAuthHttpClient()
        {
            var handler = new HttpClientHandler { CookieContainer = new CookieContainer() };

            if (!string.IsNullOrEmpty(UserSettings.AuthKey) && !string.IsNullOrEmpty(UserSettings.AuthValue))
            {
                handler.CookieContainer.Add(new Uri(Settings.Settings.Url), new Cookie(UserSettings.AuthKey, UserSettings.AuthValue));
            }

            return new HttpClient(handler);
        }

        public static bool IsLoged()
        {
            if (!string.IsNullOrEmpty(UserSettings.Expiress))
            {
                return DateTime.Now < Convert.ToDateTime(UserSettings.Expiress);
            }
            return false;
        }
    }
}
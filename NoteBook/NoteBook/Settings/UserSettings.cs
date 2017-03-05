using Xamarin.Forms;

namespace NoteBook
{
    public class UserSettings
    {

        public static string UserName
        {
            get
            {
                if (!Application.Current.Properties.ContainsKey("UserName"))
                {
                    return null;
                }
                return Application.Current.Properties["UserName"].ToString();
            }
            set { Application.Current.Properties["UserName"] = value; }
        }

        public static string AuthKey
        {
            get
            {
                if (!Application.Current.Properties.ContainsKey("AuthKey"))
                {
                    return null;
                }
                return Application.Current.Properties["AuthKey"].ToString();
            }
            set { Application.Current.Properties["AuthKey"] = value; }
        }

        public static string AuthValue
        {
            get
            {
                if (!Application.Current.Properties.ContainsKey("AuthValue"))
                {
                    return null;
                }
                return Application.Current.Properties["AuthValue"].ToString();
            }
            set { Application.Current.Properties["AuthValue"] = value; }
        }

        public static string Expiress
        {
            get
            {
                if (!Application.Current.Properties.ContainsKey("Expiress"))
                {
                    return null;
                }
                return Application.Current.Properties["Expiress"].ToString();
            }
            set { Application.Current.Properties["Expiress"] = value; }
        }

        public static string SyncDate
        {
            get
            {
                if (!Application.Current.Properties.ContainsKey("SyncDate"))
                {
                    return null;
                }
                var c = Application.Current.Properties["SyncDate"].ToString();
                return Application.Current.Properties["SyncDate"].ToString();
            }
            set
            {
                var d = value;
                Application.Current.Properties["SyncDate"] = value; }
        }
    }
}
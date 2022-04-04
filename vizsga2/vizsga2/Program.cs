using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace vizsga2
{
    public class Sor
    {
        public List<string> data = new List<string>();

        public void AddData(string dataToAdd)
        {
            data.Add(dataToAdd);
        }
    }

    static class Program
    {

        public static void writeReport(string text)
        {
            FileStream fs = new FileStream(@"jelentés.txt", FileMode.Append);

            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd | HH:mm:ss")}]: {text}");

            sw.Close();
            fs.Close();
        }

        public static string processPassword(string toHash)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(toHash));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public static void loadConfig()
        {
            StreamReader sr = new StreamReader("config.txt");

            password = sr.ReadLine().Split('=')[1];
            askPassword = bool.Parse(sr.ReadLine().Split('=')[1]);
            DataExportPath = sr.ReadLine().Split('=')[1];
            backGroundColor = Color.FromArgb(int.Parse(sr.ReadLine().Split('=')[1]));
            foreGroundColor = Color.FromArgb(int.Parse(sr.ReadLine().Split('=')[1]));
            textColor = Color.FromArgb(int.Parse(sr.ReadLine().Split('=')[1]));
            buttonColor = Color.FromArgb(int.Parse(sr.ReadLine().Split('=')[1]));

            sr.Close();
        }

        static public string newCategory;
        static public string sqlPASSWORD = "Jelszo123!";
        static public MySqlConnection conn = new MySqlConnection("SERVER=localhost;DATABASE=leltarrendszer;UID=leltarrendszer;PASSWORD=" + sqlPASSWORD + ";SslMode=none;charset=utf8");

        static public string category;
        static public string password;
        static public bool askPassword = true;
        static public bool passwordCorrect = false;

        static public string DataExportPath;

        static public Color backGroundColor = Color.FromName("Control");
        static public Color foreGroundColor = Color.FromName("Control");
        static public Color textColor = Color.FromName("ControlText");
        static public Color buttonColor = Color.FromName("Control");

        [STAThread]
        static void Main()
        {
            if (!File.Exists("config.txt"))
            {
                FileStream fs = new FileStream("config.txt", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

                string path = Directory.GetCurrentDirectory().ToString();
                var parent = Directory.GetParent(path).FullName;
                var newPath = Path.Combine(parent, "Adatok");

                sw.WriteLine($"Password={processPassword("root")}");
                sw.WriteLine($"AskPassword=true");
                sw.WriteLine($"DataExportPath={newPath}");
                sw.WriteLine($"BackGroundColor={backGroundColor.ToArgb()}");
                sw.WriteLine($"ForeGroundColor={foreGroundColor.ToArgb()}");
                sw.WriteLine($"TextColor={textColor.ToArgb()}");
                sw.WriteLine($"ButtonColor={buttonColor.ToArgb()}");

                sw.Close();
                fs.Close();
            }

            loadConfig();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Adatok());
        }
    }
}

using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace INTJBot
{
    class Program
    {
        // STAThread has to be used because OpenFileDialog had issues running on a static thread
        // Required for accessing OLE functions
        [STAThread]
        static void Main(string[] args)
        {
            string auth = ConfigurationManager.AppSettings["AuthenticationToken"];
            new Bot(auth).Start().GetAwaiter().GetResult();
        }
        
        private static string OpenFileChooser()     // This method is used for choosing a file which holds a token
        {
            string filename = "";
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    filename = openFileDialog1.FileName;
                }
            }
            return filename;
        }

        // Method reads the first line from the file. Assuming you have the token string as the only line in there
        private static string GetBotToken(string filename) 
        {
            string token = "";
            if (File.Exists(filename))
            {
                token = File.ReadLines(filename).First();
            }
            return token;
        }
    }

}

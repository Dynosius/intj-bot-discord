using System;
using System.Configuration;

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
    }
}

namespace ПрактикаРМПО
{
    internal static class Program
    {
        public static string user = "Гость";
        public static int role = 0;
        public static int manager_admin_appeal = 0;
        
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}
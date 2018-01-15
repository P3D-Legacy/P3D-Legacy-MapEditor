using System;
using System.IO;
using System.Windows.Forms;
using P3D_Legacy.MapEditor.World;

namespace P3D_Legacy.MapEditor
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var text = new StreamReader("").ReadToEnd();
            var level = LevelLoader.Load(text);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
#endif
}

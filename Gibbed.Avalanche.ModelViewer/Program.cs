using System;
using System.Windows.Forms;

namespace Gibbed.Avalanche.ModelViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Viewer());
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "Press CTRL+C to copy this dialog.\n\n" + e.ToString(),
                    "Exception!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

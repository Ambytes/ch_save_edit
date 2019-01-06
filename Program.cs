using System;
using System.Windows.Forms;

namespace ch_save_edit
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SaveEditorWindow());
        }
    }
}

using System;
using System.IO;
using System.Windows.Forms;

namespace EtrendKeszito
{
    public static class Logolas
    {
        public static void Ment(string str)
        {
            try
            {
                str = "Time: " + DateTime.Now + Environment.NewLine + str + Environment.NewLine;
                File.AppendAllText(Konstans.logfilename, str);
            }
            catch (Exception e)
            {
                MessageBox.Show("Unsuccessful log!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

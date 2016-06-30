using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace test
{
    public partial class Form1 : Form
    {
        private IDictionary<IntPtr, string> openWindowList = OpenWindowGetter.GetOpenWindows();
        IntPtr windowHandle = new IntPtr();
        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("USER32.DLL")]
        public static extern IntPtr GetForegroundWindow();

        public Form1()
        {
            InitializeComponent();

            this.comboBox2.DataSource = new BindingSource(openWindowList, null);
            this.comboBox2.DisplayMember = "Value";
            this.comboBox2.ValueMember = "Key";

            comboBox1.SelectedItem = "Clip Board";

            button1.Click += new EventHandler(sendText);
            button3.Click += new EventHandler(refreshOpenWindows);
            button2.Click += new EventHandler(pickFile);
        }
        private void sendText(object sender, EventArgs e)
        {
            try
            {
                // Get a handle to the Calculator application. The window class
                // and window name were obtained using the Spy++ tool.
                windowHandle = ((KeyValuePair<IntPtr, string>)comboBox2.SelectedItem).Key;

                // Make the foreground application and send it keys.
                SetForegroundWindow(windowHandle);
                string strToSend = "";
                if ((string)comboBox1.SelectedItem == "Text Box")
                {
                    strToSend = textBox1.Text;
                    writeText(strToSend);
                }
                else if ((string)comboBox1.SelectedItem == "Clip Board")
                {
                    if (Clipboard.ContainsText(TextDataFormat.Text))
                    {
                        strToSend = Clipboard.GetText(TextDataFormat.Text);
                        writeText(strToSend);
                    }
                }
                else if ((string)comboBox1.SelectedItem == "File")
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(label1.Text);
                    strToSend=File.ReadAllText(label1.Text);
                    writeText(strToSend);
                    file.Close();
                }
            }
            catch (Exception exc) {
                MessageBox.Show(exc.Message);
            }
        }
        private void writeText(String strToSend) {
            if (checkBox1.Checked) {
                strToSend = Base64Encode(strToSend);
            }
            IEnumerable<string> list = split(strToSend, (int)numericUpDown1.Value);
            foreach(string str in list)
            {
                
                if (checkBox1.Checked)
                {
                    strToSend = str.Replace("+", "{+}");
                }
                else { 
                    strToSend = str.Replace("{", "|opnbr|");
                    strToSend = strToSend.Replace("}", "|clsbr|");
                    strToSend = strToSend.Replace(System.Environment.NewLine, "{Enter}");
                    strToSend = strToSend.Replace("\t", "{Tab}");
                    strToSend = strToSend.Replace("+", "{+}");
                    strToSend = strToSend.Replace("^", "{^}");
                    strToSend = strToSend.Replace("~", "{~}");
                    strToSend = strToSend.Replace("%", "{%}");
                    strToSend = strToSend.Replace("&", "{&}");
                    strToSend = strToSend.Replace("(", "{(}");
                    strToSend = strToSend.Replace(")", "{)}");
                    strToSend = strToSend.Replace("]", "{]}");
                    strToSend = strToSend.Replace("[", "{[}");
                    strToSend = strToSend.Replace("|opnbr|", "{{}");
                    strToSend = strToSend.Replace("|clsbr|", "{}}");
                    }
                if (GetForegroundWindow() != windowHandle)
                {
                    return;
                }
                SendKeys.SendWait(strToSend);
            }
                           
            
        }
        static IEnumerable<string> split(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        private void refreshOpenWindows(object sender, EventArgs e)
        {
            openWindowList = OpenWindowGetter.GetOpenWindows();
            comboBox2.DataSource = new BindingSource(openWindowList, null);
            comboBox2.DisplayMember = "Value";
            comboBox2.ValueMember = "Key";
        }

        private void pickFile(object sender, EventArgs e)
        {
        
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                label1.Text = openFileDialog1.FileName;
            }

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

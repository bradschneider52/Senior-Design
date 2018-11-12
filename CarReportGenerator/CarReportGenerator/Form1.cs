using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarReportGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        OpenFileDialog ofd = new OpenFileDialog();
        private void button1_Click(object sender, EventArgs e)
        {
            ofd.Filter = "dbc files|*.dbc"; //only allows dbc file extensions
            if (ofd.ShowDialog() == DialogResult.OK)
            {

                System.IO.StreamReader sr = new System.IO.StreamReader(ofd.FileName);
                richTextBox1.Text = sr.ReadToEnd().ToString();
                //MessageBox.Show(sr.ReadToEnd());
                sr.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ofd.Filter = "dbc files|*.dbc"; //only allows dbc file extensions
            if (ofd.ShowDialog() == DialogResult.OK)
            {

                System.IO.StreamReader sr = new System.IO.StreamReader(ofd.FileName);
                richTextBox2.Text = sr.ReadToEnd().ToString();
                //MessageBox.Show(sr.ReadToEnd());
                sr.Close();
            }
        }
    }
}

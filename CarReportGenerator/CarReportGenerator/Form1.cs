using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using canlibCLSNET;
using Kvaser.Kvadblib;

using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;


//using Kvaser;

namespace CarReportGenerator
{
    public partial class View : Form
    {

        private Thread graphthread;
        private double[] signalArray = new double[30];
        public byte[] can_data = new byte[1024];
        public double F = 0;
        public double minimum = 0;
        public double maximum = 0;
        public int Insert;
        public View()
        {
            InitializeComponent();
            comboBox1.Items.Clear();
            //Canlib.canInitializeLibrary();
            
            
        }
        //String[] events1, events2, similarEvents;
        //List<string> similarEvents = new List<string>();


        // The graph at the bottom is experimental and doesnt sketch anything accurate yet


        OpenFileDialog ofd = new OpenFileDialog();
        public string filename;
        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            ofd.Filter = "dbc files|*.dbc"; //only allows dbc file extensions
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filename = ofd.FileName;
                ofd.Dispose();
                Kvadblib.Status status = this.dumpDatabase(filename, richTextBox1);
                
            }
        }
        
        

        private void button2_Click(object sender, EventArgs e)
        {
            ofd.Filter = "dbc files|*.dbc"; //only allows dbc file extensions
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filename = ofd.FileName;
               Kvadblib.Status status = this.dumpDatabase(filename, richTextBox2);

            }

        }

        private Kvadblib.Status dumpDatabase(String f, RichTextBox rt)
        {
            Kvadblib.Status status;
            Kvadblib.Hnd dh = new Kvadblib.Hnd();
            Kvadblib.MessageHnd mh = new Kvadblib.MessageHnd();
            Kvadblib.SignalHnd sh = new Kvadblib.SignalHnd();
            Kvadblib.DATABASE flags;
            Kvadblib.MESSAGE mflags;

            //Open a database handle
            status = Kvadblib.Open(out dh);
            if (status != Kvadblib.Status.OK)
            {
                Console.WriteLine("Could not create a database handle: " + status + "\n");
                return status;
            }

            //Load the database file

            status = Kvadblib.ReadFile(dh, f);
            if (status != Kvadblib.Status.OK)
            {
                Console.WriteLine("Could not load the database file: " + status + "\n");
                return status;
            }

            //Get the flags 
            status = Kvadblib.GetFlags(dh, out flags);
            if (status != Kvadblib.Status.OK)
            {
                Console.WriteLine("kvadblib.GetFlags failed: " + status + "\n");
                return status;
            }
            Console.WriteLine("Database: flags = 0x" + flags + "\n");

            //Get the first messsage in the database
            status = Kvadblib.GetFirstMsg(dh, out mh);
            if (status != Kvadblib.Status.OK)
            {
                Console.WriteLine("kvadblib.GetFirstMsg failed: " + status + "\n");
                return status;
            }

            //Go through all the messages in the database
            while (status == Kvadblib.Status.OK)
            {
                string msg_name;
                string msg_qname;
                string msg_comment;
                int dlc = 0;
                int id = 0;


                //clear the strings
                msg_name = string.Empty;
                msg_qname = string.Empty;
                msg_comment = string.Empty;

                //Get the properties for each message
                status = Kvadblib.GetMsgName(mh, out msg_name);
                // TO BE FIXED events.Add(msg_name);
                if (status != Kvadblib.Status.OK)
                {
                    Console.WriteLine("kvadblib.GetMsgName failed: " + status + "\n");
                    return status;
                }

                status = Kvadblib.GetMsgQualifiedName(mh, out msg_qname);
                if (status != Kvadblib.Status.OK)
                {
                    Console.WriteLine("kvadblib.GetMsgQualifiedName failed: " + status + "\n");
                    return status;
                }

                status = Kvadblib.GetMsgComment(mh, out msg_comment);
                if (status != Kvadblib.Status.OK)
                {
                    Console.WriteLine("kvadblib.GetMsgComment failed: " + status + "\n");
                    return status;
                }

                status = Kvadblib.GetMsgId(mh, out id, out mflags);
                if (status != Kvadblib.Status.OK)
                {
                    Console.WriteLine("kvadblib.GetMsgId failed: " + status + "\n");
                    return status;
                }

                status = Kvadblib.GetMsgDlc(mh, out dlc);
                if (status != Kvadblib.Status.OK)
                {
                    Console.WriteLine("kvadblib.GetMsgDlc failed: " + status + "\n");
                    return status;
                }

                //Print the properties for each message
               
                rt.AppendText(" MESSAGE NAME = " + msg_name + "\n");
                rt.AppendText("         qualifying message name = " + msg_qname + "\n");
                rt.AppendText("         comment = " + msg_comment + "\n");
                rt.AppendText("         message id = " + id + "\n ");
                rt.AppendText("         data length code = " + dlc + "\n");
                rt.AppendText("         flags = " + flags + "\n");
                

                //Go through all signals for this message
                status = Kvadblib.GetFirstSignal(mh, out sh);
                while (status == Kvadblib.Status.OK)
                {
                    string signal_name;
                    byte[] data = new byte[1024];
                    string signal_qname;
                    string signal_comment;
                    string signal_unit;
                    Kvadblib.SignalEncoding sigEnc;
                    Kvadblib.SignalType sigType;
                    int startbit = 0;
                    int i = 1;
                    int length = 0;
                    double minval = 0;
                    double maxval = 0;
                    double factor = 0;
                    double offset = 0;
                    
                    //Reset the strings
                  
                    signal_name = string.Empty;
                    signal_qname = string.Empty;
                    signal_comment = string.Empty;
                    signal_unit = string.Empty;

                    //Get the properties for each signal
                    status = Kvadblib.GetSignalName(sh, out signal_name);
                    //events.Add(signal_name);
                    if (status != Kvadblib.Status.OK)
                    {
                        Console.WriteLine("kvadblib.GetSignalName failed: " + status + "\n");
                        return status;
                    }
                    //events.Add(signal_name);




                    //TEST CODE FROM KVASER

                    status = Kvadblib.RetrieveSignalValuePhys(sh,out F,can_data);
                    if (status != Kvadblib.Status.OK)
                    {
                        MessageBox.Show("Kvadlib.RetrieveSignalVlauePhys has failed" + status + "");
                        return status;
                    }

                    //RETRIEVESIGNALVALUERAW

                    status = Kvadblib.GetSignalValueFloat(sh, out F, data, dlc);
                    if (status != Kvadblib.Status.OK)
                    {
                        MessageBox.Show("Retrieve message failed" + status + "");
                        return status;
                    }







                    status = Kvadblib.GetSignalQualifiedName(sh, out signal_qname);
                    if (status != Kvadblib.Status.OK)
                    {
                        Console.WriteLine("kvadblib.GetSignalQualifiedName failed: " + status + "\n");
                        return status;
                    }

                    status = Kvadblib.GetSignalComment(sh, out signal_comment);
                    if (status != Kvadblib.Status.OK)
                    {
                        Console.WriteLine("kvadblib.GetSignalComment failed: " + status + "\n");
                        return status;
                    }

                    status = Kvadblib.GetSignalUnit(sh, out signal_unit);
                    if (status != Kvadblib.Status.OK)
                    {
                        Console.WriteLine("kvadblib.GetSignalUnit failed: " + status + "\n");
                        return status;
                    }

                    status = Kvadblib.GetSignalEncoding(sh, out sigEnc);
                    if (status != Kvadblib.Status.OK)
                    {
                        Console.WriteLine("kvadblib.GetSignalEncoding failed: " + status + "\n");
                        return status;
                    }

                    status = Kvadblib.GetSignalRepresentationType(sh, out sigType);
                    if (status != Kvadblib.Status.OK)
                    {
                        Console.WriteLine("kvadblib.GetSignalRepresentation failed: " + status + "\n");
                        return status;
                    }

                    status = Kvadblib.GetSignalValueLimits(sh, out minval, out maxval);
                    if (status != Kvadblib.Status.OK)
                    {
                        Console.WriteLine("kvadblib.GetSignalValueLimits failed: " + status + "\n");
                        minimum = minval;
                        maximum = maxval;
                        return status;
                    }

                    status = Kvadblib.GetSignalValueScaling(sh, out factor, out offset);
                    if (status != Kvadblib.Status.OK)
                    {
                        Console.WriteLine("kvadblib.GetSignalValueScaling failed: " + status + "\n");
                        return status;
                    }

                    status = Kvadblib.GetSignalValueSize(sh, out startbit, out length);
                    if (status != Kvadblib.Status.OK)
                    {
                        Console.WriteLine("kvadblib.GetSignalValueSize failed: " + status + "\n");
                        return status;
                    }

                    //additional signal value integer
                    status = Kvadblib.GetSignalValueInteger(sh,out i, data, dlc);
                    Console.WriteLine("size of data=" + data.ToArray<byte>().Length);
                    
                    //Console.WriteLine();
                    rt.AppendText("             SIGNAL NAME = " + signal_name + "\n");
                    rt.AppendText("                                 unit = " + signal_unit.ToString() + "\n");
                    rt.AppendText("                                 signal qualifying name = " + signal_qname + "\n");
                    rt.AppendText("                                 signal comment = " + signal_comment.ToString() + "\n");
                    rt.AppendText("                                 signal encoding = " + sigEnc.ToString() + "\n");
                    rt.AppendText("                                 signal representation = " + sigType.ToString() + "\n");
                    rt.AppendText("                                 min sig value = " + minval.ToString() + "\n");
                    rt.AppendText("                                 max sig value = " + maxval.ToString() + "\n");
                    rt.AppendText("                                 signal scale factor = " + factor.ToString() +"\t"+ ", offset = " + offset.ToString() + "\n");
                    rt.AppendText("                                 signal startbit = " + startbit +"\t"+ ", sig length = " + length + "\n");
                    rt.AppendText("                                 signal value:\n");
                   // rt.AppendText("                                 Physical signal value = " + F + "\n");
                    rt.AppendText("                                 SIGNAL value float = " + F+"\n");
                    var sb = new StringBuilder("                    signal integer data =");
                    foreach (var b in data)
                        sb.Append(b + ",");
                    status = Kvadblib.GetNextSignal(mh, out sh);
                }
                status = Kvadblib.GetNextMsg(dh, out mh);
            }
            status = Kvadblib.Close(dh);
            if(status != Kvadblib.Status.OK)
            {
                Console.WriteLine("kvadblib.Close failed: \n");
                return status;
            }
            Console.WriteLine("\n\n");
            return Kvadblib.Status.OK;
        }


        private void importMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exportMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            chart1.Series["Series1"].Points.AddXY(10, 100);
            chart1.Series["Series1"].ChartType = SeriesChartType.FastLine;
            chart1.Series["Series1"].Color = Color.Red;
        }

        /*
private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
{

}

private void tabPage1_Click(object sender, EventArgs e)
{

}
*/
    }
}

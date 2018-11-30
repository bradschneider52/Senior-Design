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



//using Kvaser;

namespace CarReportGenerator
{
    public partial class View : Form
    {
        public View()
        {
            InitializeComponent();
            comboBox1.Items.Clear();
            //Canlib.canInitializeLibrary();
            
            
        }
        String path;
        //String[] events1, events2, similarEvents;
        List<string> events1 = new List<string>();
        List<string> events2 = new List<string>();
        List<string> similarEvents = new List<string>();
        OpenFileDialog ofd = new OpenFileDialog();

        private void button1_Click(object sender, EventArgs e)
        {
            ofd.Filter = "dbc files|*.dbc"; //only allows dbc file extensions
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
                Kvadblib.Status status = this.dumpDatabase(path, richTextBox1, events1);
                //sr.Close();
            }

            //if statement to see if you can compare events yet
            if(events1.Any() && events2.Any())
            {
                getSimilarEvents();
            }
        }
        
        

        private void button2_Click(object sender, EventArgs e)
        {
            ofd.Filter = "dbc files|*.dbc"; //only allows dbc file extensions
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
                Kvadblib.Status status = this.dumpDatabase(path, richTextBox2, events2);
            }

            //if statement to see if you can compare events yet
            if (events1.Any() && events2.Any())
            {
                getSimilarEvents();
            }

        }

        private Kvadblib.Status dumpDatabase(String f,RichTextBox rt, List<string> events)
        {
            Kvadblib.Status status;
            Kvadblib.Hnd dh = new Kvadblib.Hnd();
            Kvadblib.MessageHnd mh = new Kvadblib.MessageHnd();
            Kvadblib.SignalHnd sh = new Kvadblib.SignalHnd();
            Kvadblib.DATABASE flags;
            Kvadblib.MESSAGE mflags;

            //Open a database handle
            status = Kvadblib.Open(out dh);
            if(status != Kvadblib.Status.OK)
            {
                Console.WriteLine("Could not create a database handle: " + status + "\n");
                return status;
            }

            //Load the database file
            
            status = Kvadblib.ReadFile(dh,path.ToString());
            if(status != Kvadblib.Status.OK)
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
            while(status == Kvadblib.Status.OK)
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
                events.Add(msg_name);
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
                rt.AppendText("MESSAGE:\n");
                rt.AppendText("         name = " + msg_name + "\n");
                rt.AppendText("         qname = " + msg_qname + "\n");
                rt.AppendText("         comment = " + msg_comment + "\n");
                rt.AppendText("         id = " + id + " dlc = " + dlc + "flags = " + flags + "\n");

                //Go through all signals for this message
                status = Kvadblib.GetFirstSignal(mh, out sh);
                while(status == Kvadblib.Status.OK)
                {
                    string signal_name;
                    string signal_qname;
                    string signal_comment;
                    string signal_unit;
                    Kvadblib.SignalEncoding sigEnc;
                    Kvadblib.SignalType sigType;
                    int startbit = 0;
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
                    events.Add(signal_name);

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

                    rt.AppendText("Signal: \n");
                    //Console.WriteLine();
                    rt.AppendText("       name = " + signal_name + ", unit = " + signal_unit + "\n");
                    rt.AppendText("       qname = " + signal_qname + "\n");
                    rt.AppendText("       comment = " + signal_comment + "\n");
                    rt.AppendText("       encoding = " + sigEnc.ToString() + "\n");
                    rt.AppendText("       representation = " + sigType.ToString() + "\n");
                    rt.AppendText("       min value = " + minval.ToString("F2") + ", max value = " + maxval.ToString("F2") + "\n");
                    rt.AppendText("       scale factor = " + factor.ToString("F2") + ", offset = " + offset.ToString("F2") + "\n");
                    rt.AppendText("       startbit = " + startbit + ", length = " + length + "\n");
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

        private void getSimilarEvents()
        {
            comboBox1.Items.Clear();
            foreach(string s1 in events1)
            {
                foreach(string s2 in events2)
                {
                    if (s1.Contains(s2) && s1 != "CHECKSUM" && s1 != "COUNTER")
                    {
                        comboBox1.Items.Add(s1);
                    }//if
                    
                }//foreach
            }//foreach
        }//findSimilarNames

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

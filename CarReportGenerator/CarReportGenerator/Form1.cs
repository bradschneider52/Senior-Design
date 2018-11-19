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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Canlib.canInitializeLibrary();
        }

        OpenFileDialog ofd = new OpenFileDialog();
        private void button1_Click(object sender, EventArgs e)
        {
            ofd.Filter = "dbc files|*.dbc"; //only allows dbc file extensions
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(ofd.FileName);
                //Canlib.r
                
                richTextBox1.Text = sr.ReadToEnd().ToString();
                //MessageBox.Show(sr.ReadToEnd());
                sr.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //test the dumpdatabase function 
            Kvadblib.Status status = this.dumpDatabase();
            ofd.Filter = "dbc files|*.dbc"; //only allows dbc file extensions
            if (ofd.ShowDialog() == DialogResult.OK)
            {

                System.IO.StreamReader sr = new System.IO.StreamReader(ofd.FileName);
                richTextBox2.Text = sr.ReadToEnd().ToString();
                //MessageBox.Show(sr.ReadToEnd());
                sr.Close();
            }
        }

        private Kvadblib.Status dumpDatabase()
        {
            Kvadblib.Status status;
            Kvadblib.Hnd dh = null;
            Kvadblib.MessageHnd mh = null;
            Kvadblib.SignalHnd sh = null;
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
            status = Kvadblib.ReadFile(dh, "C:\\Users\\Ian\\Documents\\Ian Taylor PSU\\SeniorDesign\\Senior-Design\\Sample_dbc_Files\\acura_ilx_2016_can.dbc");
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
                //char[] msg_name = new char[100];
                //char[] msg_qname = new char[200];
                //char[] msg_comment = new char[200];
                string msg_name;
                string msg_qname;
                string msg_comment;
                int dlc = 0;
                int id = 0;


                //clear the strings
                msg_name = string.Empty;
                msg_qname = string.Empty;
                msg_comment = string.Empty;
                //Array.Clear(msg_name, 0, 100);
                //Array.Clear(msg_qname, 0, 100);
                //Array.Clear(msg_comment, 0, 200);

                //Get the properties for each message
                status = Kvadblib.GetMsgName(mh, out msg_name);
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
                Console.WriteLine("MESSAGE:\n");
                Console.WriteLine("         name = " + msg_name + "\n");
                Console.WriteLine("         qname = " + msg_qname + "\n");
                Console.WriteLine("         comment = " + msg_comment + "\n");
                Console.WriteLine("         id = " + id + " dlc = " + dlc + "flags = " + flags + "\n");

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
                    if (status != Kvadblib.Status.OK)
                    {
                        Console.WriteLine("kvadblib.GetSignalName failed: " + status + "\n");
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

                    Console.WriteLine("Signal: \n");
                    Console.WriteLine("       name = " + signal_name + ", unit = " + signal_unit + "\n");
                    Console.WriteLine("       qname = " + signal_qname + "\n");
                    Console.WriteLine("       comment = " + signal_comment + "\n");
                    Console.WriteLine("       encoding = " + sigEnc.ToString() + "\n");
                    Console.WriteLine("       representation = " + sigType.ToString() + "\n");
                    Console.WriteLine("       min value = " + minval.ToString("F2") + ", max value = " + maxval.ToString("F2") + "\n");
                    Console.WriteLine("       scale factor = " + factor.ToString("F2") + ", offset = " + offset.ToString("F2") + "\n");
                    Console.WriteLine("       startbit = " + startbit + ", length = " + length + "\n");
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
    }
}

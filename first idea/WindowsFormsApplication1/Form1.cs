using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
//!!!!!!!***************speed smpling/send arduio ba refresh  speed in yeki nabashe ya bishtare ya kamter mmokene aan aghab bemone ya dir neshn bedeh
//*************age buffer in sabet bashe toolesh yani speed ha barabare va harche toole buffer kamtar bashe yani realtime tare(real time be speed  sample/send arduio ham bastegi dare va be refresh rate in)

//********idea multi channel+baraye multi channel bayad protocol design beshe ke baham chand value berfese va be ge male kodom zaman
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

         
      
        int block_pixels = 100;  


        SerialPort port;
        List<Queue<int>> allchannels = new List<Queue<int>>();
        List<Pen> colors = new List<Pen>();



        System.Timers.Timer aTimer = new System.Timers.Timer();


        //display
        int horizal = 500;
        int vertical = 500;

        //real depth
        // int max = 1023;


         


        

        bool[] chsenable = { false, false, false, false };
        


        private void Form1_Load(object sender, EventArgs e)
        {


            

            allchannels.Add(new Queue<int>());
            allchannels.Add(new Queue<int>());
            allchannels.Add(new Queue<int>());
            allchannels.Add(new Queue<int>());


            //create empty queues of cannels
            for (int i=0; i<horizal; i++)
            {
                for (int j = 0; j < allchannels.Count; j++)
                {
                    allchannels[j].Enqueue(0);
                }
               
            }

            colors.Add(Pens.Yellow);
            colors.Add(Pens.LightGreen);
            colors.Add(Pens.Red);
            colors.Add(Pens.AliceBlue);















            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 10;
           

            port = new SerialPort("COM6", 9600); 
            

            this.DoubleBuffered = true;


          
        }
        
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            //rahe behtar ine ke arduio be buffer inja befrestad va ma az buffer chon timer windows taghribi ast
            // while (true)  


            //khode class port buffer darad dar c#
           // Random r = new Random();
           // channel1data.Enqueue( r.Next(50,70));
           // channel1data.Dequeue();
           // Invalidate();
          //  return;


            if (port.BytesToRead > 0) //age data nadashtim   ZERO BEZARIM  ya haman ghabli redraw???  in ravehs pull ast, payad push basahd az samte arduio
                {

                // Convert.ToInt16(TextBoxD1.Text);
                // try{
                    string s  = port.ReadLine(); //blocks until new line arrive 
                    string[] channlesdata = s.Split("#".ToCharArray());
                //age bishtar available bood chi?           

                for (int i = 0; i < channlesdata.Count(); i++)
                {
                    allchannels[i].Enqueue(Convert.ToInt32(channlesdata[i].Replace("\r","")));
                    allchannels[i].Dequeue();
                }

                Invalidate();




                //int index = d.IndexOf('#');
                //if (index > 0)
                //{
                //    string sect1 = d.Substring(0, index - 1);
                //    d = d.Remove(0, index); //baghieash bezarim bashe bara frame baad
                //    data.Enqueue(Convert.ToInt32(sect1));
                //    data.Dequeue();
                //    Invalidate();
                //}
                //if (index == 0) { d = d.Remove(0, 1); }


            }
            else
            {
                //yani sorat arduio kmatare ...
                //////??

            }








        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {


            //horizal line
            // e.Graphics.DrawLine(Pens.Black, new Point(0, vertical), new Point(horizal, vertical));

            //vertical line
            // e.Graphics.DrawLine(Pens.Black, new Point(horizal, 0), new Point(horizal, vertical));

            //background color

            e.Graphics.FillRectangle(Brushes.Black, 0, 0, horizal, vertical);
            for (int i = 0; i < vertical / block_pixels; i++) //???kasri shod chi?????
            {
                e.Graphics.DrawLine(Pens.Gray, 0, i * block_pixels, horizal, i * block_pixels);

            }


            for (int i = 0; i < horizal / block_pixels; i++)
            {
                e.Graphics.DrawLine(Pens.Gray, i * block_pixels, 0, i * block_pixels, vertical);

            }





            // e.Graphics.DrawString("Current: "+ channel1data.Last().ToString(), new Font(FontFamily.GenericSerif, 20), Brushes.Green, new Point(0, vertical));
            e.Graphics.DrawString("5".ToString(), new Font(FontFamily.GenericSerif, 20), Brushes.Green, new Point(horizal, 0));
            e.Graphics.DrawString("0".ToString(), new Font(FontFamily.GenericSerif, 20), Brushes.Green, new Point(horizal, vertical - 30));






            //firt convert discrete data to point, in order to draw continuous of lines






           





            
            for (int ch = 0; ch < allchannels.Count; ch++)
            {
                if (chsenable[ch] == false) continue;

                int x = 0;
                Point[] ppp = new Point[horizal];
                foreach (int y in allchannels[ch])///allchannels[ch].count=queuelnegt 
                {




                    int graphicvalue  = vertical - y;
                         
                     
                    

                     
                    ppp[x] = new Point(x, graphicvalue);
                    x += 1;

                }
                e.Graphics.DrawLines(colors[ch], ppp);
                e.Graphics.FillEllipse(colors[ch].Brush, new Rectangle(ppp[x - 1].X - 5, ppp[x - 1].Y - 3, 7, 7));




            }

           

            



        }
        
      
      
      



        bool connected = false;
        private void button1_Click(object sender, EventArgs e)
        {


            try{

                if (!connected)
                {
                    button1.Enabled = false;
                    port.Open();
                    port.Write("1");
                    aTimer.Enabled = connected = true;
                    button1.Text = "stop";
                    button1.Enabled = true;
                }
                else
                {
                    button1.Enabled = false;
                    aTimer.Enabled = connected = false;
                    port.Close();
                    button1.Text = "start";
                    button1.Enabled = true;


                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Restart();

            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            chsenable[0] = !chsenable[0];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            chsenable[1] = !chsenable[1];
        }

        private void button4_Click(object sender, EventArgs e)
        {
            chsenable[2] = !chsenable[2];
        }

        private void button5_Click(object sender, EventArgs e)
        {
            chsenable[3] = !chsenable[3];
        }
    }
}

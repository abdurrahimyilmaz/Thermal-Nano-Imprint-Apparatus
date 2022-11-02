using System;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace tia
{
    public partial class Form1 : Form
    {
        string sonuc = "0";
        int maksm = 20, minm = 0;
        int chartCount = 0;
        int[,] chartPoints = new int[4,2];

        public Form1()
        {
            InitializeComponent();

            chart1.ChartAreas[0].AxisX.Name = "Time";
            chart1.ChartAreas[0].AxisX.Minimum = minm;
            chart1.ChartAreas[0].AxisX.Maximum = maksm;
            chart1.ChartAreas[0].AxisX.Name = "Temperature";
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 300;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(minm, maksm);
            this.chart1.Series[0].Points.AddXY((minm + maksm) / 2, 0);

            chart2.ChartAreas[0].AxisX.Name = "Time";
            chart2.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.Maximum = 10;
            chart2.ChartAreas[0].AxisX.Name = "Temperature";
            chart2.ChartAreas[0].AxisY.Minimum = 0;
            chart2.ChartAreas[0].AxisY.Maximum = 300;
            chart2.ChartAreas[0].AxisX.ScaleView.Zoom(0, 10);
            this.chart2.Series[0].Points.AddXY(0, 0);

            txtHedef1.Text = "80";
            txtHedef2.Text = "100";
            txtHedef3.Text = "150";
            txtHedef4.Text = "200";

            string[] ports = SerialPort.GetPortNames();  //Seri portları diziye ekleme
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
                comboBox1.Text = port;
            }

            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            sonuc = serialPort1.ReadLine();                      //Veriyi al
            this.Invoke(new EventHandler(displayData_event));
        }

        private void displayData_event(object sender, EventArgs e)
        {            
            sonuc = serialPort1.ReadLine();

            adiKontrolcuPlus(Convert.ToDouble(sonuc.Substring(0, 3)));
            serialPort1.DiscardInBuffer();
            if (sonuc != null && sonuc != "nan\r")
            {
                veriLabel.Text = "Data: " + sonuc + ""; //Labele yazdırıyoruz.     
                this.chart1.Series[0].Points.AddXY((minm + maksm) / 2, sonuc);
                maksm++;
                minm++;
            }
        }

        private void programBaslat_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;  //ComboBox1'de seçili nesneyi port ismine ata
                serialPort1.BaudRate = 9600;            //BaudRate 9600 olarak ayarla
                serialPort1.Open();                     //Seri portu aç
                programDurdur.Enabled = true;                  //Durdurma butonunu aktif hale getir
                programBaslat.Enabled = false;                 //Başlatma butonunu pasif hale getir
                //timer1.Start();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hata");    //Hata mesajı göster
            }
        }

        private void programDurdur_Click(object sender, EventArgs e)
        {
            serialPort1.Write("0");
            serialPort1.Close();        //Seri Portu kapa            
            programDurdur.Enabled = false;     //Durdurma butonunu pasif hale getir
            programBaslat.Enabled = true;
            //timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();
        }

        private void adiKontrolcu(double sicaklik)
        {
            
            //serialPort1.Close();
            //Thread.Sleep(500);
            //serialPort1.Open();
            if (sicaklik < 100)
                serialPort1.Write("1");
            else
                serialPort1.Write("0");

            //serialPort1.Close();
            //Thread.Sleep(500);
            //serialPort1.Open();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                foreach (var series in chart2.Series)
                {
                    series.Points.Clear();
                }

                this.chart2.Series[0].Points.AddXY(0, 0);
                this.chart2.Series[0].Points.AddXY(2, Convert.ToInt32(txtHedef1.Text));
                this.chart2.Series[0].Points.AddXY(4, Convert.ToInt32(txtHedef2.Text));
                this.chart2.Series[0].Points.AddXY(6, Convert.ToInt32(txtHedef3.Text));
                this.chart2.Series[0].Points.AddXY(8, Convert.ToInt32(txtHedef4.Text));
            }
            else if(radioButton2.Checked == true)
            {
                chartCount = 0;

                foreach (var series in chart2.Series)
                {
                    series.Points.Clear();
                }

                this.chart2.Series[0].Points.AddXY(0, 0);
                this.chart2.Series[0].Points.AddXY(2, chartPoints[0,1]);
                this.chart2.Series[0].Points.AddXY(4, chartPoints[1,1]);
                this.chart2.Series[0].Points.AddXY(6, chartPoints[2,1]);
                this.chart2.Series[0].Points.AddXY(8, chartPoints[3,1]);

                Array.Clear(chartPoints, 0, chartPoints.Length);
            }
        }

        private void adiKontrolcuPlus(double sicaklik)
        {
            if (radioButton1.Checked == true)
            {
                if (Convert.ToInt32(txtHedef4.Text) < 200)
                {
                    serialPort1.Write("1");

                    if (sicaklik == Convert.ToInt32(txtHedef2.Text))
                        manuelStop();
                    else if (sicaklik == Convert.ToInt32(txtHedef3.Text))
                        manuelStop();
                }
                else
                {
                    serialPort1.Write("0");
                    Thread.Sleep(5000);
                }
            }
            else if(radioButton2.Checked == true)
            {
                if (sicaklik < chartPoints[3,1])
                {
                    serialPort1.Write("1");

                    if (sicaklik == chartPoints[1, 1])
                        manuelStop();
                    else if (sicaklik == chartPoints[2, 1])
                        manuelStop();
                }
                else
                {
                    serialPort1.Write("0");
                    Thread.Sleep(5000);
                }
            }
        }        

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = false;            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Checked = false;            
        }

        private void chart2_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                if (chartCount == 0)
                {
                    chartPoints[0, 0] = this.PointToClient(Cursor.Position).X * 10 / 700 - 1;
                    chartPoints[0, 1] = 867 - this.PointToClient(Cursor.Position).Y;
                    chartCount++;
                }
                else if (chartCount == 1)
                {
                    chartPoints[1, 0] = this.PointToClient(Cursor.Position).X * 10 / 700 - 1;
                    chartPoints[1, 1] = 867 - this.PointToClient(Cursor.Position).Y;
                    chartCount++;
                }
                else if (chartCount == 2)
                {
                    chartPoints[2, 0] = this.PointToClient(Cursor.Position).X * 10 / 700 - 1;
                    chartPoints[2, 1] = 867 - this.PointToClient(Cursor.Position).Y;
                    chartCount++;
                }
                else if (chartCount == 3)
                {
                    chartPoints[3, 0] = this.PointToClient(Cursor.Position).X * 10 / 700 - 1;
                    chartPoints[3, 1] = 867 - this.PointToClient(Cursor.Position).Y;
                    chartCount++;
                }
                else
                    MessageBox.Show("Daha fazla nokta belirleyemezsiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void manuelStop()
        {
            serialPort1.Write("0");
            Thread.Sleep(5000);
        }
    }
}

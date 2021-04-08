using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;

namespace ScrewDriverSetting
{
    public partial class Form1 : Form
    {
        public Point mouseLocation;
        bool EnableState;

        int tbBoxState = 0;
        string waitForSend = "";
        string MotorStatus;
        string dataIn = "";
        string strRev;
        string strTime;
        string strAmpEnd;
        string strMotorStatus;
        string strError;
        string OutRange1;
        string OutRange2;
        string OutRange3;
        string OutRange4;

        #region Main Function
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            cbBoxBaudrate.Text = "9600";
            cbBoxDataBits.Text = "8";
            cbBoxStopBits.Text = "One";
            cbBoxParityBits.Text = "None";

            chkBoxAdvancedSetting.Checked = false;
            cbBoxDataBits.Enabled = false;
            cbBoxStopBits.Enabled = false;
            cbBoxParityBits.Enabled = false;

            btnOpen.Enabled = true;
            btnClose.Enabled = false;
            lbComport.Text = "";
            lbCommunity.Text = "Disconnected";
            pBarStatus.Value = 0;

            EnableState = false;
            HideEnable();
            btnRun.Enabled = false;
            btnStop.Enabled = false;

            MotorStatus = "Stop";

            lbMotorStatus.Text = "Stop";

            trBarError.Enabled = false;
            tboxError.Enabled = false;

            trBarError.Value = 1;
            tboxError.Text = "1";
        }

        private void HideEnable()
        {
            if(EnableState == false)
            {
                cbBoxComPort.Enabled = true;
                cbBoxBaudrate.Enabled = true;
                chkBoxAdvancedSetting.Enabled = true;

                if (chkBoxAdvancedSetting.Checked)
                {
                    cbBoxDataBits.Enabled = true;
                    cbBoxStopBits.Enabled = true;
                    cbBoxParityBits.Enabled = true;
                }

                tBoxTxRev.Enabled = false;
                tBoxTxTime.Enabled = false;
                btnTxApply.Enabled = false;

                btn_1.Enabled = false;
                btn_2.Enabled = false;
                btn_3.Enabled = false;
                btn_4.Enabled = false;
                btn_5.Enabled = false;
                btn_6.Enabled = false;
                btn_7.Enabled = false;
                btn_8.Enabled = false;
                btn_9.Enabled = false;
                btn_Dot.Enabled = false;
                btn_0.Enabled = false;
                btn_Clear.Enabled = false;

                trBarError.Enabled = false;
                tboxError.Enabled = false;
            }
            else
            {
                cbBoxComPort.Enabled = false;
                cbBoxBaudrate.Enabled = false;
                cbBoxDataBits.Enabled = false;
                cbBoxStopBits.Enabled = false;
                cbBoxParityBits.Enabled = false;
                chkBoxAdvancedSetting.Enabled = false;

                tBoxTxRev.Enabled = true;
                tBoxTxTime.Enabled = true;
                btnTxApply.Enabled = true;

                btn_1.Enabled = true;
                btn_2.Enabled = true;
                btn_3.Enabled = true;
                btn_4.Enabled = true;
                btn_5.Enabled = true;
                btn_6.Enabled = true;
                btn_7.Enabled = true;
                btn_8.Enabled = true;
                btn_9.Enabled = true;
                btn_Dot.Enabled = true;
                btn_0.Enabled = true;
                btn_Clear.Enabled = true;

                trBarError.Enabled = true;
                tboxError.Enabled = true;
            }
        }

        private void cbBoxComPort_DropDown(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            cbBoxComPort.Items.Clear();
            cbBoxComPort.Items.AddRange(ports);
        }

        private void chkBoxAdvancedSetting_CheckedChanged(object sender, EventArgs e)
        {
            if(chkBoxAdvancedSetting.Checked)
            {
                cbBoxDataBits.Enabled = true;
                cbBoxStopBits.Enabled = true;
                cbBoxParityBits.Enabled = true;
            }
            else
            {
                cbBoxDataBits.Enabled = false;
                cbBoxStopBits.Enabled = false;
                cbBoxParityBits.Enabled = false;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = cbBoxComPort.Text;
                serialPort1.BaudRate = Convert.ToInt32(cbBoxBaudrate.Text);
                serialPort1.DataBits = Convert.ToInt32(cbBoxDataBits.Text);
                serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cbBoxStopBits.Text);
                serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), cbBoxParityBits.Text);

                serialPort1.Open();
                lbComport.Text = cbBoxComPort.Text;
                lbCommunity.Text = "Connected";
                pBarStatus.Value = 100;

                btnClose.Enabled = true;
                btnOpen.Enabled = false;

                EnableState = true;
                HideEnable();
                btnRun.Enabled = true;
                btnStop.Enabled = false;

                serialPort1.Write("-/-/-/Stop/-;");

            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if(serialPort1.IsOpen)
            {
                serialPort1.Close();

                lbComport.Text = "";
                lbCommunity.Text = "Disconnected";
                pBarStatus.Value = 0;

                btnOpen.Enabled = true;
                btnClose.Enabled = false;

                EnableState = false;
                HideEnable();
                btnRun.Enabled = false;
                btnStop.Enabled = false;

                tBoxTxRev.Text = "";
                tBoxTxTime.Text = "";
                tBoxTxAmpEnd.Text = "";

                tBoxRxRev.Text = "";
                tBoxRxTime.Text = "";
                tBoxRxAmpEnd.Text = "";

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        #endregion

        #region TxData

        #region tbBoxState

        private void tBoxTxRev_Click(object sender, EventArgs e)
        {
            tbBoxState = 1;
        }

        private void tBoxTxTime_Click(object sender, EventArgs e)
        {
            tbBoxState = 2;
        }

        private void tBoxTxAmpEnd_Click(object sender, EventArgs e)
        {
            tbBoxState = 3;
        }

        #endregion

        #region NumPad
        private void btn_1_Click(object sender, EventArgs e)
        {
            if (tbBoxState == 1)
            {
                tBoxTxRev.Text += "1";
            }
            else if(tbBoxState == 2)
            {
                tBoxTxTime.Text += "1";
            }
            else if (tbBoxState == 3)
            {
                tBoxTxAmpEnd.Text += "1";
            }
        }

        private void btn_2_Click(object sender, EventArgs e)
        {
            if (tbBoxState == 1)
            {
                tBoxTxRev.Text += "2";
            }
            else if (tbBoxState == 2)
            {
                tBoxTxTime.Text += "2";
            }
            else if (tbBoxState == 3)
            {
                tBoxTxAmpEnd.Text += "2";
            }
        }

        private void btn_3_Click(object sender, EventArgs e)
        {
            if (tbBoxState == 1)
            {
                tBoxTxRev.Text += "3";
            }
            else if (tbBoxState == 2)
            {
                tBoxTxTime.Text += "3";
            }
            else if (tbBoxState == 3)
            {
                tBoxTxAmpEnd.Text += "3";
            }
        }

        private void btn_4_Click(object sender, EventArgs e)
        {
            if (tbBoxState == 1)
            {
                tBoxTxRev.Text += "4";
            }
            else if (tbBoxState == 2)
            {
                tBoxTxTime.Text += "4";
            }
            else if (tbBoxState == 3)
            {
                tBoxTxAmpEnd.Text += "4";
            }
        }

        private void btn_5_Click(object sender, EventArgs e)
        {
            if (tbBoxState == 1)
            {
                tBoxTxRev.Text += "5";
            }
            else if (tbBoxState == 2)
            {
                tBoxTxTime.Text += "5";
            }
            else if (tbBoxState == 3)
            {
                tBoxTxAmpEnd.Text += "5";
            }
        }

        private void btn_6_Click(object sender, EventArgs e)
        {
            if (tbBoxState == 1)
            {
                tBoxTxRev.Text += "6";
            }
            else if (tbBoxState == 2)
            {
                tBoxTxTime.Text += "6";
            }
            else if (tbBoxState == 3)
            {
                tBoxTxAmpEnd.Text += "6";
            }
        }

        private void btn_7_Click(object sender, EventArgs e)
        {
            if (tbBoxState == 1)
            {
                tBoxTxRev.Text += "7";
            }
            else if (tbBoxState == 2)
            {
                tBoxTxTime.Text += "7";
            }
            else if (tbBoxState == 3)
            {
                tBoxTxAmpEnd.Text += "7";
            }
        }

        private void btn_8_Click(object sender, EventArgs e)
        {
            if (tbBoxState == 1)
            {
                tBoxTxRev.Text += "8";
            }
            else if (tbBoxState == 2)
            {
                tBoxTxTime.Text += "8";
            }
            else if (tbBoxState == 3)
            {
                tBoxTxAmpEnd.Text += "8";
            }
        }

        private void btn_9_Click(object sender, EventArgs e)
        {
            if (tbBoxState == 1)
            {
                tBoxTxRev.Text += "9";
            }
            else if (tbBoxState == 2)
            {
                tBoxTxTime.Text += "9";
            }
            else if (tbBoxState == 3)
            {
                tBoxTxAmpEnd.Text += "9";
            }
        }

        private void btn_0_Click(object sender, EventArgs e)
        {
            if (tbBoxState == 1)
            {
                tBoxTxRev.Text += "0";
            }
            else if (tbBoxState == 2)
            {
                tBoxTxTime.Text += "0";
            }
            else if (tbBoxState == 3)
            {
                tBoxTxAmpEnd.Text += "0";
            }
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            if (tbBoxState == 1)
            {
                tBoxTxRev.Text = "";
            }
            else if (tbBoxState == 2)
            {
                tBoxTxTime.Text = "";
            }
            else if (tbBoxState == 3)
            {
                tBoxTxAmpEnd.Text = "";
            }
        }


        #endregion

        #region tBoxTx KeyPress
        private void tBoxTxRev_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            if (!char.IsDigit(c) && c != 8)
            {
                e.Handled = true;
            }
            
            if(c == 13)
            {
                if (tBoxTxRev.Text == "")
                {
                    tBoxTxRev.Text = "-";
                }
                if (tBoxTxTime.Text == "")
                {
                    tBoxTxTime.Text = "-";
                }
                if (tBoxTxAmpEnd.Text == "")
                {
                    tBoxTxAmpEnd.Text = "-";
                }
                if (serialPort1.IsOpen)
                {
                    waitForSend = tBoxTxRev.Text + "/" + tBoxTxTime.Text + "/" + tBoxTxAmpEnd.Text + "/" + MotorStatus + "/" + tboxError.Text + ";";
                    serialPort1.Write(waitForSend);

                    tBoxTxRev.Clear(); tBoxTxTime.Clear(); tBoxTxAmpEnd.Clear();
                }
            }
        }

        private void tBoxTxTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            if (!char.IsDigit(c) && c != 8)
            {
                e.Handled = true;
            }
            if (c == 13)
            {
                if (tBoxTxRev.Text == "")
                {
                    tBoxTxRev.Text = "-";
                }
                if (tBoxTxTime.Text == "")
                {
                    tBoxTxTime.Text = "-";
                }
                if (tBoxTxAmpEnd.Text == "")
                {
                    tBoxTxAmpEnd.Text = "-";
                }
                if (serialPort1.IsOpen)
                {
                    waitForSend = tBoxTxRev.Text + "/" + tBoxTxTime.Text + "/" + tBoxTxAmpEnd.Text + "/" + MotorStatus + "/" + tboxError.Text + ";";
                    serialPort1.Write(waitForSend);

                    tBoxTxRev.Clear(); tBoxTxTime.Clear(); tBoxTxAmpEnd.Clear();
                }
            }
        }

        private void tBoxTxAmp_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            if (!char.IsDigit(c) && c != 8)
            {
                e.Handled = true;
            }

            if (c == 13)
            {
                if (tBoxTxRev.Text == "")
                {
                    tBoxTxRev.Text = "-";
                }
                if (tBoxTxTime.Text == "")
                {
                    tBoxTxTime.Text = "-";
                }
                if (tBoxTxAmpEnd.Text == "")
                {
                    tBoxTxAmpEnd.Text = "-";
                }
                if (serialPort1.IsOpen)
                {
                    waitForSend = tBoxTxRev.Text + "/" + tBoxTxTime.Text + "/" + tBoxTxAmpEnd.Text + "/" + MotorStatus + "/" + tboxError.Text + ";";
                    serialPort1.Write(waitForSend);

                    tBoxTxRev.Clear(); tBoxTxTime.Clear(); tBoxTxAmpEnd.Clear();
                }
            }
        }

        private void tBoxTxAmpEnd_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            if (!char.IsDigit(c) && c != 8)
            {
                e.Handled = true;
            }

            if (c == 13)
            {
                if (tBoxTxRev.Text == "")
                {
                    tBoxTxRev.Text = "-";
                }
                if (tBoxTxTime.Text == "")
                {
                    tBoxTxTime.Text = "-";
                }
                if (tBoxTxAmpEnd.Text == "")
                {
                    tBoxTxAmpEnd.Text = "-";
                }
                if (serialPort1.IsOpen)
                {
                    waitForSend = tBoxTxRev.Text + "/" + tBoxTxTime.Text + "/" + tBoxTxAmpEnd.Text + "/" + MotorStatus + "/" + tboxError.Text + ";";
                    serialPort1.Write(waitForSend);

                    tBoxTxRev.Clear(); tBoxTxTime.Clear(); tBoxTxAmpEnd.Clear();
                }
            }
        }

        #endregion

        private void btnTxApply_Click(object sender, EventArgs e)
        {
            if(tBoxTxRev.Text == "")
            {
                tBoxTxRev.Text = "-";
            }
            if(tBoxTxTime.Text == "")
            {
                tBoxTxTime.Text = "-";
            }
            if (tBoxTxAmpEnd.Text == "")
            {
                tBoxTxAmpEnd.Text = "-";
            }
            if (serialPort1.IsOpen)
            {
                waitForSend = tBoxTxRev.Text + "/" + tBoxTxTime.Text + "/" + tBoxTxAmpEnd.Text + "/" + MotorStatus + "/" + tboxError.Text + ";";
                serialPort1.Write(waitForSend);

                tBoxTxRev.Clear(); tBoxTxTime.Clear(); tBoxTxAmpEnd.Clear();
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = true;
            btnRun.Enabled = false;
            MotorStatus = "Run";

            if (serialPort1.IsOpen)
            {
                waitForSend = "-/-/-/" + MotorStatus + "/-;";
                serialPort1.Write(waitForSend);

                tBoxTxRev.Clear(); tBoxTxTime.Clear(); tBoxTxAmpEnd.Clear();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            btnRun.Enabled = true;
            MotorStatus = "Stop";

            if (serialPort1.IsOpen)
            {
                waitForSend = "-/-/-/" + MotorStatus + "/-;";
                serialPort1.Write(waitForSend);

                tBoxTxRev.Clear(); tBoxTxTime.Clear(); tBoxTxAmpEnd.Clear();
            }
        }

        #endregion

        #region RxData
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                dataIn += serialPort1.ReadExisting();
                RxRevDataCutString(dataIn);

                this.Invoke(new EventHandler(ShowData));
                dataIn = null;
            }
            catch
            {

            }
        }

        private void ShowData(object sender, EventArgs e)
        {
            if (strRev != "-")
            {
                tBoxRxRev.Text = strRev;
            }
            if (strTime != "-")
            {
                tBoxRxTime.Text = strTime;
            }
            if (strAmpEnd != "-")
            {
                tBoxRxAmpEnd.Text = strAmpEnd;
            }
            if (strMotorStatus != "-")
            {
                lbMotorStatus.Text = strMotorStatus;
            }
            if (strMotorStatus != "-")
            {
                lbRealMotorStatus.Text = strMotorStatus;
            }
            if (strError != "-")
            {
                lbError.Text = strError;
            }
        }

        private string RxRevDataCutString(string dataWaitProcess)
        {
            int Cut;

            Cut = dataWaitProcess.IndexOf("/");
            strRev = dataWaitProcess.Substring(0, Cut);

            OutRange1 = dataWaitProcess.Substring(Cut + 1);
            RxTimeDataCutString(OutRange1);

            return strRev;
        }

        private string RxTimeDataCutString(string dataWaitProcess)
        {
            int Cut;

            Cut = dataWaitProcess.IndexOf("/");
            strTime = dataWaitProcess.Substring(0, Cut);

            OutRange2 = dataWaitProcess.Substring(Cut + 1);
            RxAmpEndDataCutString(OutRange2);

            return strTime;
        }

        private string RxAmpEndDataCutString(string dataWaitProcess)
        {
            int Cut;

            Cut = dataWaitProcess.IndexOf("/");
            strAmpEnd = dataWaitProcess.Substring(0, Cut);

            OutRange3 = dataWaitProcess.Substring(Cut + 1);
            RxMotorStatusDataCutString(OutRange3);

            return strAmpEnd;
        }

        private string RxMotorStatusDataCutString(string dataWaitProcess)
        {
            int Cut;

            Cut = dataWaitProcess.IndexOf("/");
            strMotorStatus = dataWaitProcess.Substring(0, Cut);

            OutRange4 = dataWaitProcess.Substring(Cut + 1);
            RxErrorDataCutString(OutRange4);

            return strMotorStatus;
        }

        private string RxErrorDataCutString(string dataWaitProcess)
        {
            int Cut;

            Cut = dataWaitProcess.IndexOf(";");
            strError = dataWaitProcess.Substring(0, Cut);

            return strError;
        }

        #endregion

        #region Custom WindowsForm
        private void btnFormClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnFormMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLocation = new Point(-e.X, -e.Y);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePose = Control.MousePosition;
                mousePose.Offset(mouseLocation.X, mouseLocation.Y);
                Location = mousePose;
            }
        }


        #endregion

        #region Save As
        private void btn_SaveAs_Click(object sender, EventArgs e)
        {
            string NowTime;
            string NowDate;
            string DataSaveAll;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(saveFileDialog1.FileName.ToString());
                NowDate = Convert.ToString(DateTime.Now.Day) + "/" + Convert.ToString(DateTime.Now.Month) + "/" + Convert.ToString(DateTime.Now.Year);
                NowTime = Convert.ToString(DateTime.Now.Hour) + ":" + Convert.ToString(DateTime.Now.Minute) + ":" + Convert.ToString(DateTime.Now.Second);
                DataSaveAll = "DateSetting :  "+ NowDate + "  " + NowTime + "\nRevSet :  " + tBoxRxRev.Text + "\nTimeSet :  " + tBoxRxTime.Text + "\nAmpEndSet  " + tBoxRxAmpEnd.Text;
                file.WriteLine(DataSaveAll);
                file.Close();
            }
        }

        #endregion

        private void trBarError_Scroll(object sender, EventArgs e)
        {
            tboxError.Text = Convert.ToString(trBarError.Value);
        }
    }
}
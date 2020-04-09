using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketReuseAddress
{
    public partial class FormSocketReuseAddress : Form
    {
        bool run = false;
        public FormSocketReuseAddress()
        {
            InitializeComponent();
        }

        private void FormSocketReuseAddress_Load(object sender, EventArgs e)
        {
            buttonStop.Enabled = false;
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            buttonRun.Enabled = false;
            buttonStop.Enabled = true;
            run = true;
            Run();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonStop.Enabled = false;
            buttonRun.Enabled = true;
            run = false;
        }

        public void Run()
        {
            int delay = int.Parse(comboBoxDelay.Text);
            int port = int.Parse(comboBoxPort.Text);
            new Thread(new ParameterizedThreadStart(delegate (object threadObject)
            {
                Thread.CurrentThread.IsBackground = true;
                while (run)
                {
                    if (!this.Disposing && !this.IsDisposed)
                    {
                        IPEndPoint localpt = new IPEndPoint(Dns.Resolve(Dns.GetHostName()).AddressList[0], port);
                        localpt = new IPEndPoint(IPAddress.Any, port);

                        //if the following lines up until serverConnect(); are removed all packets are received correctly
                        UdpClient UdpClient = new UdpClient();
                        TcpClient TcpClient = new TcpClient();
                        TcpListener TcpListener = new TcpListener(localpt)
                        {

                        };
                        try
                        {
                            UdpClient.ExclusiveAddressUse = false;
                            UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                            UdpClient.Client.Bind(localpt);

                            TcpClient.ExclusiveAddressUse = false;
                            TcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                            TcpClient.Client.Bind(localpt);

                            TcpListener.Server.ExclusiveAddressUse = false;
                            TcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                            TcpListener.Start();

                            new Thread(new ParameterizedThreadStart(delegate (object threadObject1)
                            {
                                Thread.CurrentThread.IsBackground = true;
                                if (!this.Disposing && !this.IsDisposed)
                                {
                                    this.BeginInvoke(new MethodInvoker(delegate
                                    {
                                     if(checkBox1.Checked)   this.listBox1.Items.Add(port.ToString() + "Success");
                                    }));
                                }
                            })).Start(null);
                        }
                        catch (Exception catchException)
                        {
                            new Thread(new ParameterizedThreadStart(delegate (object threadObject2)
                            {
                                Thread.CurrentThread.IsBackground = true;
                                if (!this.Disposing && !this.IsDisposed)
                                {
                                    this.BeginInvoke(new MethodInvoker(delegate
                                    {
                                        this.listBox1.Items.Add(port.ToString() + "-Failed:" + catchException.ToString());
                                    }));
                                }
                            })).Start(null);
                        }
                        finally
                        {
                            if (UdpClient != null) UdpClient.Dispose();
                            if (TcpClient != null) TcpClient.Dispose();
                            if (TcpListener != null) TcpListener.Stop();
                            if (TcpListener != null && TcpListener.Server != null) TcpListener.Server.Dispose();
                        }
                        port++;
                        new Thread(new ParameterizedThreadStart(delegate (object threadObject3)
                       {
                           Thread.CurrentThread.IsBackground = true;
                           if (!this.Disposing && !this.IsDisposed)
                           {
                               this.BeginInvoke(new MethodInvoker(delegate
                               {
                                   comboBoxPort.Text = port.ToString();
                               }));
                           }
                       })).Start(null);
                        Thread.Sleep(delay);
                    }
                }
            })).Start(null);
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
        }
    }
}

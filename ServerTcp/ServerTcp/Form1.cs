using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerTcp
{
    public partial class Form1 : Form
    {
        Thread thread;
        String exitString = "EXIT";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStartServerTCP_Click(object sender, EventArgs e)
        {
            // ��������� ������ ������ �� ��������� ���������� � ��� ����
            // �������� �������� ���������� ������ (���� ������ ���� - ����������� �����)
            if (thread != null)
            {
                return;
            }

            // ��������� ������ �������� ������ �� ������ � ����� �������� ����� (� ����� ServerFunc)
            thread = new Thread(ServerFunc);
            thread.IsBackground = true;
            thread.Start();

            Text = "ServerTCP was started !";
            tbServerTcpStatistics.Text = $"ServerTCP was started at {DateTime.Now.ToString()}{Environment.NewLine}";
        }

        private void ServerFunc(object? obj)
        {
            // ��������� ��������� ������ ����� ��'��� TcpListener,
            // ���� � �������� �������� ����� �� �������������� ���������� �볺���
            // - ������ 
            // IP-������ �������
            // ���������� ���� �������
            TcpListener listener = new TcpListener(IPAddress.Parse("192.168.56.1"), 11000);

            try
            {
                // ������ ������ �� �������� �� ������� ��������� ���������� �볺���
                listener.Start(10);

                // ���� ��������������� ���������� �볺���
                do
                {
                    // �������� - ���� � �볺���, �� ������� ���������� (����� Pending()) -
                    // ��� �� ���������� ����������� �������� ����� �볺���
                    if (listener.Pending())
                    {
                        // ��������� ������ ��� ���������� ��������� ������ �볺���
                        // - ���������� ���������� ����� ��'��� listener ��
                        // ���������� ��������� �� ���������� �볺��� (AcceptTcpClient())
                        // �� ������ ���� � �볺���� ����� ���������
                        TcpClient client = listener.AcceptTcpClient();

                        // "����������" ������ � �볺����
                        // ��������� ������ ��� ���������/�������� �����
                        byte[] buffer = new byte[1024];

                        // -------------------------------- ��������� ����� �� �볺��� ------------------------------
                        // ��������� ����� ����� ������ � �볺��� ������ GetStream() �� �������� ���������� � ����� ���� NetworkStream (�������������)
                        NetworkStream ns = client.GetStream();

                        // ���������� ����� � ���������� ������ - ������� ���������� ���� ���������� � ����� ���� int
                        int len = ns.Read(buffer, 0, buffer.Length);

                        // ��������� �� ���������� ���������� ���������� ���������� �� ���������� �����������
                        StringBuilder sb = new StringBuilder();
                        //sb.Append($"{len} was recived from {client.Client.RemoteEndPoint} {Environment.NewLine}");
                        //sb.AppendLine($"{len} was recived from {client.Client.RemoteEndPoint} at {DateTime.Now.ToLongTimeString()}"); // ��������� ������������ �����
                        string messageFromClient = (Encoding.Default.GetString(buffer, 0, len));
                        sb.AppendLine(messageFromClient);
                        
                        // �������� ����� ����������� �볺���
                        if(messageFromClient.Equals(exitString))
                        {
                            listener.Stop();
                            tbServerTcpStatistics.BeginInvoke(new Action<string>(AddText), "The client sent a server stop command !");
                            
                            // ��������� / �������� ������
                            client.Client.Shutdown(SocketShutdown.Receive);
                            client.Close();
                            return;
                        }

                        // �������� �����, ��������� �� �볺��� � �������� ������ �� ���������� ���������� � ��������� ������
                        // ��������������� ������� Action<>
                        // (����, ��������� �����, ������ ����� (����������) � ����� ������� ����� �������� ��� �� ���������� ����������)
                        tbServerTcpStatistics.BeginInvoke(new Action<string>(AddText), sb.ToString());

                        // ��������� / �������� ������
                        client.Client.Shutdown(SocketShutdown.Receive);
                        client.Close();
                    }

                } while (true);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // �������� �'������� - ����� ��������� �� ��������� ������ listener
                listener.Stop();
            }
        }

        private void AddText(string str)
        {
            StringBuilder sb = new StringBuilder(tbServerTcpStatistics.Text);
            sb.Append(str);
            tbServerTcpStatistics.Text = sb.ToString();
        }
    }
}
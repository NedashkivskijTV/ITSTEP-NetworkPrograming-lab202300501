using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientTcp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //private void btnSendTCPText_Click(object sender, EventArgs e)
        private async void btnSendTCPText_Click(object sender, EventArgs e)
        {
            // ��������� �볺��� - ������� �������� �����
            TcpClient tcpClient = new TcpClient();

            try
            {
                // ��������� ������ ������ ��� ���������� �� ���������� �������
                IPAddress remouteAddress = IPAddress.Parse("192.168.56.1");

                // ϳ��������� �볺��� �� �������
                // - ������ IP-������ ������� �� ���� �������
                //tcpClient.Connect(remouteAddress, 11000); // ����������� �����
                await tcpClient.ConnectAsync(remouteAddress, 11000); // ����� async/await (Task)

                // ��������� ������ NetworkStream ��� �������� ����� - ����� ���������� �� �볺���
                NetworkStream ns = tcpClient.GetStream();
                // ����� �������� ����� � ����
                //ns.Write(Encoding.Default.GetBytes(tbClientsTText.Text)); // ����������� �����
                await ns.WriteAsync(Encoding.Default.GetBytes(tbClientsText.Text)); // ����� async/await (Task)
                // �������� ���������� ����������
                tbClientsText.Clear();
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // �������� ����������
                tcpClient.Close();
            }
        }

        private void AddTextToClientFromServer(string str)
        {
            tbClientsText.Text = str;
        }
    }
}
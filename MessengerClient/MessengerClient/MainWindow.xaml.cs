using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;

namespace MessengerSO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            User user = Settings.Load("settings.bin");
            if (user == null)
            {
                IPAddress IP;
                if(Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
                {
                    IP = Dns.GetHostAddresses(Dns.GetHostName())[0];
                }
                else
                {
                    IP = IPAddress.Parse("127.0.0.1");
                }
                user = new User("New User", IP);
            }

            IPEndPoint ipe = new IPEndPoint(user.IP, 8000);
            /*
            socket.Bind(ipe); //asociates the socket with a local IP direction.
            socket.Listen(10); //tells the socket that it must be capable of listening entering connections (up yo 10 pending).
            Socket newConnection = socket.Accept(); //waits for a connection to arrive, when it does, returns a socket with the stablished connection.

            socket.Connect(IPAddress.Paarse("127.0.0.1"), 8000); //the client must connect to the server.
            */
            
        }
    }
}

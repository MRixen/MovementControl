using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MovementControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Task task_robotControl;
        private Task task_initComPort, task_initDatabase;
        private GlobalDataSet globalDataSet;
        private RobotControl robotControl;
        private bool startTransfer = false;
        private Class1 class1;

        public MainPage()
        {
            this.InitializeComponent();

            // Add this to prevent encoding errors
            System.Text.EncodingProvider ppp;
            ppp = System.Text.CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(ppp);

            // Init
            globalDataSet = new GlobalDataSet();
            robotControl = new RobotControl(globalDataSet);
            //task_initComPort = new Task(initComPort_task);
            //task_initComPort.Start();
            //task_initComPort.Wait();

            //task_initDatabase = new Task(initDatabase_task);
            //task_initDatabase.Start();
            //task_initDatabase.Wait();

            //initDatabase_task();

            // Start control algorithm
            //task_robotControl = new Task(robotControl_task);
            //task_robotControl.Start();

            class1 = new Class1();
        }

        private void initDatabase_task()
        {

            string connectionString = "Server=192.168.0.66;Database=moveForward;Uid=root;Pwd=rbc;SslMode=None;";
            using (MySqlConnection dbConn = new MySqlConnection(connectionString))
            {
                //MySqlCommand dbCmd = new MySqlCommand("INSERT INTO s1(x, y, z, timestamp) VALUES('" + 1111 + "', '" + 1111 + "', '" + 1111 + "', '" + 1111 + "')", dbConn); 
                MySqlCommand dbCmd = new MySqlCommand("SELECT * FROM s1", dbConn);
                MySqlDataReader dr;
                dbConn.Open();
                dr = dbCmd.ExecuteReader();

                int count = 0;

                while (dr.Read())
                {
                    count += 1;
                }

                //using (MySqlCommand dbCmd = new MySqlCommand("s0", dbConn))
                //{
                //    for (int i = 0; i < 1; i++)
                //    {
                //        dbCmd.CommandType = CommandType.StoredProcedure;
                //        dbCmd.Parameters.Add("x", MySqlDbType.Int32).Value = 1111;
                //        dbCmd.Parameters.Add("y", MySqlDbType.Int32).Value = 1111;
                //        dbCmd.Parameters.Add("z", MySqlDbType.Int32).Value = 1111;
                //        dbCmd.Parameters.Add("timestamp", MySqlDbType.Int32).Value = 1111;
                //    }

                //dbCmd.ExecuteNonQuery();
                //}
            }
        }

        private async void initComPort_task()
        {
            var deviceSelector = SerialDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(deviceSelector);
            int i = 0;
            try
            {
                foreach (var item in devices)
                {
                    // Find openCM device
                    var port = await SerialDevice.FromIdAsync(devices[i].Id);
                    if (port.UsbVendorId == 65521)
                    {
                        Debug.WriteLine("Device found");
                        Debug.WriteLine("UsbVendorId: " + port.UsbVendorId);
                        Debug.WriteLine("UsbProductId: " + port.UsbProductId);

                        // Configure port
                        port.BaudRate = 9600;
                        port.DataBits = 8;
                        port.StopBits = SerialStopBitCount.One;
                        port.Parity = SerialParity.None;
                        port.Handshake = SerialHandshake.None;
                        port.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                        port.WriteTimeout = TimeSpan.FromMilliseconds(1000);

                        globalDataSet.Port = port;

                        startTransfer = true;
                        break;
                    }
                    else i++;
                    if (!startTransfer) Debug.WriteLine("No device found");
                }
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine("!!! NullReferenceException in devices !!!");
            }
        }

        public async void robotControl_task()
        {
            await Task.Run(() => execServ_openCM());
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            initDatabase_task();
            //class1.InsertTemp(1111, 1111, 1111, 1111);
        }

        private void execServ_openCM()
        {
            int stepsize = 100; // In percentage of max stepsize
            int velocity = 100; // In percentage of max velocity
            int steps = 1;

            while (!globalDataSet.StopAllOperations)
            {
                if (startTransfer)
                {
                    // Send control data to openCM via usb

                    // Condition one is set - move forward
                    if (true)
                    {
                        robotControl.moveForward(stepsize, velocity, steps);
                        //robotControl.testFunction(66);
                        //toggleMode = false;
                    }

                    // Condition two is set - move backward
                    if (true) ;

                }
                Task.Delay(-1).Wait(1000);
            }
        }
    }
}

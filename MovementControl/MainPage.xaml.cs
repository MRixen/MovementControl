using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private LocalDbManagement localDbManagement;

        public MainPage()
        {
            this.InitializeComponent();

            // Add this to prevent encoding errors
            System.Text.EncodingProvider ppp;
            ppp = System.Text.CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(ppp);

            // Init
            globalDataSet = new GlobalDataSet();
            localDbManagement = new LocalDbManagement(globalDataSet);
            robotControl = new RobotControl(globalDataSet, localDbManagement);

            task_initDatabase = new Task(initDatabase_task);
            task_initDatabase.Start();
            task_initDatabase.Wait();


            //task_initComPort = new Task(initComPort_task);
            //task_initComPort.Start();
            //task_initComPort.Wait();

            // Start control algorithm
            //task_robotControl = new Task(robotControl_task);
            //task_robotControl.Start();
        }

        private void initDatabase_task()
        {
            // Create local db
            int[] data = new int[4];
            localDbManagement.createDb("moveforward");

            // Connect to remote db
            string connectionString = "Server=192.168.0.66;Database=moveforward;Uid=root;Pwd=rbc;SslMode=None;";
            using (MySqlConnection dbConn = new MySqlConnection(connectionString))
            {
                // Read from remote db and save content to local db 
                // Todo: Read more tables -> Actual we read only s0 and s1
                for (int i = 0; i < 2; i++)
                {
                    MySqlCommand dbCmd = new MySqlCommand("SELECT * FROM s" + i, dbConn);
                    dbConn.Open();

                    using (MySqlDataReader reader = dbCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (int j = 0; j < 4; j++) data[i] = reader.GetInt32(j);
                            if (i == 0) localDbManagement.insertToTable_s0(data, "moveforward");
                            if (i == 1) localDbManagement.insertToTable_s1(data, "moveforward");
                        }
                    }
                    dbConn.Close();
                }
                // Set z data of tables to list, so we can use it inside movement control algorithm
                localDbManagement.setDataLists_z("moveforward");
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

        private void execServ_openCM()
        {
            int stepsize = 100; // In percentage of max stepsize
            int velocity = 300; // In percentage of max velocity
            int steps = 1;

            while (!globalDataSet.StopAllOperations)
            {
                // Send control data to openCM via usb
                if (startTransfer)
                {
                    // Condition one is set - move forward
                    if (true)
                    {
                        robotControl.moveForward(stepsize, velocity, steps);
                    }

                    // Condition two is set - move backward
                    if (true) ;

                }
                Task.Delay(-1).Wait(1000);
            }
        }
    }
}

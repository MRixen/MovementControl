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
        private Task task_robotControl_write, task_robotControl_read;
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

            task_initComPort = new Task(initComPort_task);
            task_initComPort.Start();
            task_initComPort.Wait();

            // Start control algorithm
            task_robotControl_write = new Task(robotControl_task_write);
            task_robotControl_write.Start();

            task_robotControl_read = new Task(robotControl_task_read);
            task_robotControl_read.Start();

        }

        private void initDatabase_task()
        {
            // Create local db
            int[] data = new int[4];
            localDbManagement.deleteTable_s0("moveforward");
            localDbManagement.deleteTable_s1("moveforward");
            //localDbManagement.createDb("moveforward");

            // Connect to remote db and read from remote db and save content to local db 
            string connectionString = "Server=192.168.0.66;Database=moveforward;Uid=root;Pwd=rbc;SslMode=None;";
            using (MySqlConnection dbConn = new MySqlConnection(connectionString))
            {
                // Todo: Read more tables -> Actual we read only s0 and s1
                // Todo: Get db and table count at first

                // Read from specific db
                //for (int dbCounter  = 0; dbCounter < globalDataSet.DbNameList.Length; dbCounter++)
                for (int dbCounter  = 0; dbCounter < 1; dbCounter++)
                {
                    // Read tables from specific db
                    for (int rowCounter = 0; rowCounter < 2; rowCounter++)
                    {
                        MySqlCommand dbCmd = new MySqlCommand("SELECT * FROM s" + rowCounter, dbConn);
                        dbConn.Open();
               
                        using (MySqlDataReader reader = dbCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int j = 0; j < 4; j++) data[j] = reader.GetInt32(j);
                                localDbManagement.insertToTable(data, globalDataSet.DbNameList[dbCounter], rowCounter);
                            }
                        }
                        dbConn.Close();
                    }

                }
                // Set z data of table to list, so we can use it inside movement control algorithm   
                //for (int i = 0; i < globalDataSet.DbNameList.Length; i++) localDbManagement.setDataLists_z(globalDataSet.DbNameList[i]);
                for (int i = 0; i < 1; i++) localDbManagement.setDataLists_z(globalDataSet.DbNameList[i]);
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
                        port.BaudRate = 115200;
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

        public async void robotControl_task_write()
        {
            await Task.Run(() => execServ_openCM_write());
        }

        public async void robotControl_task_read()
        {
            await Task.Run(() => execServ_openCM_read());
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = "Hello WOrld";
        }

        private void button_Click_Clear(object sender, RoutedEventArgs e)
        {
            textBox.Text = "";
        }

        private void execServ_openCM_write()
        {
            int stepsize = 100; // In percentage of max stepsize
            int velocity = 400; // In percentage of max velocity
            int steps = 1;

            while (!globalDataSet.StopAllOperations)
            {
                // Send control data to openCM via usb
                if (startTransfer)
                { 
                    // Condition one is set - move forward
                    if (true)
                    {
                        //Debug.WriteLine("Execute moveforward...");
                        robotControl.moveForward(stepsize, velocity, steps);
                    }

                    // Condition two is set - move backward
                    if (true) ;

                }
                Task.Delay(-1).Wait(2000);
            }
        }

        private void execServ_openCM_read()
        {
            while (!globalDataSet.StopAllOperations)
            {
                if (startTransfer)
                {
                    //Debug.WriteLine("Execute readSomething...");
                    robotControl.readSomething();
                }
            }
        }
    }
}

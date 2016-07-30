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
        private Task task_test;
        private GlobalDataSet globalDataSet;
        private RobotControl robotControl;

        private bool toggleMode = false;

        public MainPage()
        {
            this.InitializeComponent();

            task_test = new Task(test_task);
            task_test.Start();


            //// Init
            //globalDataSet = new GlobalDataSet();
            //robotControl = new RobotControl();

            //// Connect to database


            //// Connect to openCM via usb


            //// Start control algorithm
            //task_robotControl = new Task(robotControl_task);
            //task_robotControl.Start();

            //// Set start signal after the init process
            //globalDataSet.StopAllOperations = false;
        }

        private async void test_task()
        {
            var deviceSelector = SerialDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(deviceSelector);
            int i = 0;
            foreach (var item in devices)
            {
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

                    string myString = "H";
                    byte[] array = Encoding.ASCII.GetBytes(myString);

                    var writeBuffer = array.AsBuffer();
                    for (int j = 0; i < 1000; i++)
                    {
                        if (toggleMode)
                        {
                            await port.OutputStream.WriteAsync(writeBuffer);                          
                        }
                        Task.Delay(-1).Wait(1000);
                    }                   
                    break;
                }
                i++;
            }



        
        }

        public async void robotControl_task()
        {
            await Task.Run(() => execServ_openCM());
        }

        private void execServ_openCM()
        {
            int stepsize = 100; // In percentage of max stepsize
            int velocity = 100; // In percentage of max velocity
            int steps = 10;

            while (!globalDataSet.StopAllOperations)
            {
                // Send control data to openCM via usb

                // Testroutine
                robotControl.moveForward(stepsize, velocity, steps);


            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (toggleMode) toggleMode = false;
            else toggleMode = true;
        }
    }
}

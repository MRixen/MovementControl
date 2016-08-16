using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using SQLite.Net.Attributes;
using System.Collections.ObjectModel;

namespace MovementControl
{
    class LocalDbManagement
    {
        private GlobalDataSet globalDataSet;

        public LocalDbManagement(GlobalDataSet globalDataSet)
        {
            this.globalDataSet = globalDataSet;
        }

        public void createDb(string dbName)
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName + ".sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                conn.CreateTable<s0>();
                conn.CreateTable<s1>();
            }
        }

        public void insertToTable_s0(int[] dbDataIn, string dbName)
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName + ".sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {          
                var s = conn.Insert(new s0()
                {
                    x = dbDataIn[0],
                    y = dbDataIn[1],
                    z = dbDataIn[2],
                    timestamp = dbDataIn[3]
                });
            }
        }

        public void insertToTable_s1(int[] dbDataIn, string dbName)
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName + ".sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                var s = conn.Insert(new s1()
                {
                    x = dbDataIn[0],
                    y = dbDataIn[1],
                    z = dbDataIn[2],
                    timestamp = dbDataIn[3]
                });
            }
        }

        public s0 readTableEntry_s0(int id, string tableName, string dbName)
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName + ".sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                var dbEntry = conn.Query<s0>("select * from " +tableName+ " where Id =" + id).FirstOrDefault();
                return dbEntry;
            }
        }

        public s1 readTableEntry_s1(int id, string tableName, string dbName)
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName + ".sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                var dbEntry = conn.Query<s1>("select * from " + tableName + " where Id =" + id).FirstOrDefault();
                return dbEntry;
            }
        }

        public Collection<s0> readTableAll_s0(string dbName)
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName+".sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                List<s0> myCollection = conn.Table<s0>().ToList<s0>();
                Collection<s0> DataSetList = new Collection<s0>(myCollection);
                return DataSetList;
            }
        }

        public Boolean setDataLists_z(string dbName)
        {
            var zDataListTemp = new List<int>();
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName + ".sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                // Create list with z-value from local db for all tables
                var query_s0 = conn.Table<s0>();
                var query_s1 = conn.Table<s1>();

                // Tabel s0
                foreach (var message in query_s0) zDataListTemp.Add(message.z);
                globalDataSet.ZData_s0 = zDataListTemp;

                // Clear temporary list
                zDataListTemp.Clear();

                // Tabel s1
                foreach (var message in query_s1) zDataListTemp.Add(message.z);
                globalDataSet.ZData_s1 = zDataListTemp;
            }

            return true;
        }

        public ObservableCollection<s1> readTableAll_s1(string dbName)
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName + ".sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                List<s1> myCollection = conn.Table<s1>().ToList<s1>();
                ObservableCollection<s1> DataSetList = new ObservableCollection<s1>(myCollection);
                return DataSetList;
            }
        }

        public void deleteTable_s0(string dbName)
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName + ".sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                conn.DropTable<s0>();
                conn.CreateTable<s0>();
                conn.Dispose();
                conn.Close();
            }
        }

        public void deleteTable_s1(string dbName)
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName + ".sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                conn.DropTable<s1>();
                conn.CreateTable<s1>();
                conn.Dispose();
                conn.Close();
            }
        }

        public class s0
        {
            [PrimaryKey, AutoIncrement]
            public int id { get; set; }
            public int x { get; set; }
            public int y { get; set; }
            public int z { get; set; }
            public int timestamp { get; set; }
        }

        public class s1
        {
            [PrimaryKey, AutoIncrement]
            public int id { get; set; }
            public int x { get; set; }
            public int y { get; set; }
            public int z { get; set; }
            public int timestamp { get; set; }
        }




        ////server=212.44.99.2;user id=tomassho_mps;database=tomassho_mps;persistsecurityinfo=True
        ////private const string csMySQL = "host=xxx.44.99.xxx;" +
        ////                                 "database=ts-servis_IoT;" +
        ////                                 "user id=ts-servis_IoT;" +
        ////                                 "password=xxxxjz00;" +
        ////                                 "CharSet=utf8mb4;" +
        ////                                 "persist security info=True;";
        //private const string csMySQL = "server=192.168.0.22;user id=root;database=moveforward;persistsecurityinfo=True";

        //public DispatcherTimer timer;
        ////private Timer periodicTimer;
        //int x = 25;
        //double y = 23.5;
        //int z = 1;
        //long count = 0;
        //long count2 = 0;
        //public int instruc = 0;
        ////int q = 0;
        //private const int LED_PIN = 5;
        //private GpioPin pin;
        //private GpioPinValue pinValue;
        //private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        //private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);
        //const string CalibrationFilename = "TSC2046";
        ////private Tsc2046 tsc2046;
        ////private TouchPanels.TouchProcessor processor;
        //private Point lastPosition = new Point(double.NaN, double.NaN);

        //MySqlCommand mcd;

        //private void InitGPIO()
        //{
        //    var gpio = GpioController.GetDefault();

        //    // Show an error if there is no GPIO controller
        //    if (gpio == null)
        //    {
        //        pin = null;
        //        //GpioStatus.Text = "There is no GPIO controller on this device.";
        //        return;
        //    }

        //    pin = gpio.OpenPin(LED_PIN);
        //    pinValue = GpioPinValue.High;
        //    pin.Write(pinValue);
        //    pin.SetDriveMode(GpioPinDriveMode.Output);

        //    //GpioStatus.Text = "GPIO pin initialized correctly.";

        //}
        ////  private void TimerCallback(object state)
        ////  {
        ////     z = 1;

        ////  }
        //public void Timer_Tick1(object sender, object e)
        //{
        //    if (pinValue == GpioPinValue.High)
        //    {
        //        z = 1;
        //        pinValue = GpioPinValue.Low;
        //        pin.Write(pinValue);
        //        //LED.Fill = redBrush;
        //    }
        //    else
        //    {
        //        pinValue = GpioPinValue.High;
        //        pin.Write(pinValue);
        //        // LED.Fill = grayBrush;
        //    }
        //    if (z == 1)
        //    {
        //        //timer.Start();
        //        x = x + 1;
        //        y = y + 0.42;
        //        //InsertTemp(x, y);
        //        z = 0;
        //        //TimerStatus.Text = "Writing to MySQL DB!!! ";
        //    }
        //    else
        //    {
        //        //TimerStatus.Text = "Waiting 5000ms!!! ";

        //        //HelloMessage.Text = "Instruc.nr- " + count2 + ": " + instruc / 5 + " Instructs/s";
        //        instruc = 0;
        //    }
        //    if (x > 100)
        //    {
        //        x = 10;
        //    }
        //    if (y > 100)
        //    {
        //        y = 10;
        //    }
        //    //throw new NotImplementedException();
        //    // switch (z)
        //    // {
        //    //    case 0:

        //    //        break;
        //    //    case 1:

        //    //       break; 
        //    //   default:

        //    //       break;
        //    //}

        //}

        //// private void Timer_Tick(object sender, object e)
        ////  {
        ////      z = 1;
        ////  }

        //public void InsertTemp(int x, int y, int z, int timestamp)
        //{
        //    using (MySqlConnection dbConn = new MySqlConnection(csMySQL))
        //    {
        //        //dbConn.Open();

        //        {

        //            {
        //                string q = "INSERT INTO s1(x, y, z, timestamp) VALUES('" + x + "', '" + y + "', '" + z + "', '" + timestamp + "')";
        //                ExecuteQuery(q);
        //            }

        //        }
        //    }
        //}

        //public void ExecuteQuery(string q)
        //{
        //    try
        //    {
        //        using (MySqlConnection dbConn = new MySqlConnection(csMySQL))
        //        {
        //            dbConn.Open();
        //            //mcd = new MySqlCommand("INSERT INTO s1(x, y, z, timestamp) VALUES('" + x + "', '" + y + "', '" + z + "', '" + timestamp + "')", dbConn);
        //            if (mcd.ExecuteNonQuery() == 1)
        //            {
        //                //HelloMessage.Text = "Query Executed";
        //                count2 = count2 + 1;
        //                instruc = instruc + 11;
        //                for (count = 1; count <= 60000000; count++)
        //                {
        //                    instruc = instruc + 2;
        //                    // if (pin != null)
        //                    // {
        //                    //     break; // TODO: might not be correct. Was : Exit For
        //                    // }
        //                    // HelloMessage.Text = "Pin: " + pin;
        //                    //if (formatedTime != 5)
        //                    //{
        //                    //    break; // TODO: might not be correct. Was : Exit For
        //                    //}
        //                }
        //            }
        //            else
        //            {
        //                //HelloMessage.Text = "Query Not Executed";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //HelloMessage.Text = (ex.Message);
        //    }
        //    finally
        //    {
        //        using (MySqlConnection dbConn = new MySqlConnection(csMySQL))
        //        {
        //            dbConn.Close();
        //        }
        //    }
        //}

        //private void ClickMe_Click(object sender, RoutedEventArgs e)
        //{
        //    //HelloMessage.Text = "Hello, Windows 10 IoT Core!";
        //}

        //private void GpioStatus_SelectionChanged(object sender, RoutedEventArgs e)
        //{

        //}


        ////*************************
    }

}

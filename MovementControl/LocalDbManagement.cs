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
        private List<TableList> tableList;
        private List<int> zDataListTemp;
        private float factor = (1023f / 300f);

        public LocalDbManagement(GlobalDataSet globalDataSet)
        {
            this.globalDataSet = globalDataSet;
            tableList = new List<TableList>();
            tableList.Add(new s0());
            tableList.Add(new s1());
        }

        public void createDb(string dbName)
        {
            zDataListTemp = new List<int>();
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName + ".sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                conn.CreateTable<s0>();
                conn.CreateTable<s1>();
            }
        }

        public void insertToTable(int[] dbDataIn, string dbName, int tableId)
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName + ".sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                // Populate temporary table with modified data (modified to increments for dynmixel motors)
                tableList[tableId].x = (int)Math.Round((dbDataIn[0]/100) * factor, 0);
                tableList[tableId].y = (int)Math.Round((dbDataIn[1] / 100) * factor, 0);
                tableList[tableId].z = (int)Math.Round((dbDataIn[2] / 100) * factor, 0);
                tableList[tableId].timestamp = dbDataIn[3];

                // Insert data to local db
                var s = conn.Insert(tableList[tableId]);
            }

        }

        public int readTableEntry_s0(int id, string dbName)
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName + ".sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                var dbEntry = conn.Query<s0>("select * from s0 where Id=" + id).FirstOrDefault().z;

                return dbEntry;
            }
        }

        public int readTableEntry_s1(int id, string dbName)
        {
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName + ".sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                var dbEntry = conn.Query<s1>("select * from s1 where Id =" + id).FirstOrDefault().z;
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

        public bool setDataLists_z(string dbName)
        {          
            var sqlpath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, dbName + ".sqlite");

            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), sqlpath))
            {
                // Create list with z-value from local db for all tables
                var query_s0 = conn.Table<s0>();
                var query_s1 = conn.Table<s1>();

                // Table s0
                int[] dataTemp = new int[23];
                int count = 0;
                foreach (var message in query_s0)
                {
                    dataTemp[count] = message.z;
                    count++;
                }
                globalDataSet.Moveforward.Add(new LocalList { s0 = dataTemp});
                dataTemp = new int[23];
                count = 0;
                foreach (var message in query_s1)
                {
                    dataTemp[count] = message.z;
                    count++;
                }
                globalDataSet.Moveforward.Add(new LocalList { s1 = dataTemp });
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

        public class s0 : TableList
        {
            
        }

        public class s1 : TableList
        {

        }

    }
}

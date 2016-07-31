using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace MovementControl
{
    class RobotControl
    {
        GlobalDataSet globalDataSet;
        byte[] speed = new byte[1];
        byte[] position, ids;
        byte[] byteArray = new byte[35];


        public RobotControl(GlobalDataSet globalDataSet)
        {
            this.globalDataSet = globalDataSet;
            initControlData();
        }

        public void initControlData()
        {
            // Set ids
            ids = BitConverter.GetBytes((short)500);
            byteArray[0] = ids[0];
            byteArray[1] = ids[1];

            Debug.WriteLine("ids[0]: " + ids[0]);
            Debug.WriteLine("ids[1]: " + ids[1]);
        }

        public async void moveForward(int stepsize, int velocity, int steps)
        {
            // Set speed
            speed = BitConverter.GetBytes(velocity);
            byteArray[2] = speed[0];

            // FOR TESTING
            //byte[] posTemp1 = { 5, 50, 100, 200, 0 };
            byte[] posTemp1 = { 5 };
            //byte[] posTemp2 = { 5, 50, 100, 200, 0 };
            byte[] posTemp2 = { 5 };

            int maxTableSize = posTemp1.Length;

            for (int i = 0; i < steps; i++)
            {
                for (int j = 0; i < maxTableSize; i++)
                {
                    // Get control data from database
                    // Database content:
                    //  -> moveForward
                    //      -> stepsize 
                    //      -> velocity 

                    // Set control data to buffer
                    // Max 64 byte
                    // 2 byte: ids (16 different dxls)
                    // 1 byte: speed (for every dxl the same speed)
                    // 2 byte position * 16 = 32 byte
                    // Sum: 35 byte to send for 16 dxls in one cycle

                    // Set position from database for dxl 1
                    position = BitConverter.GetBytes((short)posTemp1[i]);
                    byteArray[3] = position[0];
                    byteArray[4] = position[1];

                    // Set position from database for dxl 2
                    position = BitConverter.GetBytes((short)posTemp2[i]);
                    byteArray[5] = position[0];
                    byteArray[6] = position[1];

                    sendToPort(byteArray);
                }
            }
        }

        private async void sendToPort(byte[] byteArray)
        {
            byte[] byteArray1 = { 244 };
            var bufferArray = byteArray.AsBuffer();
            await globalDataSet.Port.OutputStream.WriteAsync(bufferArray);
        }

        public void moveBackward()
        {

        }

        public void moveRight()
        {

        }

        public void moveLeft()
        {

        }

        public async void testFunction(byte data)
        {
            byte[] byteArray = { data };
            var bufferArray = byteArray.AsBuffer();
            await globalDataSet.Port.OutputStream.WriteAsync(bufferArray);
        }
    }
}

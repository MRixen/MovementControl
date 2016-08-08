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
        byte[] position, ids,msgStart, msgEnd;
        byte[] byteArray = new byte[8];


        public RobotControl(GlobalDataSet globalDataSet)
        {
            this.globalDataSet = globalDataSet;
            initControlData();
        }

        public void initControlData()
        {
            // Set start of message
            msgStart = BitConverter.GetBytes(256);
            for (int i = 0; i < msgStart.Length; i++)
            {
                byteArray[i] = msgStart[i];
                Debug.WriteLine(msgStart[i]);
            }

            Debug.WriteLine("Converted: " + BitConverter.ToInt32(msgStart, 0));

            // Set ids
            ids = BitConverter.GetBytes((short)1);
            //byteArray[2] = ids[0];
            //byteArray[3] = ids[1];
        }

        public void moveForward(int stepsize, int velocity, int steps)
        {
            // Set speed
            speed = BitConverter.GetBytes(velocity);
            //byteArray[4] = speed[0];

            int maxTableSize = 5;

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
                    position = BitConverter.GetBytes((short)50 * maxTableSize);
                    //byteArray[5] = position[0];
                    //byteArray[6] = position[1];

                    // Set position from database for dxl 2
                    position = BitConverter.GetBytes((short)50 * maxTableSize);
                    //byteArray[7] = position[0];
                    //byteArray[8] = position[1];

                    // Set end of message
                    msgEnd = BitConverter.GetBytes((short)8888);
                    //byteArray[9] = msgEnd[0];
                    //byteArray[10] = msgEnd[1];

                    sendToPort(byteArray);
                }
            }
        }

        private async void sendToPort(byte[] byteArray)
        {
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace MovementControl
{
    class RobotControl
    {
        GlobalDataSet globalDataSet;
        LocalDbManagement localDbManagement;
        byte[] position, ids, msgStart, msgEnd, speed;
        byte[] byteArray = new byte[64];
        int maxRotAngle = 300;
        int maxIncrements = 1024;
        byte positionReached = 2;

        int SMOOTH_ZONE = 3; // Zone to set position reached bit
        int counter = 0;

        short[] dataArray = { 52,
        107,
        133,
        144,
        151,
        160,
        173,
        189,
        209,
        229,
        248,
        263,
        273,
        276,
        274,
        266,
        254,
        239,
        223,
        207,
        189,
        168,
        138,
        88};
        //short[] dataArray = { 300, 0 };

        // NOTE: MAX 8 IDs per cycle available, because there is a problem to send more than one byte...
        // TODO: Load txt-files into cloud / database and implement possibility to download txt-file from raspberry and import as list

        public RobotControl(GlobalDataSet globalDataSet, LocalDbManagement localDbManagement)
        {
            this.globalDataSet = globalDataSet;
            this.localDbManagement = localDbManagement;
        }

        public async void readSomething()
        {
            byte[] byteArrayIn = new byte[64];
            var bufferArrayIn = byteArrayIn.AsBuffer();
            if (globalDataSet.FirstWriteExecuted)
            {
                await globalDataSet.Port.InputStream.ReadAsync(bufferArrayIn, 1, InputStreamOptions.Partial);
                byte retVal = bufferArrayIn.GetByte(0);
                //Debug.WriteLine("retVal: " + retVal);
                if (retVal == 1) globalDataSet.NextPositionRequest = true;
            }
        }

        public void moveForward(int stepsize, int velocity, int steps)
        {
            // Set start of message
            msgStart = BitConverter.GetBytes((short)9999);
            for (int i = 0; i < msgStart.Length; i++) byteArray[i] = msgStart[i];

            // Set ids
            // Binär (i.e. 0000 0011 for motor 1 and 2)
            ids = BitConverter.GetBytes((short)3);
            for (int i = 0; i < ids.Length; i++) byteArray[i + 2] = ids[i];

            // Set speed for dynamixel 1
            speed = BitConverter.GetBytes((short)velocity);
            for (int i = 0; i < speed.Length; i++) byteArray[i + 4] = speed[i];

            // Set speed for dynamixel 2
            speed = BitConverter.GetBytes((short)velocity);
            for (int i = 0; i < speed.Length; i++) byteArray[i + 6] = speed[i];

            for (int k = 0; k < steps; k++)
            {
                counter = 0;
                // TODO: Add length variable for the count of rows per table (not hardcoded 23!!!)
                while (counter < 23)
                {
                    if (globalDataSet.NextPositionRequest)
                    {
                        // Set position from database for dxl 1
                        position = BitConverter.GetBytes((short)globalDataSet.Moveforward.ElementAt(0).s0[counter]);
                        //position = BitConverter.GetBytes((short)dataArray[counter]);
                        for (int i = 0; i < position.Length; i++) byteArray[i + 8] = position[i];

                        // Set position from database for dxl 2
                        position = BitConverter.GetBytes((short)globalDataSet.Moveforward.ElementAt(1).s1[counter]);
                        //position = BitConverter.GetBytes((short)dataArray[counter]);
                        for (int i = 0; i < position.Length; i++) byteArray[i + 10] = position[i];

                        globalDataSet.NextPositionRequest = false;
                        counter++;
                        sendToPort(byteArray);
                    }
                }
            }
        }

        private async void sendToPort(byte[] byteArray)
        {
            var bufferArray = byteArray.AsBuffer();
            await globalDataSet.Port.OutputStream.WriteAsync(bufferArray);
            globalDataSet.FirstWriteExecuted = true;
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
    }
}

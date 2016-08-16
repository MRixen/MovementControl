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
        LocalDbManagement localDbManagement;
        byte[] position, ids, msgStart, msgEnd, speed;
        byte[] byteArray = new byte[64];
        int maxRotAngle = 300;
        int maxIncrements = 1024;
        // NOTE: MAX 8 IDs per cycle available, because there is a problem to send more than one byte...
        // TODO: Load txt-files into cloud / database and implement possibility to download txt-file from raspberry and import as list

        public RobotControl(GlobalDataSet globalDataSet, LocalDbManagement localDbManagement)
        {
            this.globalDataSet = globalDataSet;
            this.localDbManagement = localDbManagement;
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
                // Note: The lists in a table need to have the same length
                for (int p = 0; p < globalDataSet.ZData_s0.Count; p++)
                {
                    // Set position from database for dxl 1
                    position = BitConverter.GetBytes((short)globalDataSet.ZData_s0[p]);
                    for (int i = 0; i < position.Length; i++) byteArray[i + 8] = position[i];

                    // Set position from database for dxl 2
                    position = BitConverter.GetBytes((short)globalDataSet.ZData_s1[p]);
                    for (int i = 0; i < position.Length; i++) byteArray[i + 10] = position[i];

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

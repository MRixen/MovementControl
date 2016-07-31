using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;

namespace MovementControl
{
    class GlobalDataSet
    {
        private bool stopAllOperations = false;
        private byte[] send_Buffer = new byte[64];
        private Dxl_Control_Data dxlControlData = new Dxl_Control_Data();
        private SerialDevice port;

        public struct Dxl_Control_Data
        {
            private int id;
            private int position;
            private int speed;

            public int Id
            {
                get
                {
                    return id;
                }

                set
                {
                    id = value;
                }
            }

            public int Position
            {
                get
                {
                    return position;
                }

                set
                {
                    position = value;
                }
            }

            public int Speed
            {
                get
                {
                    return speed;
                }

                set
                {
                    speed = value;
                }
            }
        };

        public bool StopAllOperations
        {
            get
            {
                return stopAllOperations;
            }

            set
            {
                stopAllOperations = value;
            }
        }


        public byte[] sendBuffer
        {
            get
            {
                return send_Buffer;
            }

            set
            {
                send_Buffer = value;
            }
        }

        public Dxl_Control_Data DxlControlData
        {
            get
            {
                return dxlControlData;
            }

            set
            {
                dxlControlData = value;
            }
        }

        public SerialDevice Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
            }
        }
    }
}

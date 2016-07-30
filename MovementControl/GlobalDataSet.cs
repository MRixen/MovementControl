using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovementControl
{
    class GlobalDataSet
    {
        private bool stopAllOperations = true;


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
    }
}

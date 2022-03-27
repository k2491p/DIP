using System;
using System.Collections.Generic;
using System.Text;

namespace DIP.Good
{
    public interface IRecord
    {
        enum Recorder 
        {
            VIDEO,
            BLU_RAY,
            HDD
        }

        public void Record();
    }
}

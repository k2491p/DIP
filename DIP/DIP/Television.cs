using DIP.Good;
using System;

namespace DIP
{
    class Television
    {
        static void Main(string[] args)
        {
            IRecord recorder = Factories.CreateRecord();
            recorder.Record();
        }
    }
}

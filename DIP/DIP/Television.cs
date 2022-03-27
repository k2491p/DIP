using DIP.Good;
using System;

namespace DIP
{
    class Television
    {
        static void Main(string[] args)
        {
            RecordGood();

            RecordBad()
        }

        public static void RecordGood()
        {
            IRecord recorder = Factories.CreateRecord();
            recorder.Record();
        }

        public static void RecordBad()
        {
            if (ConnectsVideo())
            {
                Video video = new Video();
                video.Record();
                return;
            }

            if (ConnectsBluRay())
            {
                Blu_ray bluRay = new Blu_ray();
                bluRay.Record();
                return;
            }

            HDD hdd = new HDD();
            hdd.Record();
        }
    }
}

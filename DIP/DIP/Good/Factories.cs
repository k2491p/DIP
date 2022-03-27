using System;
using System.Collections.Generic;
using System.Text;

namespace DIP.Good
{
    public static class Factories
    {
        public static IRecord CreateRecord()
        {
#if DEBUG
            return new Fake();
#endif


            // テレビに接続されている機器取得
            var recorder = GetRecorder();

            if (recorder == IRecord.Recorder.VIDEO)
            {
                return new Video();
            }

            if (recorder == IRecord.Recorder.BLU_RAY)
            {
                return new Blu_ray();
            }

            return new HDD();
        }
    }
}

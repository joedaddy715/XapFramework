using System.Diagnostics;

namespace Xap.Infrastructure.Utilities {
    public class XapStopWatch {
        Stopwatch swatch = null;
        private string _elapsedTime = string.Empty;

        public XapStopWatch() {
            swatch = new Stopwatch();
        }


        public void Start() => swatch.Start();

        public void Stop() => swatch.Stop();

        public string ElapsedTime => $"Runtime hr:{swatch.Elapsed.Hours.ToString()} sec:{swatch.Elapsed.Seconds.ToString()} ms:{swatch.Elapsed.Milliseconds.ToString()} ticks:{swatch.ElapsedTicks.ToString()}";
    }
}

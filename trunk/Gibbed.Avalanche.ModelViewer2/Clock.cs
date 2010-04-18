using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Gibbed.Avalanche.ModelViewer2
{
    public class Clock
    {
        #region Public Interface
        /// <summary>
        /// Initializes a new instance of the <see cref="Clock"/> class.
        /// </summary>
        public Clock()
        {
            this.Frequency = Stopwatch.Frequency;
        }

        public void Start()
        {
            this.Count = Stopwatch.GetTimestamp();
            this.IsRunning = true;
        }

        /// <summary>
        /// Updates the clock.
        /// </summary>
        /// <returns>The time, in seconds, that elapsed since the previous update.</returns>
        public float Update()
        {
            float result = 0.0f;
            
            if (this.IsRunning == true)
            {
                long last = this.Count;
                this.Count = Stopwatch.GetTimestamp();
                result = (float)(this.Count - last) / this.Frequency;
            }

            return result;
        }
        #endregion
        #region Implementation Detail
        private bool IsRunning;
        private readonly long Frequency;
        private long Count;
        #endregion
    }
}

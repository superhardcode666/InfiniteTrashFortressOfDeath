using UnityEngine;

namespace QFSW.QC.Actions
{
    /// <summary>
    ///     Waits for the given amount of seconds using scaled time.
    /// </summary>
    public class Wait : ICommandAction
    {
        private readonly float _duration;
        private float _startTime;

        /// <param name="seconds">The duration to wait in seconds.</param>
        public Wait(float seconds)
        {
            _duration = seconds;
        }

        public bool IsFinished => Time.time >= _startTime + _duration;
        public bool StartsIdle => true;

        public void Start(ActionContext ctx)
        {
            _startTime = Time.time;
        }

        public void Finalize(ActionContext ctx)
        {
        }
    }
}
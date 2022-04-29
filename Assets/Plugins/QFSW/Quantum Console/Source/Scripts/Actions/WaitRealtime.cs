using UnityEngine;

namespace QFSW.QC.Actions
{
    /// <summary>
    ///     Waits for the given amount of seconds using real time.
    /// </summary>
    public class WaitRealtime : ICommandAction
    {
        private readonly float _duration;
        private float _startTime;

        /// <param name="seconds">The duration to wait for in seconds.</param>
        public WaitRealtime(float seconds)
        {
            _duration = seconds;
        }

        public bool IsFinished => Time.realtimeSinceStartup >= _startTime + _duration;
        public bool StartsIdle => true;

        public void Start(ActionContext ctx)
        {
            _startTime = Time.realtimeSinceStartup;
        }

        public void Finalize(ActionContext ctx)
        {
        }
    }
}
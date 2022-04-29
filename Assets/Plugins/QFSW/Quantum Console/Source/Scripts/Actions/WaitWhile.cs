using System;

namespace QFSW.QC.Actions
{
    /// <summary>
    ///     Waits while the given condition is met.
    /// </summary>
    public class WaitWhile : ICommandAction
    {
        private readonly Func<bool> _condition;

        /// <param name="condition">The condition to wait on.</param>
        public WaitWhile(Func<bool> condition)
        {
            _condition = condition;
        }

        public bool IsFinished => _condition();
        public bool StartsIdle => true;


        public void Start(ActionContext context)
        {
        }

        public void Finalize(ActionContext context)
        {
        }
    }
}
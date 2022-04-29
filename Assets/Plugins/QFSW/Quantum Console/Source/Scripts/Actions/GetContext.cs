using System;

namespace QFSW.QC.Actions
{
    /// <summary>
    ///     Gets the <c>ActionContext</c> that the command is currently being invoked on.
    /// </summary>
    public class GetContext : ICommandAction
    {
        private readonly Action<ActionContext> _onContext;

        /// <param name="onContext">Action to invoke when the context is retrieved.</param>
        public GetContext(Action<ActionContext> onContext)
        {
            _onContext = onContext;
        }

        public bool IsFinished => true;
        public bool StartsIdle => false;

        public void Start(ActionContext context)
        {
        }

        public void Finalize(ActionContext context)
        {
            _onContext(context);
        }
    }
}
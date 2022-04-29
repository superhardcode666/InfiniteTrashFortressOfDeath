using System;
using System.Linq;
using UnityEngine;

namespace QFSW.QC.Actions
{
    /// <summary>
    ///     Waits for any key to be pressed and returns the key via the given delegate.
    /// </summary>
    public class GetKey : ICommandAction
    {
        private static readonly KeyCode[] KeyCodes = Enum.GetValues(typeof(KeyCode))
            .Cast<KeyCode>()
            .Where(k => (int) k < (int) KeyCode.Mouse0)
            .ToArray();

        private readonly Action<KeyCode> _onKey;
        private KeyCode _key;

        /// <param name="onKey">The action to perform when a key is pressed.</param>
        public GetKey(Action<KeyCode> onKey)
        {
            _onKey = onKey;
        }

        public bool IsFinished
        {
            get
            {
                _key = GetCurrentKeyDown();
                return _key != KeyCode.None;
            }
        }

        public bool StartsIdle => true;

        public void Start(ActionContext context)
        {
        }

        public void Finalize(ActionContext context)
        {
            _onKey(_key);
        }

        private KeyCode GetCurrentKeyDown()
        {
            return KeyCodes.FirstOrDefault(InputHelper.GetKeyDown);
        }
    }
}
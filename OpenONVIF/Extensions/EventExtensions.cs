using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Extensions
{
    internal static class EventExtensions
    {
        internal static void Trigger(this EventHandler eventHandler, object sender, EventArgs eventArgs)
        {
            if (eventHandler != null)
            {
                Action<IAsyncResult> callback = (iar) => eventHandler.EndInvoke(iar);
                eventHandler.BeginInvoke(sender, eventArgs, Trigger_EndInvoke, callback);
            }
        }

        internal static void Trigger<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            if (eventHandler != null)
            {
                Action<IAsyncResult> callback = (iar) => eventHandler.EndInvoke(iar);
                eventHandler.BeginInvoke(sender, eventArgs, Trigger_EndInvoke, callback);
            }
        }

        private static void Trigger_EndInvoke(IAsyncResult iar)
        {
            Action<IAsyncResult> callback = (Action<IAsyncResult>)iar.AsyncState;
            callback(iar);
        }
    }
}

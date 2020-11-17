using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LeanCode.Standard.Threading
{
    public partial class WaitFor
    {
        public static TResult Run<TResult>(Func<TResult> function, TimeSpan timeout)
        {
            return new WaitForImpl(timeout).RunCallback(function);
        }

        public static void Run(Action action, TimeSpan timeout)
        {
            new WaitForImpl(timeout).RunCallback<object>(() => { action(); return null; });
        }


        /// <summary>
        /// The performance trick is that we do not interrupt the current
        /// running thread. Instead, we just create a watcher that will sleep
        /// until the originating thread terminates or until the timeout is
        /// elapsed.
        /// </summary>
        private class WaitForImpl
        {
            private readonly TimeSpan _timeout;

            public WaitForImpl(TimeSpan timeout)
            {
                _timeout = timeout;
            }

            public TResult RunCallback<TResult>(Func<TResult> function)
            {
                if (function == null) throw new ArgumentNullException("function");
                object syncLock = new object();
                bool isFunctionCompleted = false;

                WaitCallback watcherCallback = currentThreadObj =>
                {
                    Thread watchedThread = (Thread)currentThreadObj;

                    lock (syncLock)
                    {
                        if (!isFunctionCompleted)
                        {
                            Monitor.Wait(syncLock, _timeout);
                        }
                    }

                    if (!isFunctionCompleted)
                    {
                        watchedThread.Abort();
                    }
                };

                try
                {
                    Thread watcherThread = new Thread(new ParameterizedThreadStart(watcherCallback));

                    watcherThread.IsBackground = true;
                    watcherThread.Start(Thread.CurrentThread);

                    return function();
                }
                catch (ThreadAbortException ex)
                {
                    Thread.ResetAbort();

                    throw new TimeoutException($"The operation has timed out after {_timeout}.", ex);
                }
                finally
                {
                    lock (syncLock)
                    {
                        isFunctionCompleted = true;
                        Monitor.Pulse(syncLock);
                    }
                }
            }
        }
    }
}

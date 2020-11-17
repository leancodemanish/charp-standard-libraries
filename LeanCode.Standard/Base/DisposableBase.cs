using LeanCode.Standard.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LeanCode.Standard.Base
{
    public abstract class DisposableBase : IDisposable
    {
        protected const int NOTCALLED = 0;
        protected const int CALLED = 1;
        private int _disposedState = NOTCALLED;

        protected bool IsDisposed
        {
            get { return Interlocked.CompareExchange(ref _disposedState, NOTCALLED, NOTCALLED) == CALLED; }
        }

        protected abstract void OnDispose();

        public void Dispose()
        {
            if (IsDisposed) return;//Dispose only once
            OnDispose();
            Interlocked.Exchange(ref _disposedState, CALLED);
        }

        protected void DisposeIfNotNull(object mayBeDisposable)
        {
            var disposable = mayBeDisposable as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        protected void TryDispose(IDisposable anyDisposable)
        {
            try
            {
                if (anyDisposable == null) return;
                //Allow safe cleanup of resources/connections

                //MB: Apparently 5second is not enough for RFA connection to close down, 10second seem to suffice
                WaitFor.Run(anyDisposable.Dispose, TimeSpan.FromSeconds(10));//To Make sure that any dispose is not taking more than 10secs
            }
            catch (Exception ex)
            {
                //Ignore, just log
                OnDisposeError(string.Format("Failed to dispose {0} on shutdown. Exception: {1}", anyDisposable.GetType(), ex));
            }
        }

        protected virtual void OnDisposeError(string errorMessage) { }
    }
}

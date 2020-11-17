using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LeanCode.Standard.Threading
{
    public class SingleShotGuard
    {
        protected const int NOTCALLED = 0;
        protected const int CALLED = 1;
        private int _disposedState = NOTCALLED;

        public bool IsSet
        {
            get { return Interlocked.CompareExchange(ref _disposedState, NOTCALLED, NOTCALLED) == CALLED; }
        }

        public void Set()
        {
            Interlocked.Exchange(ref _disposedState, CALLED);
        }
    }
}

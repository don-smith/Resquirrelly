using System;
using System.Threading;
using System.Threading.Tasks;

namespace Resquirrelly
{
    public interface ITimer
    {
        Task Start();
        void Stop();
    }

    /// <summary>
    /// A portable timer that will execute a callback delegate after a time 
    /// delay and/or on a timed interval period.
    /// Inspired from: http://stackoverflow.com/questions/12555049/timer-in-portable-library
    /// </summary>
    public sealed class Timer : CancellationTokenSource, ITimer
    {
        private readonly bool _oneShot = true;
        private readonly Action<Object> _callback;
        private readonly Object _state;
        private readonly int _delayInMilliseconds;
        private readonly int _intervalInMilliseconds;
        private readonly bool _waitForCallbackBeforeNextPeriod;

        /// <summary>
        /// Creates a one-shot timer that executes the callback after a delay.
        /// </summary>
        /// <param name="callback">The action to execure.</param>
        /// <param name="state">The parameter to the callback.</param>
        /// <param name="delayInMilliseconds">The initial delay before executing the callback.</param>
        public Timer(Action<object> callback, object state, int delayInMilliseconds)
            : this(callback, state, delayInMilliseconds, 0, true)
        {
        }

        /// <summary>
        /// Creates an interval timer that executes the callback after a delay and
        /// again after each interval.
        /// </summary>
        /// <param name="callback">The action to execure.</param>
        /// <param name="state">The parameter to the callback.</param>
        /// <param name="delayInMilliseconds">The initial delay before executing the callback.</param>
        /// <param name="intervalInMilliseconds">The interval to repeat the callback.</param>
        public Timer(Action<object> callback, object state, int delayInMilliseconds, int intervalInMilliseconds)
            : this(callback, state, delayInMilliseconds, intervalInMilliseconds, true)
        {
        }

        /// <summary>
        /// Creates a cancellable timer that will execute a callback after a 
        /// delay and/or on a specified interval.
        /// </summary>
        /// <param name="callback">The action to execure.</param>
        /// <param name="state">The parameter to the callback.</param>
        /// <param name="delayInMilliseconds">The initial delay before executing the callback.</param>
        /// <param name="intervalInMilliseconds">The interval to repeat the callback.</param>
        /// <param name="waitForCallbackBeforeNextPeriod">Run intervals in sequence. Default is true.</param>
        public Timer(Action<object> callback, object state, int delayInMilliseconds, int intervalInMilliseconds, bool waitForCallbackBeforeNextPeriod = true)
        {
            _oneShot = intervalInMilliseconds == 0;
            _callback = callback;
            _state = state;
            _delayInMilliseconds = delayInMilliseconds;
            _intervalInMilliseconds = intervalInMilliseconds;
            _waitForCallbackBeforeNextPeriod = waitForCallbackBeforeNextPeriod;
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public async Task Start()
        {
            await Task.Delay(_delayInMilliseconds, Token).ContinueWith(async (t, s) =>
            {
                var tuple = (Tuple<Action<object>, object>)s;

                while (!IsCancellationRequested)
                {
                    if (_waitForCallbackBeforeNextPeriod)
                        tuple.Item1(tuple.Item2);
                    else
                        Task.Run(() => tuple.Item1(tuple.Item2));

                    if (_oneShot)
                    {
                        Cancel();
                        break;
                    }

                    await Task.Delay(_intervalInMilliseconds, Token).ConfigureAwait(false);
                }
            },
                Tuple.Create(_callback, _state),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.Default
            );
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop()
        {
            Cancel();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Cancel();

            base.Dispose(disposing);
        }
    }
}

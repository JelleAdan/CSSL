using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling
{
    public abstract class IterativeProcess<T> : IProcess
    {
        public enum EndStateIndicator
        {
            END_CONDITION_SATISFIED,
            ALL_STEPS_COMPLETED,
            PROCESS_TIME_EXCEEDED
        }

        protected virtual double maxComputationalTimeMiliseconds => throw new NotImplementedException();

        private DateTime beginComputionalTime;

        private bool IsComputationalTimeExceeded => DateTime.Now.Subtract(beginComputionalTime).TotalMilliseconds < maxComputationalTimeMiliseconds;

        public string Name => GetType().Name;

        public ProcessState CurrentState { get; private set; }

        public bool IsCreated => CurrentState.GetType().Name == "CreatedState";

        public bool IsInitialized => CurrentState.GetType().Name == "InitializedState";

        public bool IsRunning => CurrentState.GetType().Name == "RunningState";

        public bool IsEnded => CurrentState.GetType().Name == "EndedState";

        private CreatedState createdState;

        private InitializedState initializedState;

        private RunningState runningState;

        private EndedState endedState;

        internal protected bool StopFlag { get; protected set; }

        internal protected bool IsDoneFlag { get; protected set; }

        private void SetState(ProcessState processState)
        {
            CurrentState = processState;
        }

        internal IterativeProcess()
        {
            createdState = new CreatedState(this);
            initializedState = new InitializedState(this);
            runningState = new RunningState(this);
            endedState = new EndedState(this);
            CurrentState = createdState;
        }

        protected virtual bool HasNext => throw new NotImplementedException();

        public void TryInitialize()
        {
            if (CurrentState.TryInitialize())
            {
                DoInitialize();
            }
        }

        public void TryRunAll()
        {
            if (IsCreated || IsEnded)
            {
                TryInitialize();
            }

            if (CurrentState.TryRunAll())
            {
                DoRunAll();
            }
        }

        public void TryRunNext()
        {
            if (IsCreated || IsEnded)
            {
                TryInitialize();
            }

            if (CurrentState.TryRunNext())
            {
                DoRunNext();
            }
        }

        public void TryEnd()
        {
            if (CurrentState.TryEnd())
            {
                DoEnd();
            }
        }

        protected virtual void DoInitialize()
        {
            SetState(initializedState);
            beginComputionalTime = DateTime.Now;
        }

        protected void DoRunAll()
        {
            SetState(runningState);

            if (HasNext)
            {
                while (!IsDoneFlag)
                {
                    TryRunNext();
                }
            }
            else
            {
                throw new Exception($"Can not run {Name} because there are no iterations.");
            }

            TryEnd();
        }

        protected void DoRunNext()
        {
            SetState(runningState);

            if (!IsDoneFlag && !HasNext)
            {
                throw new Exception($"Expected another step in {Name} but there is none.");
            }

            RunIteration();

            CheckIsDoneFlag();
        }

        private void CheckIsDoneFlag()
        {
            if (StopFlag)
            {
                IsDoneFlag = true;
            }
            else if (!HasNext)
            {
                IsDoneFlag = true;
            }
            else if (IsComputationalTimeExceeded)
            {
                IsDoneFlag = true;
            }
        }

        protected virtual void DoEnd()
        {
            SetState(endedState);
        }

        protected virtual T NextIteration() { throw new NotImplementedException(); }

        protected virtual void RunIteration() { throw new NotImplementedException(); }

        internal void Stop()
        {
            StopFlag = true;
        }
    }
}

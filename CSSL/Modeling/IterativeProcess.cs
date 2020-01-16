using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling
{
    public abstract class IterativeProcess<T> : IProcess
    {
        public string Name => throw new NotImplementedException();

        public ProcessState CurrentState { get; private set; }

        public bool IsCreated => CurrentState.GetType().Name == "CreatedState";

        public bool IsInitialized => CurrentState.GetType().Name == "InitializedState";

        public bool IsRunning => CurrentState.GetType().Name == "RunningState";

        public bool IsEnded => CurrentState.GetType().Name == "EndedState";

        private CreatedState createdState;

        private InitializedState initializedState;

        private RunningState runningState;

        private EndedState endedState;

        private void SetState(ProcessState processState)
        {
            CurrentState = processState;
        }

        public IterativeProcess()
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
            if (CurrentState.TryRunAll())
            {
                DoRunAll();
            }
        }

        public void TryRunNext()
        {
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
        }

        protected void DoRunAll()
        {
            if (!IsInitialized)
            {
                TryInitialize();
            }
            SetState(runningState);
            while (HasNext)
            {
                RunIteration();
            }
            TryEnd();
        }

        protected void DoRunNext()
        {
            if (!IsInitialized)
            {
                TryInitialize();
            }
            SetState(runningState);
            if (HasNext)
            {
                RunIteration();
            }
            else
            {
                TryEnd();
            }
        }

        protected virtual void DoEnd()
        {
            SetState(endedState);
        }

        protected virtual T NextIteration() { throw new NotImplementedException(); }

        protected virtual void RunIteration() { throw new NotImplementedException(); }
    }
}

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

        public bool IsCreated => CurrentState.Name == "CreatedState";

        public bool IsInitialized => CurrentState.Name == "InitializedState";

        public bool IsRunning => CurrentState.Name == "RunningState";

        public bool IsEnded => CurrentState.Name == "EndedState";

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

        protected bool HasNext => throw new NotImplementedException();

        public void TryInitialize()
        {
            if (CurrentState.TryInitialize())
            {
                DoInitialize();
            }
        }

        public void TryRun()
        {
            if (CurrentState.TryRun())
            {
                DoRun();
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

        protected virtual void DoRun()
        {
            SetState(runningState);
        }

        protected virtual void DoRunNext()
        {
            SetState(runningState);
        }

        protected virtual void DoEnd()
        {
            SetState(endedState);
        }
    }
}

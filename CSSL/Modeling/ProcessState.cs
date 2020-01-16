using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling
{
    public abstract class ProcessState
    {
        protected IProcess process;

        public ProcessState(IProcess process)
        {
            this.process = process;
        }

        public virtual bool TryInitialize()
        {
            throw new Exception($"\nTried to initialize {process.GetType().Name} from an illegal state: {process.CurrentState.GetType().Name}");
        }

        public virtual bool TryRunAll()
        {
            throw new Exception($"\nTried to run {process.GetType().Name} from an illegal state: {process.CurrentState.GetType().Name}");
        }

        public virtual bool TryRunNext()
        {
            throw new Exception($"\nTried to run next step in {process.GetType().Name} from an illegal state: {process.CurrentState.GetType().Name}");
        }

        public virtual bool TryEnd()
        {
            throw new Exception($"\nTried to end {process.GetType().Name} from an illegal state: {process.CurrentState.GetType().Name}");
        }
    }

    public sealed class CreatedState : ProcessState
    {
        public CreatedState(IProcess process) : base(process)
        {
        }

        public override bool TryInitialize() { return true; }

        public override bool TryEnd() { return true; }
    }

    public sealed class InitializedState : ProcessState
    {
        public InitializedState(IProcess process) : base(process)
        {
        }

        public override bool TryRunAll() { return true; }

        public override bool TryRunNext() { return true; }

        public override bool TryEnd() { return true; }
    }

    public sealed class RunningState : ProcessState
    {
        public RunningState(IProcess process) : base(process)
        {
        }

        public override bool TryRunAll() { return true; }

        public override bool TryRunNext() { return true; }

        public override bool TryEnd() { return true; }
    }

    public sealed class EndedState : ProcessState
    {
        public EndedState(IProcess process) : base(process)
        {
        }

        public override bool TryInitialize() { return true; }
    }
}

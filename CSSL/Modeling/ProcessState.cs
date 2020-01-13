using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling
{
    public abstract class ProcessState : IName
    {
        public virtual string Name => throw new NotImplementedException();
        
        protected IProcess process;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process">The parent process.</param>
        public ProcessState(IProcess process)
        {
            this.process = process;
        }

        public virtual bool TryInitialize()
        {
            throw new Exception($"\nTried to initialize {process.Name} from an illegal state: {process.CurrentState.Name}");
        }

        public virtual void TryRun()
        {
            throw new Exception($"\nTried to run {process.Name} from an illegal state: {process.CurrentState.Name}");
        }

        public virtual void TryRunNext()
        {
            throw new Exception($"\nTried to run next step in {process.Name} from an illegal state: {process.CurrentState.Name}");
        }

        public virtual void TryEnd()
        {
            throw new Exception($"\nTried to end {process.Name} from an illegal state: {process.CurrentState.Name}");
        }
    }

    public sealed class CreatedState : ProcessState
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="process">The parent process.</param>
        public CreatedState(IProcess process) : base(process)
        {
        }

        public override string Name => "CreatedState";

        public override bool TryInitialize()
        {
            return true;
            process.DoInitialize();
        }

        public override void TryEnd()
        {
            process.DoEnd();
        }
    }

    public sealed class InitializedState : ProcessState
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="process">The parent process.</param>
        public InitializedState(IProcess process) : base(process)
        {
        }

        public override string Name => "InitializedState";

        public override void TryRun()
        {
            process.DoRun();
        }

        public override void TryRunNext()
        {
            process.DoRunNext();
        }

        public override void TryEnd()
        {
            process.DoEnd();
        }
    }

    public sealed class RunningState : ProcessState
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="process">The parent process.</param>
        public RunningState(IProcess process) : base(process)
        {
        }

        public override string Name => "RunningState";

        public override void TryRun()
        {
            process.DoRun();
        }

        public override void TryRunNext()
        {
            process.DoRunNext();
        }

        public override void TryEnd()
        {
            process.DoEnd();
        }
    }

    public sealed class EndedState : ProcessState
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="process">The parent process.</param>
        public EndedState(IProcess process) : base(process)
        {
        }

        public override string Name => "EndedState";
    }
}

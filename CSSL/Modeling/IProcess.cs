using CSSL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling
{
    public interface IProcess : IName
    {
        /// <summary>
        /// The current state of the process. 
        /// </summary>
        ProcessState CurrentState { get; }

        /// <summary>
        /// Flag to indicate if process is created.
        /// </summary>
        bool IsCreated { get; }

        /// <summary>
        /// Flag to indicate if process is initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Flag to indicate if process is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Flag to indicate if process is ended.
        /// </summary>
        bool IsEnded { get; }

        /// <summary>
        /// Flag to indicate if the process has another step to run.
        /// </summary>
        //bool HasNext { get; }

        /// <summary>
        /// Method to initialize the process. 
        /// First checks if state transition is allowed, if allowed it proceeds, otherwise it throws an error. 
        /// </summary>
        void TryInitialize();

        /// <summary>
        /// Method to run all steps in the process. 
        /// First checks if state transition is allowed, if allowed it proceeds, otherwise it throws an error.
        /// </summary>
        void TryRun();

        /// <summary>
        /// Method to run the next step in the process. 
        /// First checks if state transition is allowed, if allowed it proceeds, otherwise it throws an error.
        /// </summary>
        void TryRunNext();

        /// <summary>
        ///  Method to end the process. 
        ///  First checks if state transition is allowed, if allowed it proceeds, otherwise it throws an error.
        /// </summary>
        void TryEnd();
    }
}

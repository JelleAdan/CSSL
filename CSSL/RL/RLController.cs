using CSSL.Examples.AccessController;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CSSL.RL
{
    public class RLController : IDisposable
    {
        public RLLayer RLLayer { get; }

        //private CancellationToken cancellationToken { get; }

        private Mutex mutex { get; }

        private MemoryMappedFile mmfResponse { get; }

        private MemoryMappedFile mmfAction { get; }

        private MemoryMappedFile mmfFlag { get; }

        private int overtakeCount = 0; // TODO Remove this

        private enum Flag : byte
        {
            WAIT, RESET, ACT, CANCEL
        }

        public RLController(RLLayer RLLayer)
        {
            this.RLLayer = RLLayer;
            //cancellationToken = new CancellationToken();
            bool mutexCreated;
            mutex = new Mutex(true, "cssl_rl_mutex", out mutexCreated);
            mmfResponse = MemoryMappedFile.CreateNew("response", 100);
            mmfAction = MemoryMappedFile.CreateNew("action", 4);
            mmfFlag = MemoryMappedFile.CreateNew("flag", 1);
            SetFlag(Flag.WAIT);
            mutex.ReleaseMutex();

        }

        public void Dispose()
        {
            mutex.Dispose();
            mmfResponse.Dispose();
            mmfAction.Dispose();
            mmfFlag.Dispose();
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Flag flag = Wait(cancellationToken);

                switch (flag)
                {
                    case Flag.ACT:
                        Act();
                        break;
                    case Flag.RESET:
                        Reset();
                        break;
                    case Flag.CANCEL:
                        break;
                }

                //mutex.ReleaseMutex(); // TODO MUTEX
            }

            Dispose(); 
        }

        private void SetFlag(Flag flag)
        {
            using (MemoryMappedViewStream stream = mmfFlag.CreateViewStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.BaseStream.Position = 0;
                writer.Write((byte)flag);
            }
        }

        private Flag ReadFlag()
        {
            using (MemoryMappedViewStream stream = mmfFlag.CreateViewStream())
            {
                BinaryReader reader = new BinaryReader(stream);
                return (Flag)reader.ReadByte();
            }
        }

        private int ReadAction()
        {
            using (MemoryMappedViewStream stream = mmfAction.CreateViewStream())
            {
                BinaryReader reader = new BinaryReader(stream);
                return reader.ReadInt32();
            }
        }

        private void WriteResponse(Response response)
        {
            using (MemoryMappedViewStream stream = mmfResponse.CreateViewStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(JsonSerializer.Serialize(response));
            }
        }

        private Flag Wait(CancellationToken cancellationToken)
        {
            Flag flag;

            while (!cancellationToken.IsCancellationRequested)
            {
                mutex.WaitOne(); // TODO MUTEX

                flag = ReadFlag();

                if (flag != Flag.WAIT)
                {
                    return flag;
                }
                else
                {
                    //overtakeCount++;
                    //Console.WriteLine(overtakeCount);
                }

                mutex.ReleaseMutex(); // TODO MUTEX
            }

            return Flag.CANCEL;
        }

        public void Reset()
        {
            try
            {
                Response response = RLLayer.Reset();

                WriteResponse(response);

                SetFlag(Flag.WAIT);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void Act()
        {
            try
            {
                int action = ReadAction();

                Response response = RLLayer.Act(action);

                WriteResponse(response);

                SetFlag(Flag.WAIT);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}

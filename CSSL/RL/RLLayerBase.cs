using CSSL.Modeling;
using CSSL.Modeling.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CSSL.RL
{
    public abstract class RLLayerBase
    {
        protected RLSimulation Simulation;

        private RLElementBase sender;

        private double[][] state = new double[1][];

        private double reward;

        private Dictionary<string, string> info;

        public RLLayerBase()
        {
            BuildTrainingEnvironment();
        }

        public Response Reset()
        {
            if (Simulation.MyExecutive.IsPaused)
            {
                Simulation.MyExecutive.TryEnd();
                Simulation.EndTrain();
            }

            Simulation.StartTrain();

            Simulation.Train();

            if (Simulation.MyExecutive.IsEnded)
            {
                Simulation.EndTrain();
            }

            return GetResponse();
        }

        public abstract void BuildTrainingEnvironment();

        public Response Act(int action)
        {
            if (sender != null)
            {
                if (sender.TryAct(action))
                {
                    sender.Act(action);
                    sender = null;
                }
            }
            else
            {
                throw new InvalidOperationException("No sender waiting for action");
            }

            Simulation.Train();

            if (Simulation.MyExecutive.IsEnded)
            {
                Simulation.EndTrain();
            }

            return GetResponse();
        }

        internal void GetAction(RLElementBase sender, double[] state, double reward, Dictionary<string, string> info = null)
        {
            this.sender = sender;
            this.state[0] = state;
            this.reward = reward;
            this.info = info;
        }

        internal void GetAction(RLElementBase sender, double[][] state, double reward, Dictionary<string, string> info = null)
        {
            this.sender = sender;
            this.state = state;
            this.reward = reward;
            this.info = info;
        }

        public Response GetResponse()
        {
            if (sender != null)
            {
                return new Response()
                {
                    State = state,
                    Reward = reward,
                    IsEnded = Simulation.MyExecutive.IsEnded,
                    Info = info
                };
            }

            throw new InvalidOperationException("No sender to respond");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetcodePeering
{
    public class Server : IListener
    {
        private PriorityQueue<FrameState> Messages = new PriorityQueue<FrameState>(100);
        List<IListener> Peers = new List<IListener>();
        public List<Character> Characters;
        public Server(List<Character> characters)
        {
            Characters = characters;
        }
        public void AddPeer(IListener peer) => Peers.Add(peer);
        public void Handle(IListener source, FrameState state)
        {
            Messages.Push(state.Frame, state);
        }
        public void Run(int t)
        {
            while (Messages.Count > 0 && Messages.Top.Priority + DemoScene.DelayInFrames < t)
            {
                int updateT = Messages.Top.Priority;
                var frame = Messages.Top.Value;
                Messages.Pop();
                if (frame.Input.HasValue) Characters[frame.Character].Input = frame.Input.Value;
                if (Messages.Count > 0 && Messages.Top.Priority <= updateT) continue;
                foreach (var character in Characters)
                {
                    foreach (var peer in Peers)
                    {
                        peer.Handle(this, new FrameState()
                        {
                            Frame = updateT,
                            Character = character.Id,
                            Input = character.Input,
                            Position = character.Position,
                        });
                    }
                    character.Position += character.Input * DemoScene.FrameRate;
                }
            }
        }
    }
}

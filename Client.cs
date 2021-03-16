using Spectrum.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetcodePeering
{
    public class FrameSet
    {
        public const int MaxFrames = DemoScene.DelayInFrames * 3;
        private int OldestFrame = 0;
        private DefaultDict<int, DefaultDict<int, FrameState>> Frames
            = new DefaultDict<int, DefaultDict<int, FrameState>>((frame) => new DefaultDict<int, FrameState>(
                (id) => new FrameState() { Frame = frame, Character = id }, true), true);
        public FrameState this[int t, int character]
        {
            get => Frames[t][character];
            private set
            {
                if (t < OldestFrame) throw new InvalidOperationException("Frame too old");
                while (OldestFrame < t - MaxFrames)
                {
                    Frames.Remove(OldestFrame);
                    OldestFrame++;
                }
                Frames[t][character] = value;
            }
        }
        public void Merge(FrameState frame)
        {
            this[frame.Frame, frame.Character] = this[frame.Frame, frame.Character].Merge(frame);
        }
    }
    public class Client : IListener
    {
        public int OldestFrame = 0;
        public FrameSet FrameHistory = new FrameSet();
        public FrameSet KeyFrames = new FrameSet();
        public List<Character> Characters;
        public Action<int> Update;
        private List<IListener> Peers = new List<IListener>();
        int CurrentFrame = 0;
        private PriorityQueue<FrameState> messages = new PriorityQueue<FrameState>(100);
        public Client(List<Character> characters)
        {
            Characters = characters;
            foreach (var character in characters)
                character.Client = this;
        }
        public void AddPeer(IListener peer) => Peers.Add(peer);
        bool HasAuthority(IListener other) => other is Client && DemoScene.ClientAuthority;
        public void ApplyInput(int id, Vector2 input)
        {
            Characters[id].Input = input;
            var frame = new FrameState()
            {
                Frame = CurrentFrame,
                Character = id,
                Input = input,
            };
            KeyFrames.Merge(frame);
            foreach (var peer in Peers)
            {
                // We could update position as well with authority, but input is sufficient
                //frame.Position = HasAuthority(peer) ? (Vector2?)Characters[id].Position : null;
                peer.Handle(this, frame);
            }
        }

        public void Handle(IListener source, FrameState state)
        {
            messages.Push(CurrentFrame + DemoScene.DelayInFrames, state);
        }
        private int? PollMessages(int t)
        {
            int? rewindTo = null;
            while (messages.Count > 0 && messages.Top.Priority < t)
            {
                var messageFrame = messages.Top.Value.Frame;
                if (Apply(messages.Top.Value) && (!rewindTo.HasValue || messageFrame < rewindTo.Value))
                    rewindTo = messageFrame;
                messages.Pop();
            }
            return rewindTo;
        }
        /// <summary>
        /// Returns true if applying this state requires rewinding
        /// </summary>
        public bool Apply(FrameState frame)
        {
            var pastFrame = FrameHistory[frame.Frame, frame.Character];
            bool needRewind = false;
            if (frame.Input.HasValue && pastFrame.Input != frame.Input.Value)
                needRewind = true;
            if (frame.Position.HasValue && pastFrame.Position != frame.Position.Value)
                needRewind = true;
            KeyFrames.Merge(frame);
            return needRewind;
        }

        public void Simulate(int t)
        {
            foreach (var character in Characters)
            {
                var characterFrame = FrameHistory[t, character.Id].Merge(KeyFrames[t, character.Id]);
                if (characterFrame.Input.HasValue) character.Input = characterFrame.Input.Value;
                if (characterFrame.Position.HasValue) character.Position = characterFrame.Position.Value;
                character.Position += character.Input * DemoScene.FrameRate;
                characterFrame = new FrameState()
                {
                    Frame = t + 1,
                    Character = character.Id,
                    Input = character.Input,
                    Position = character.Position,
                };
                FrameHistory.Merge(characterFrame);
            }
        }

        public bool Run(int t)
        {
            Update?.Invoke(t);
            var rewindFrame = PollMessages(t);
            CurrentFrame = rewindFrame ?? CurrentFrame;
            while (CurrentFrame < t)
            {
                Simulate(CurrentFrame);
                CurrentFrame++;
            }
            return rewindFrame.HasValue;
        }
    }
}

using Spectrum.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetcodePeering
{
    public struct FrameState
    {
        public int Frame;
        public int Character;
        public Vector2? Input;
        public Vector2? Position;
        public FrameState Merge(FrameState other)
        {
            if (other.Input.HasValue) Input = other.Input;
            if (other.Position.HasValue) Position = other.Position;
            return this;
        }
    }
}

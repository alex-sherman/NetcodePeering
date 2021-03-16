using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetcodePeering
{
    public interface IListener
    {
        void Handle(IListener source, FrameState state);
    }
}

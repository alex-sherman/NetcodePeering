using Spectrum;
using Spectrum.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetcodePeering
{
    public static class NetcodePeeringEntry
    {
        public static void Main(string[] args) { Entry<NetcodePeeringGame>.Main(args); }
    }
    public class NetcodePeeringGame : SpectrumGame
    {
        protected override void Initialize()
        {
            base.Initialize();
            Root.AddElement(new DemoScene());
        }
    }
}

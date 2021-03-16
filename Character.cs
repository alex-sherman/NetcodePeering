using Spectrum.Framework;
using Spectrum.Framework.Content;
using Spectrum.Framework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetcodePeering
{
    public class Character : GameObject2D
    {
        public int Id { get; private set; }
        public Client Client;
        public Vector2 Input;
        public Character(int id)
        {
            Id = id;
            Texture = ImageAsset.Blank;
            Bounds = new Rectangle(-100, -100, 200, 200);
            Color = Color.Red;
        }
    }
}

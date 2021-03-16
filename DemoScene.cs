using Microsoft.Xna.Framework.Input;
using Spectrum.Framework;
using Spectrum.Framework.Entities;
using Spectrum.Framework.Graphics;
using Spectrum.Framework.Input;
using Spectrum.Framework.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetcodePeering
{
    public class DemoScene : SceneScreen
    {
        public const float FrameRate = 1 / 20f;
        public const bool ClientAuthority = true;
        public const int DelayInFrames = 3;
        new Camera2D Camera => base.Camera as Camera2D;
        Client client1;
        Client client2;
        Server server;
        public DemoScene() : base(new Camera2D())
        {
            InputLayout.Default.Axes1["vertical"] = new Axis1(new KeyboardAxis(Keys.Up, Keys.Down));
            InputLayout.Default.Axes1["horizontal"] = new Axis1(new KeyboardAxis(Keys.Right, Keys.Left));
            var char1 = new InitData<Character>(() => new Character(0) { Position = Vector2.UnitX * -25 * 20 })
                .ToImmutable();
            //var char2 = new InitData<Character>(() => new Character(1)
            //{
            //    Position = Vector2.Zero,
            //    Color = Color.Green
            //}).ToImmutable();
            var characters = new[] { char1 };
            client1 = new Client(characters.Select(c => Manager.CreateEntity(c)).ToList());
            client1.Update = (frame) =>
            {
                client1.ApplyInput(0, InputState.Current.GetAxis2D("horizontal", "vertical", true) * 400);
            };
            client2 = new Client(characters.Select(c =>
                Manager.CreateEntity(c.Set(e => e.Color, new Color(c.Single.Color, 0.2f))))
            .ToList());
            server = new Server(characters.Select(c =>
                Manager.CreateEntity(c.Set(e => e.Color, new Color(Color.Blue, 0.5f))))
            .ToList());
            server.AddPeer(client1);
            server.AddPeer(client2);
            client1.AddPeer(server);
            //client1.AddPeer(client2);
            client2.AddPeer(server);
        }
        int frame = 0;
        float timeSinceLastFrame = 0;
        public override void Update(float dt)
        {
            base.Update(dt);
            timeSinceLastFrame += dt;
            while (timeSinceLastFrame > FrameRate)
            {
                server.Run(frame);
                client1.Run(frame);
                client2.Run(frame);
                timeSinceLastFrame -= FrameRate;
                frame++;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Play_As_You_Go
{
    public class LogoScreen : IntroScreen
    {
        public LogoScreen()
        {
            ScreenTime = TimeSpan.FromSeconds(6.0);
        }

        public override void Remove()
        {
            ScreenManager.AddScreen(new PreMenu(), ControllingPlayer);
            base.Remove();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PAYG
{
    public class LogoScreen : IntroScreen
    {
        public LogoScreen()
        {
            ScreenTime = TimeSpan.FromSeconds(6);
        }

        public override void Remove()
        {
            ScreenManager.AddScreen(new PreMenu());
            base.Remove();
        }
    }
}

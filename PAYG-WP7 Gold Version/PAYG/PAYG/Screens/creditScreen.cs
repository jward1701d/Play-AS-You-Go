#region Info
/**********************************************
 *            Credit Screen Settings          *  
 *                      By:                   *
 *        James Ward & Johnathan Witvoet      *
 *                                            *  
 *  Property of Scrubby Fresh Studios L.L.C.  *
 *  2011.                                     *  
 **********************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PAYG
{
    public class creditScreen : CreditsScreen
    {
        public creditScreen()
        {
            CreditsTime = TimeSpan.FromSeconds(33.20);
        }

        public override void Remove()
        {
            //ScreenManager.AddScreen(new PlayAsYouGoMainMenu());
            base.Remove();
        }
    }

}
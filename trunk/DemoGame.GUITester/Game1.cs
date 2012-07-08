using System;
using System.Linq;
using NetGore;
using NetGore.Graphics;
using NetGore.Graphics.GUI;
using NetGore.IO;
using SFML.Graphics;
using SFML.Window;

namespace DemoGame.GUITester
{
    sealed class Game1 : GameBase
    {
        readonly ScreenManager _screenManager;
        readonly ISkinManager _skinManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game1"/> class.
        /// </summary>
        public Game1() : base(new Point(1024, 768), new Point(1024, 768))
        {
            UseVerticalSync = true;
            ShowMouseCursor = true;

            _skinManager = new SkinManager("Default");
            _screenManager = new ScreenManager(this, _skinManager, "Font/Arial", 14);
            GrhInfo.Load(ContentPaths.Build, _screenManager.Content);

            var ts = new TestScreen(_screenManager);
            _screenManager.ActiveScreen = ts;

            Closed += Game1_Closed;

            KeyPressed += Game1_KeyPressed;

            Run();
        }

        void Game1_Closed(object sender, EventArgs e)
        {
            Dispose();
        }

        void Game1_KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == KeyCode.Return && e.Alt)
                IsFullscreen = !IsFullscreen;
        }

        /// <summary>
        /// When overridden in the derived class, handles drawing the game.
        /// </summary>
        /// <param name="currentTime">The current time.</param>
        protected override void HandleDraw(TickCount currentTime)
        {
            _screenManager.Draw(currentTime);
        }

        /// <summary>
        /// When overridden in the derived class, handles updating the game.
        /// </summary>
        /// <param name="currentTime">The current time.</param>
        protected override void HandleUpdate(TickCount currentTime)
        {
            _screenManager.Update(currentTime);
        }
    }
}
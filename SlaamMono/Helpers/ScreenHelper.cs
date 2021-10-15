using Microsoft.Xna.Framework.Graphics;
using SlaamMono.Screens;
using System;

namespace SlaamMono
{
    public static class ScreenHelper
    {
        private static IScreen CurrentScreen = new LogoScreen(DI.Instance.Get<MainMenuScreen>());
        private static IScreen NextScreen;

        private static bool ChangingScreens = false;

        public static void Update()
        {
            CurrentScreen.Update();

            if (ChangingScreens)
            {
                ChangingScreens = false;
                CurrentScreen.Close();
                CurrentScreen = NextScreen;
                NextScreen = null;
                BackgroundManager.ChangeBG(BackgroundManager.BackgroundType.Normal);
                CurrentScreen.Open();
                GC.Collect();
            }
        }

        public static void Draw(SpriteBatch batch)
        {
            CurrentScreen.Draw(batch);

        }

        public static void ChangeScreen(IScreen scrn)
        {
            ChangingScreens = true;
            NextScreen = scrn;
        }
    }
}

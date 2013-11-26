using System;
using System.Linq;
using System.Windows.Forms;
using Artentus.GameUtils;
using Artentus.GameUtils.Input.DefaultDevices;
using Artentus.GameUtils.Renderers.Direct2D;
using Artentus.GameUtils.Renderers.Gdi;
using Artentus.GameUtils.UI;
using Artentus.GameUtils.Audio.CsCore;
using MainMenu = FillTheRow.UI.MainMenu;

namespace FillTheRow
{
    static class Game
    {
        public static GameLoop Loop;

        [STAThread]
        static void Main(string[] args)
        {
            var window = new GameWindow(!(args.Contains("-windowed")));
            if (args.Contains("-gdi"))
                Loop = GameLoop.CreateWithRenderer<GdiRenderer>(window);
            else
                Loop = GameLoop.CreateWithRenderer<Direct2DRenderer>(window);
            Loop.TargetUpdatesPerSecond = 60;

            var keyboard = new Keyboard();
            keyboard.Initialize(window);
            Loop.Components.Add(keyboard);
            var mouse = new Mouse();
            mouse.Initialize(window);
            Loop.Components.Add(mouse);

            Loop.Components.Add(new CSCoreEngine());
            Loop.Components.Add(new XorshiftEngine());
            Loop.Components.Add(new GameLayer());
            Loop.Components.Add(new TetrominoManager());

            var uiRoot = new UIRoot(window, keyboard, mouse);
            Loop.Components.Add(uiRoot);
            uiRoot.AspectRatio = 16.0f / 9.0f;
            uiRoot.Children.Add(new MainMenu());

            keyboard.BeginCapture();
            mouse.BeginCapture();
            Loop.Start();

            Application.ApplicationExit += (sender, e) =>
            {
                keyboard.EndCapture();
                mouse.EndCapture();
                Loop.Stop();
            };
            Application.Run(window);
        }
    }
}

using System;
using System.Linq;
using System.Windows.Forms;
using Artentus.GameUtils;
using Artentus.GameUtils.Input.DefaultDevices;
using Artentus.GameUtils.Renderers.Direct2D;
using Artentus.GameUtils.Renderers.Gdi;
using Artentus.GameUtils.UI;
using MainMenu = FillTheRow.UI.MainMenu;

namespace FillTheRow
{
    static class Game
    {
        public static XorshiftEngine Random;
        public static GameWindow Window;
        public static GameLoop Loop;
        public static Keyboard Keyboard;
        public static Mouse Mouse;
        public static UIRoot UIRoot;
        public static GameLayer MainGameLayer;
        public static TetrominoManager Manager;

        [STAThread]
        static void Main(string[] args)
        {
            Random = new XorshiftEngine();
            Window = new GameWindow(!(args.Contains("-windowed")));
            Keyboard = new Keyboard();
            Keyboard.Initialize(Window);
            Mouse = new Mouse();
            Mouse.Initialize(Window);
            if (args.Contains("-gdi"))
                Loop = new GameLoop(Renderer.Create<GdiRenderer>(Window));
            else
                Loop = new GameLoop(Renderer.Create<Direct2DRenderer>(Window));
            Loop.TargetUpdatesPerSecond = 60;
            MainGameLayer = new GameLayer();
            Loop.Components.Add(MainGameLayer);
            UIRoot = new UIRoot(Window, Keyboard, Mouse);
            UIRoot.AspectRatio = 16.0f / 9.0f;
            Loop.Components.Add(UIRoot);
            UIRoot.Children.Add(new MainMenu());
            Manager = new TetrominoManager();
            Loop.Components.Add(Manager);
            
            Keyboard.BeginCapture();
            Mouse.BeginCapture();
            Loop.Start();

            Application.ApplicationExit += (sender, e) =>
            {
                Keyboard.EndCapture();
                Mouse.EndCapture();
                Loop.Stop();
            };
            Application.Run(Window);
        }
    }
}

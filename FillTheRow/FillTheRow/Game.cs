using System;
using System.Linq;
using System.Windows.Forms;
using GameUtils;
using GameUtils.Graphics;
using GameUtils.Input.DefaultDevices;
using GameUtils.Renderers.Direct2D1;
using GameUtils.Renderers.Direct2D1_1;
using GameUtils.Renderers.Gdi;
using GameUtils.UI;
using GameUtils.Audio.CsCore;
using MainMenu = FillTheRow.UI.MainMenu;

namespace FillTheRow
{
    static class Game
    {

        [STAThread]
        static void Main(string[] args)
        {
            var window = new GameWindow(!args.Contains("-windowed"));
            var renderers = new Renderer[] { new GdiRenderer(), new Direct2D1Renderer(), new Direct2D1_1Renderer() };
            GameEngine.RegisterComponent(Renderer.IntelligentSelect(renderers, window));
            var loop = new GameLoop();
            loop.TargetUpdatesPerSecond = 60;
            GameEngine.RegisterComponent(loop);

            var keyboard = new Keyboard();
            keyboard.Initialize(window);
            GameEngine.RegisterComponent(keyboard);
            var mouse = new Mouse();
            mouse.Initialize(window);
            GameEngine.RegisterComponent(mouse);

            GameEngine.RegisterComponent(new CSCoreEngine());
            GameEngine.RegisterComponent(new XorshiftEngine());
            GameEngine.RegisterComponent(new TetrominoManager());
            loop.Components.Add(new GameLayer());

            var uiRoot = new UIRoot(window);
            loop.Components.Add(uiRoot);
            uiRoot.AspectRatio = 16.0f / 9.0f;
            uiRoot.Children.Add(new MainMenu());

            keyboard.BeginCapture();
            mouse.BeginCapture();
            loop.Start();

            Application.ApplicationExit += (sender, e) =>
            {
                keyboard.EndCapture();
                mouse.EndCapture();
                loop.Stop();
            };
            Application.Run(window);
        }
    }
}

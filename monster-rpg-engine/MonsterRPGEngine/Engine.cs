﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Windows;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using D2D1Factory = SharpDX.Direct2D1.Factory;
using D3D11Device = SharpDX.Direct3D11.Device;

namespace MonsterRPGEngine {
    /// <summary>
    /// Represents implementation of the game
    /// </summary>
    class Engine {
        public static RenderForm gameWindow;

        private static SwapChain swapChain;
        private static RenderTarget renderTarget;

        long gameTime = 0L;
        Boolean shouldTerminate = false;
        /// <summary>
        /// Load game assets and prepare to start
        /// </summary>
        public void Init() {
            //Run the game window on its own thread
            Thread messageLoopThread = new Thread(() => {
                //Application.Run(gameWindow);
                gameWindow = new RenderForm("MonsterRPGEngine");
                gameWindow.FormClosed += (x, y) => {
                    Terminate();
                };
                //TODO: Learn how to initialize DirectX components
                SwapChainDescription swapChainDesc = new SwapChainDescription() {
                    BufferCount = 2, //Double buffer
                    Usage = Usage.RenderTargetOutput, //We're using this swap chain as a RenderTarget
                    OutputHandle = gameWindow.Handle, //Pass handle of our game window
                    IsWindowed = true, //Yep
                    ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.B8G8R8A8_UNorm),
                    SampleDescription = new SampleDescription(1, 0), //No Multisampling
                    Flags = SwapChainFlags.AllowModeSwitch,
                    SwapEffect = SwapEffect.Discard //Assuming this means buffer will be cleared when it is moved offscreen
                };
                D3D11Device device;
                //SwapChain swapChain;
                D3D11Device.CreateWithSwapChain(SharpDX.Direct3D.DriverType.Hardware, SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport, swapChainDesc, out device, out swapChain);
                Surface backbuffer = Surface.FromSwapChain(swapChain, 0);
                D2D1Factory factory = new D2D1Factory(FactoryType.SingleThreaded, DebugLevel.Error);
                Size2F dpi = factory.DesktopDpi;
                renderTarget = new RenderTarget(factory, backbuffer, new RenderTargetProperties() {
                    DpiX = dpi.Width,
                    DpiY = dpi.Height,
                    MinLevel = FeatureLevel.Level_DEFAULT,
                    PixelFormat = new PixelFormat(Format.Unknown, AlphaMode.Ignore),
                    Type = RenderTargetType.Default,
                    Usage = RenderTargetUsage.None
                });
                gameWindow.Show();
                Application.Run(gameWindow);
            });
            messageLoopThread.SetApartmentState(ApartmentState.STA);
            messageLoopThread.Start();
            while (gameWindow == null || renderTarget == null) { }
            
            
            //TODO: Load game assets
        }

        /// <summary>
        /// Check if the user has given input
        /// </summary>
        public void CheckInputs() {

        }
        /// <summary>
        /// Do an update tick
        /// </summary>
        public void Update() {
            //TODO: Add game logic here
            gameTime++; //Iterate game time
            
        }
        /// <summary>
        /// Render to the screen
        /// </summary>
        public void Render() {
            //TODO: Add render logic here
            Console.Clear();
            Console.WriteLine("Game Time: " + gameTime);
            //Begin Draw
            renderTarget.BeginDraw();
            renderTarget.Clear(Color4.Black);
            //End Draw
            renderTarget.EndDraw();
            //Swap Backbuffer
            swapChain.Present(0, PresentFlags.None);
        }
        /// <summary>
        /// Game loop
        /// </summary>
        public void Run() {
            //TODO: Add engine logic here
            long tickFrequency = 1000L / 60L; //60 ticks per second
            Stopwatch lastTick = new Stopwatch();
            lastTick.Start();
            while (!shouldTerminate) {
                CheckInputs();
                Update();
                Render();
                if (lastTick.ElapsedMilliseconds > tickFrequency) {
                    Console.WriteLine("Tick Lag: " + (lastTick.ElapsedMilliseconds - tickFrequency) + " ms");
                }
                //Wait patiently to perform the next tick
                while (lastTick.ElapsedMilliseconds < tickFrequency) {
                    Thread.Yield();
                }
                lastTick.Restart();
            }
            Terminate();
        }
        /// <summary>
        /// Terminates the game
        /// </summary>
        public void Terminate() {
            //TODO: Destroy and clean up game objects
            Environment.Exit(0);
        }
    }
}

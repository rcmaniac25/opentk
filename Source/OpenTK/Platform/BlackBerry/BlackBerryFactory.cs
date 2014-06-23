#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2014 the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;
using System.Runtime.InteropServices;

using OpenTK.Graphics;
using OpenTK.Input;

namespace OpenTK
{
    namespace Platform.BlackBerry
    {
        //XXX
        // Spaghetti code when it comes to input. InputDriver handles all input. But gets it through Factory. GLNative generates input events using Factory, 
        // which passes them to InputDriver. Throw on top a bunch of reference counting, and it doesn't look pretty. Cleanup...

        class BlackBerryFactory : PlatformFactoryBase
        {
            static BlackBerryInputDriver inputDriver = null;

            #region Context

            static int contextUsage = 0;
            static int inputEventsRequested = 0;

            internal static IntPtr InitialContext { get; private set; }
            internal static bool ScreenEventsEnabled
            {
                get
                {
                    return inputEventsRequested > 0;
                }
            }

            public static IntPtr RequestContext()
            {
                contextUsage++;
                return InitialContext;
            }

            public static void ReleaseContext()
            {
                contextUsage--;
                if (contextUsage < 0)
                {
                    Debug.Print("Context has been released more times then it's been used");
                }
            }

            public static bool RequestScreenEvents()
            {
                if ((inputEventsRequested++) == 1)
                {
                    return BPS.ScreenRequestEvents(InitialContext) == BPS.BPS_SUCCESS;
                }
                return true;
            }

            public static bool StopScreenEvents()
            {
                if ((inputEventsRequested--) == 1)
                {
                    return BPS.ScreenStopEvents(InitialContext) == BPS.BPS_SUCCESS;
                }
                if (inputEventsRequested < 0)
                {
                    Debug.Print("Screen events has been stopped more times then started");
                }
                return true;
            }

            #endregion

            public BlackBerryFactory()
            {
                if (BPS.Initialize() != BPS.BPS_SUCCESS)
                {
                    throw new ApplicationException("Could not initialize BPS");
                }
                if ((InitialContext = Screen.CreateContext()) == IntPtr.Zero)
                {
                    BPS.Shutdown();
                    throw new ApplicationException("Could not create application's screen context. Returned -1");
                }
                // Prevent rotation of app
                BPS.LockRotation(true);
                contextUsage = 0;
            }

            #region IPlatformFactory Members

            public override INativeWindow CreateNativeWindow(int x, int y, int width, int height, string title, GraphicsMode mode, GameWindowFlags options, DisplayDevice device)
            {
                return new BlackBerryGLNative(x, y, width, height, title, mode, options, device);
            }

            public override IDisplayDeviceDriver CreateDisplayDeviceDriver()
            {
                return new BlackBerryDisplayDeviceDriver();
            }

            public override IGraphicsContext CreateGLContext(GraphicsMode mode, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
            {
                BlackBerryWindowInfo bb_win = (BlackBerryWindowInfo)window;
                Egl.EglWindowInfo egl_win = new OpenTK.Platform.Egl.EglWindowInfo(bb_win.Handle, Egl.Egl.GetDisplay(IntPtr.Zero));
                return new Egl.EglUnixContext(mode, egl_win, shareContext, major, minor, flags);
            }

            public override IGraphicsContext CreateGLContext(ContextHandle handle, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
            {
                BlackBerryWindowInfo bb_win = (BlackBerryWindowInfo)window;
                Egl.EglWindowInfo egl_win = new OpenTK.Platform.Egl.EglWindowInfo(bb_win.Handle, Egl.Egl.GetDisplay(IntPtr.Zero));
                return new Egl.EglUnixContext(handle, egl_win, shareContext, major, minor, flags);
            }

            public override GraphicsContext.GetCurrentContextDelegate CreateGetCurrentGraphicsContext()
            {
                return (GraphicsContext.GetCurrentContextDelegate)delegate
                {
                    return new ContextHandle(Egl.Egl.GetCurrentContext());
                };
            }

            public override IKeyboardDriver2 CreateKeyboardDriver()
            {
                return InputDriver.KeyboardDriver;
            }

            public override IMouseDriver2 CreateMouseDriver()
            {
                return InputDriver.MouseDriver;
            }

            public override IGamePadDriver CreateGamePadDriver()
            {
                return InputDriver.GamePadDriver;
            }

            public override IJoystickDriver2 CreateJoystickDriver()
            {
                return InputDriver.JoystickDriver;
            }

            #endregion

            #region Input

            IInputDriver2 InputDriver
            {
                get
                {
                    if (inputDriver == null)
                    {
                        inputDriver = new BlackBerryInputDriver();
                    }
                    return inputDriver;
                }
            }

            internal static IntPtr PollEvent()
            {
                if (contextUsage > 0)
                {
                    IntPtr ev = IntPtr.Zero;
                    if (BPS.GetEvent(out ev, 1) == BPS.BPS_SUCCESS && ev != IntPtr.Zero && 
                        inputEventsRequested > 0 && inputDriver != null)
                    {
                        int domain = BPS.GetDomain(ev);
                        if (domain == BPS.ScreenGetDomain())
                        {
                            inputDriver.HandleScreenEvents(BPS.ScreenGetEvent(ev));
                        }
                    }
                    return ev;
                }
                return IntPtr.Zero;
            }

            #endregion

            protected override void Dispose(bool manual)
            {
                if (!IsDisposed)
                {
                    if (manual)
                    {
                        if (inputDriver != null)
                        {
                            inputDriver.Dispose();
                            inputDriver = null;
                        }
                    }

                    if (inputEventsRequested > 0)
                    {
                        BPS.ScreenStopEvents(InitialContext);
                    }

                    if (contextUsage > 0)
                    {
                        Debug.Print("Context is still in use");
                    }

                    Screen.DestroyContext(InitialContext);
                    InitialContext = IntPtr.Zero;

                    BPS.Shutdown();

                    base.Dispose(manual);
                }
            }
        }
    }

    #region Debugging

#if !(IPHONE || ANDROID || MINIMAL)

    class LoggingState
    {
        private const ushort CODE = 0x544B; //TK in hex

        private int indent = 0;
        private byte level;
        private System.Text.StringBuilder builder;
        private object sync = new object();

        public LoggingState(byte level)
        {
            this.level = level;
            this.builder = new System.Text.StringBuilder();
        }

        private void FlushUnlocked()
        {
            if (builder.Length == 0 || builder.ToString().Trim().Length == 0)
            {
                builder.Length = 0;
                return;
            }
            if (indent > 0)
            {
                builder.Insert(0, "    ", indent);
            }
            Platform.BlackBerry.Slog2.Log(IntPtr.Zero, CODE, level, builder.ToString());
            builder.Length = 0;
        }

        public void Indent() { lock (sync) { indent++; } }
        public void Unindent() { lock (sync) { if ((indent--) == 0) { indent = 0; } } }

        public void Flush()
        {
            lock (sync)
            {
                FlushUnlocked();
            }
        }

        public void Write(string message)
        {
            lock (sync)
            {
                builder.Append(message);
            }
        }

        public void WriteLine(string message)
        {
            lock (sync)
            {
                builder.Append(message);
                FlushUnlocked();
            }
        }

        public void Print(string format, params object[] args)
        {
            lock (sync)
            {
                builder.AppendFormat(format, args);
                FlushUnlocked();
            }
        }
    }

    // System.Diagnostics.Debug
    static class Debug
    {
        private static LoggingState state = new LoggingState(Platform.BlackBerry.Slog2.SLOG2_INFO);

        public static void Write(string message) { state.Write(message); }
        public static void Write(object obj) { Write(obj.ToString()); }
        public static void WriteLine(string message) { state.WriteLine(message); }
        public static void WriteLine(object obj) { WriteLine(obj.ToString()); }
        public static void Print(string message) { Print(message, new object[0]); }
        public static void Print(string format, params object[] args) { state.Print(format, args); }
        public static void Indent() { state.Indent(); }
        public static void Unindent() { state.Unindent(); }
        public static void Flush() { state.Flush(); }
    }

    // System.Diagnostics.Trace
    static class Trace
    {
        private static LoggingState state = new LoggingState(Platform.BlackBerry.Slog2.SLOG2_DEBUG1);

        public static void Write(string message) { state.Write(message); }
        public static void Write(object obj) { Write(obj.ToString()); }
        public static void WriteLine(string message) { state.WriteLine(message); }
        public static void WriteLine(object obj) { WriteLine(obj.ToString()); }
        public static void Indent() { state.Indent(); }
        public static void Unindent() { state.Unindent(); }
        public static void Flush() { state.Flush(); }
    }

#endif

    #endregion
}

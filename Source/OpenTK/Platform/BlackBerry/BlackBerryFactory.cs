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

namespace OpenTK.Platform.BlackBerry
{
    class BlackBerryFactory : PlatformFactoryBase
    {
        internal static IntPtr InitialContext { get; private set; }

        public BlackBerryFactory()
        {
            if ((InitialContext = Screen.CreateContext()) == IntPtr.Zero)
            {
                throw new ApplicationException("Could not create application's screen context. Returned -1");
            }
        }

        #region IPlatformFactory Members

        public override INativeWindow CreateNativeWindow(int x, int y, int width, int height, string title, GraphicsMode mode, GameWindowFlags options, DisplayDevice device)
        {
            //TODO
            throw new NotImplementedException();
        }

        public override IDisplayDeviceDriver CreateDisplayDeviceDriver()
        {
            return new BlackBerryDisplayDeviceDriver();
        }

        public override IGraphicsContext CreateGLContext(GraphicsMode mode, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
        {
            //TODO
            throw new NotImplementedException();
        }

        public override GraphicsContext.GetCurrentContextDelegate CreateGetCurrentGraphicsContext()
        {
            return (GraphicsContext.GetCurrentContextDelegate)delegate
            {
                return new ContextHandle(InitialContext);
            };
        }

        public override IKeyboardDriver2 CreateKeyboardDriver()
        {
            //TODO
            throw new NotImplementedException();
        }

        public override IMouseDriver2 CreateMouseDriver()
        {
            //TODO
            throw new NotImplementedException();
        }

        public override IGamePadDriver CreateGamePadDriver()
        {
            //TODO
            throw new NotImplementedException();
        }

        public override IJoystickDriver2 CreateJoystickDriver()
        {
            //TODO
            throw new NotImplementedException();
        }

        #endregion

        protected override void Dispose(bool manual)
        {
            if (!IsDisposed)
            {
                if (manual)
                {
                    //TODO
                }

                Screen.DestroyContext(InitialContext);
                InitialContext = IntPtr.Zero;

                base.Dispose(manual);
            }
        }
    }
}

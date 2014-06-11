using System;

using OpenTK.Graphics;
using OpenTK.Input;

namespace OpenTK.Platform.BlackBerry
{
    class BlackBerryFactory : PlatformFactoryBase
    {
        #region IPlatformFactory Members

        public override INativeWindow CreateNativeWindow(int x, int y, int width, int height, string title, GraphicsMode mode, GameWindowFlags options, DisplayDevice device)
        {
            //TODO
            throw new NotImplementedException();
        }

        public override IDisplayDeviceDriver CreateDisplayDeviceDriver()
        {
            //TODO
            throw new NotImplementedException();
        }

        public override IGraphicsContext CreateGLContext(GraphicsMode mode, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
        {
            //TODO
            throw new NotImplementedException();
        }

        public override GraphicsContext.GetCurrentContextDelegate CreateGetCurrentGraphicsContext()
        {
            //TODO
            throw new NotImplementedException();
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

        public override IJoystickDriver2 CreateJoystickDriver()
        {
            //TODO
            throw new NotImplementedException();
        }

        #endregion
    }
}

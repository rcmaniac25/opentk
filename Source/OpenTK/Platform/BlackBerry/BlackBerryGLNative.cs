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
using System.Collections.Generic;
#if !MINIMAL
using System.Drawing;
#endif

using OpenTK.Graphics;

namespace OpenTK.Platform.BlackBerry
{
    internal sealed class BlackBerryGLNative : NativeWindowBase
    {
        bool disposed = false;

        BlackBerryWindowInfo window;

        public BlackBerryGLNative(int x, int y, int width, int height, string title, GraphicsMode mode, GameWindowFlags options, DisplayDevice device)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", "Must be higher than zero.");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", "Must be higher than zero.");

            IntPtr windowHandle = Screen.CreateWindow(BlackBerryFactory.InitialContext);
            if (windowHandle == IntPtr.Zero)
            {
                throw new ApplicationException("screen_create_window call failed (returned 0).");
            }
            window = new BlackBerryWindowInfo(windowHandle);

            // Set window properties
            ColorFormat format = mode.ColorFormat;
            PixelFormat screenFormat = PixelFormat.SCREEN_FORMAT_RGB565;
            if (format.Alpha > 0 || format.Red > 5 || format.Green > 6 || format.Blue > 5)
            {
                screenFormat = PixelFormat.SCREEN_FORMAT_RGBA8888;
            }
            if (!Screen.WindowSetInt(windowHandle, Screen.SCREEN_PROPERTY_FORMAT, (int)screenFormat))
            {
                Debug.Print("Window format could not be set to {0}", screenFormat);
            }
            if (!Screen.WindowSetInt(windowHandle, Screen.SCREEN_PROPERTY_USAGE, Screen.SCREEN_USAGE_OPENGL_ES2)) //XXX Probably shouldn't hard code this...
            {
                Debug.Print("Window usage could not be set to OpenGL ES 2.0");
            }
            if (options == GameWindowFlags.FixedWindow)
            {
                if (x != 0 || y != 0)
                {
                    if (Screen.WindowSetInts(windowHandle, Screen.SCREEN_PROPERTY_POSITION, new int[] { x, y }) != Screen.SCREEN_SUCCESS)
                    {
                        Debug.Print("Window position could not be set to {0}x{1}", x, y);
                    }
                }
                if (width != 0 || height != 0)
                {
                    if (Screen.WindowSetInts(windowHandle, Screen.SCREEN_PROPERTY_BUFFER_SIZE, new int[] { width, height }) != Screen.SCREEN_SUCCESS)
                    {
                        Debug.Print("Window buffer size could not be set to {0}x{1}", width, height);
                    }
                }
            }
            if (title != null)
            {
                if (!Screen.WindowSetString(windowHandle, Screen.SCREEN_PROPERTY_ID_STRING, title))
                {
                    Debug.Print("Window ID could not be set to \"{0}\"", title);
                }
            }
            if (device != null)
            {
                if (!Screen.WindowSetIntPtr(windowHandle, Screen.SCREEN_PROPERTY_DISPLAY, (IntPtr)device.Id))
                {
                    Debug.Print("Window display could not be changed from default");
                }
            }
            if (Screen.WindowCreateBuffers(windowHandle, mode.Buffers) != Screen.SCREEN_SUCCESS)
            {
                Debug.Print("Could not create {0} window buffers", mode.Buffers);
                window.Dispose();
                window = null;
                throw new ApplicationException("screen_create_window_buffers failed to create buffers");
            }
        }

        #region INativeWindow members

        public override void Close()
        {
            //TODO
            //navigator_close_window
            throw new NotImplementedException();
        }

        public override Point PointToClient(Point point)
        {
            //TODO
            throw new NotImplementedException();
        }

        public override Point PointToScreen(Point point)
        {
            //TODO
            throw new NotImplementedException();
        }

        public override Icon Icon
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override string Title
        {
            //XXX Doesn't change app title...
            get
            {
                return Screen.WindowGetString(window.Handle, Screen.SCREEN_PROPERTY_ID_STRING);
            }
            set
            {
                if (Title != value)
                {
                    if (Screen.WindowSetString(window.Handle, Screen.SCREEN_PROPERTY_ID_STRING, value))
                    {
                        OnTitleChanged(EventArgs.Empty);
                    }
                }
            }
        }

        public override bool Focused
        {
            //TODO
            get { throw new NotImplementedException(); }
        }

        public override bool Visible
        {
            //XXX this should probably be based on app visibility as opposed to simple window visibility
            get
            {
                int res;
                if (Screen.WindowGetInt(window.Handle, Screen.SCREEN_PROPERTY_VISIBLE, out res) == Screen.SCREEN_SUCCESS)
                {
                    return res == 1;
                }
                return false;
            }
            set
            {
                if (Visible != value)
                {
                    if (Screen.WindowSetInt(window.Handle, Screen.SCREEN_PROPERTY_VISIBLE, value ? 1 : 0))
                    {
                        OnVisibleChanged(EventArgs.Empty);
                    }
                }
            }
        }

        public override bool Exists
        {
            get
            {
                return window != null && window.Handle != IntPtr.Zero;
            }
        }

        public override IWindowInfo WindowInfo
        {
            get { return window; }
        }

        public override WindowState WindowState
        {
            get
            {
                //TODO: only use Fullscreen and Minimized. The others aren't applicable to mobile
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override WindowBorder WindowBorder
        {
            // Border can't be changed in terms of style
            get
            {
                return OpenTK.WindowBorder.Hidden;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override Rectangle Bounds
        {
            //TODO
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        //TODO: Location

        //TODO: Size

        public override Size ClientSize
        {
            //TODO
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool CursorVisible
        {
            get
            {
                return false;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override MouseCursor Cursor
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        //TODO: ProcessEvents

        protected override void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    if (window != null && window.Handle != IntPtr.Zero)
                    {
                        window.Dispose();
                    }
                    window = null;
                }
                else
                {
                    Debug.Print("[Warning] INativeWindow leaked ({0}). Did you forget to call INativeWindow.Dispose()?", this);
                }

                OnDisposed(EventArgs.Empty);
                disposed = true;
            }
        }
    }
}

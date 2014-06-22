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

        bool isClosing = false;
        bool focused = true;
        bool active = true;
        bool visible = true;
        DisplayDevice display;
        Point screenLocation;
        Size screenSize;
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
            screenLocation = Point.Empty;
            screenSize = Size.Empty;
            if (options == GameWindowFlags.FixedWindow)
            {
                if (x != 0 || y != 0)
                {
                    if (Screen.WindowSetInts(windowHandle, Screen.SCREEN_PROPERTY_POSITION, new int[] { x, y }) != Screen.SCREEN_SUCCESS)
                    {
                        Debug.Print("Window position could not be set to {0}x{1}", x, y);
                    }
                    else
                    {
                        screenLocation = new Point(x, y);
                    }
                }
            }
            if (Screen.WindowSetInts(windowHandle, Screen.SCREEN_PROPERTY_BUFFER_SIZE, new int[] { width, height }) != Screen.SCREEN_SUCCESS)
            {
                Debug.Print("Window buffer size could not be set to {0}x{1}", width, height);
            }
            else
            {
                screenSize = new Size(width, height);
            }
            if (title != null)
            {
                if (!Screen.WindowSetString(windowHandle, Screen.SCREEN_PROPERTY_ID_STRING, title))
                {
                    Debug.Print("Window ID could not be set to \"{0}\"", title);
                }
            }
            display = device;
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

            // Startup events
            BPS.NavigatorRequestEvents();
            BPS.ScreenRequestEvents(BlackBerryFactory.InitialContext);
        }

        #region INativeWindow members

        public override void Close()
        {
            if (Exists && !isClosing)
            {
                BPS.NavigatorRequestExit();
            }
        }

        public override Point PointToClient(Point point)
        {
            // From Sdl2NativeWindow
            var origin = Point.Empty;
            if (display != null)
            {
                origin = display.Bounds.Location;
            }
            var client = Location;
            return new Point(point.X + client.X - origin.X, point.Y + client.Y - origin.Y);
        }

        public override Point PointToScreen(Point point)
        {
            // From Sdl2NativeWindow
            var origin = Point.Empty;
            if (display != null)
            {
                origin = display.Bounds.Location;
            }
            var client = Location;
            return new Point(point.X + origin.X - client.X, point.Y + origin.Y - client.Y);
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
            get { return active && focused; }
        }

        public override bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                // If this was Cascades, then we could at least thumbnail it. But that doesn't make it invisible.
                throw new NotImplementedException();
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
                return visible ? focused ? WindowState.Fullscreen : WindowState.Normal : WindowState.Minimized;
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
            get
            {
                return new Rectangle(Location, Size);
            }
            set
            {
                Location = value.Location;
                Size = value.Size;
            }
        }

        public override Point Location
        {
            get
            {
                return screenLocation;
            }
            set
            {
                if (screenLocation != value)
                {
                    if (Screen.WindowSetInts(window.Handle, Screen.SCREEN_PROPERTY_POSITION, new int[] { value.X, value.Y }) == Screen.SCREEN_SUCCESS)
                    {
                        // Update's on redraw
                        screenLocation = value;
                        OnMove(EventArgs.Empty);
                    }
                }
            }
        }

        public override Size Size
        {
            // Internal (client) and external (size) sizes are the same as there are no borders.
            get
            {
                return ClientSize;
            }
            set
            {
                ClientSize = value;
            }
        }

        public override Size ClientSize
        {
            get
            {
                return screenSize;
            }
            set
            {
                if (screenSize != value)
                {
                    if (Screen.WindowSetInts(window.Handle, Screen.SCREEN_PROPERTY_BUFFER_SIZE, new int[] { value.Width, value.Height }) == Screen.SCREEN_SUCCESS)
                    {
                        // Update's on redraw
                        screenSize = value;
                        OnResize(EventArgs.Empty);
                    }
                }
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

        #region Internal members

        private void DestroyWindow()
        {
            if (window != null && window.Handle != IntPtr.Zero)
            {
                BPS.ScreenStopEvents(BlackBerryFactory.InitialContext);
                BPS.NavigatorStopEvents();

                window.Dispose();
            }
            window = null;
        }

        #region ProcessEvents

        public override void ProcessEvents()
        {
            base.ProcessEvents();

            IntPtr ev;
            while (BPS.GetEvent(out ev, 1) == BPS.BPS_SUCCESS && ev != IntPtr.Zero)
            {
                int domain = BPS.GetDomain(ev);
                if (domain == BPS.ScreenGetDomain())
                {
                    int type;
                    IntPtr screenEvent = BPS.ScreenGetEvent(ev);
                    Screen.EventGetInt(screenEvent, Screen.SCREEN_PROPERTY_TYPE, out type);
                    switch (type)
                    {
                        #region Keyboard

                        case Screen.SCREEN_EVENT_KEYBOARD:
                            int flags;
                            int value;
                            int mod;
                            Screen.EventGetInt(screenEvent, Screen.SCREEN_PROPERTY_KEY_FLAGS, out flags);
                            Screen.EventGetInt(screenEvent, Screen.SCREEN_PROPERTY_KEY_SYM, out value);
                            Screen.EventGetInt(screenEvent, Screen.SCREEN_PROPERTY_KEY_MODIFIERS, out mod);

                            UpdateModifierFlags(BlackBerryKeyMap.GetModifiers(mod));
                            Input.Key key = BlackBerryKeyMap.GetKey(value);
                            if ((flags & KeyConst.KEY_DOWN) != 0)
                            {
                                OnKeyDown(key, (flags & KeyConst.KEY_REPEAT) != 0);
                                if ((flags & KeyConst.KEY_SYM_VALID) != 0)
                                {
                                    OnKeyPress(BlackBerryKeyMap.GetAscii(value));
                                }
                            }
                            else
                            {
                                OnKeyUp(key);
                            }
                            break;

                        #endregion

                        #region Mouse

                        case Screen.SCREEN_EVENT_POINTER:
                            int[] position = new int[2];
                            int[] wheel = new int[2];
                            Screen.EventGetInt(screenEvent, Screen.SCREEN_PROPERTY_BUTTONS, out value);
                            Screen.EventGetInts(screenEvent, Screen.SCREEN_PROPERTY_SOURCE_POSITION, ref position);
                            Screen.EventGetInt(screenEvent, Screen.SCREEN_PROPERTY_MOUSE_HORIZONTAL_WHEEL, out wheel[0]);
                            Screen.EventGetInt(screenEvent, Screen.SCREEN_PROPERTY_MOUSE_WHEEL, out wheel[1]);

                            MouseButton buttons = (MouseButton)value;
                            if ((buttons & MouseButton.SCREEN_LEFT_MOUSE_BUTTON) == MouseButton.SCREEN_LEFT_MOUSE_BUTTON && MouseState.IsButtonUp(Input.MouseButton.Left))
                            {
                                OnMouseDown(Input.MouseButton.Left);
                            }
                            else if (MouseState.IsButtonDown(Input.MouseButton.Left))
                            {
                                OnMouseUp(Input.MouseButton.Left);
                            }
                            if ((buttons & MouseButton.SCREEN_RIGHT_MOUSE_BUTTON) == MouseButton.SCREEN_RIGHT_MOUSE_BUTTON && MouseState.IsButtonUp(Input.MouseButton.Right))
                            {
                                OnMouseDown(Input.MouseButton.Right);
                            }
                            else if (MouseState.IsButtonDown(Input.MouseButton.Right))
                            {
                                OnMouseUp(Input.MouseButton.Right);
                            }
                            if ((buttons & MouseButton.SCREEN_MIDDLE_MOUSE_BUTTON) == MouseButton.SCREEN_MIDDLE_MOUSE_BUTTON && MouseState.IsButtonUp(Input.MouseButton.Middle))
                            {
                                OnMouseDown(Input.MouseButton.Middle);
                            }
                            else if (MouseState.IsButtonDown(Input.MouseButton.Middle))
                            {
                                OnMouseUp(Input.MouseButton.Middle);
                            }
                            if (MouseState.X != position[0] || MouseState.Y != position[1])
                            {
                                OnMouseMove(position[0], position[1]);
                            }
                            if (MouseState.Scroll.X != wheel[0] || MouseState.Scroll.Y != wheel[1])
                            {
                                OnMouseWheel(wheel[0] - MouseState.Scroll.X, wheel[1] - MouseState.Scroll.Y);
                            }
                            break;

                        #endregion

                        #region Touch

                        case Screen.SCREEN_EVENT_MTOUCH_TOUCH:
                            int id;
                            Screen.EventGetInt(screenEvent, Screen.SCREEN_PROPERTY_TOUCH_ID, out id);
                            if (id == 0)
                            {
                                position = new int[2];
                                Screen.EventGetInts(screenEvent, Screen.SCREEN_PROPERTY_SOURCE_POSITION, ref position);
                                if (MouseState.X != position[0] || MouseState.Y != position[1])
                                {
                                    OnMouseMove(position[0], position[1]);
                                }
                                OnMouseDown(Input.MouseButton.Left);
                            }
                            break;

                        case Screen.SCREEN_EVENT_MTOUCH_MOVE:
                            Screen.EventGetInt(screenEvent, Screen.SCREEN_PROPERTY_TOUCH_ID, out id);
                            if (id == 0)
                            {
                                position = new int[2];
                                Screen.EventGetInts(screenEvent, Screen.SCREEN_PROPERTY_SOURCE_POSITION, ref position);
                                if (MouseState.X != position[0] || MouseState.Y != position[1])
                                {
                                    OnMouseMove(position[0], position[1]);
                                }
                            }
                            break;

                        case Screen.SCREEN_EVENT_MTOUCH_RELEASE:
                            Screen.EventGetInt(screenEvent, Screen.SCREEN_PROPERTY_TOUCH_ID, out id);
                            if (id == 0)
                            {
                                position = new int[2];
                                Screen.EventGetInts(screenEvent, Screen.SCREEN_PROPERTY_SOURCE_POSITION, ref position);
                                if (MouseState.X != position[0] || MouseState.Y != position[1])
                                {
                                    OnMouseMove(position[0], position[1]);
                                }
                                OnMouseUp(Input.MouseButton.Left);
                            }
                            break;

                        #endregion
                    }
                }
                else if (domain == BPS.NavigatorGetDomain())
                {
                    uint code = BPS.GetCode(ev);
                    switch (code)
                    {
                        case BPS.NAVIGATOR_EXIT:
                            System.ComponentModel.CancelEventArgs close = new System.ComponentModel.CancelEventArgs();
                            try
                            {
                                isClosing = true;
                                OnClosing(close);
                            }
                            finally
                            {
                                isClosing = false;
                            }

                            if (!close.Cancel)
                            {
                                OnClosed(EventArgs.Empty);
                                DestroyWindow();
                            }
                            break;

                        case BPS.NAVIGATOR_WINDOW_STATE:
                            WindowState oldState = this.WindowState;
                            bool oldFocus = this.Focused;
                            bool oldVisible = this.Visible;
                            switch (BPS.NavigatorEventWindowState(ev))
                            {
                                case NavigatorWindowState.Fullscreen:
                                    visible = true;
                                    focused = true;
                                    break;
                                case NavigatorWindowState.Thumbnail:
                                    visible = true;
                                    focused = false;
                                    break;
                                case NavigatorWindowState.Invisible:
                                    visible = false;
                                    focused = false;
                                    break;
                            }
                            if (this.WindowState != oldState)
                            {
                                OnWindowStateChanged(EventArgs.Empty);
                            }
                            if (this.Focused != oldFocus)
                            {
                                OnFocusedChanged(EventArgs.Empty);
                            }
                            if (this.Visible != oldVisible)
                            {
                                OnVisibleChanged(EventArgs.Empty);
                            }
                            break;

                        case BPS.NAVIGATOR_WINDOW_ACTIVE:
                        case BPS.NAVIGATOR_WINDOW_INACTIVE:
                            oldFocus = this.Focused;
                            active = code == BPS.NAVIGATOR_WINDOW_ACTIVE;
                            if (this.Focused != oldFocus)
                            {
                                OnFocusedChanged(EventArgs.Empty);
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #endregion

        protected override void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    DestroyWindow();
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

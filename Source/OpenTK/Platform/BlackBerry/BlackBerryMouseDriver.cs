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
using System.Diagnostics;
using OpenTK.Input;
#if !MINIMAL
using System.Drawing;
#endif

namespace OpenTK.Platform.BlackBerry
{
    class BlackBerryMouseDriver : IMouseDriver2
    {
        class KnownMouse
        {
            public int ID = -1;
            public IntPtr Device;
            public Point LocationRel;
            public Point LocationAbs;
            public int WheelVert;
            public int WheelHorz;
            public MouseButton Button;
        }

        readonly object sync = new object();
        readonly IList<KnownMouse> mice = new List<KnownMouse>();

        public BlackBerryMouseDriver()
        {
        }

        #region IMouseDriver2 members

        public MouseState GetState()
        {
            MouseState state = new MouseState();
            lock (sync)
            {
                foreach (KnownMouse mouse in mice)
                {
                    state.MergeBits(ConvertKnownMouseToMouseState(mouse, false));
                }
            }
            return state;
        }

        public MouseState GetState(int index)
        {
            lock (sync)
            {
                if (index >= 0 && index < mice.Count)
                {
                    return ConvertKnownMouseToMouseState(mice[index], false);
                }
            }
            return new MouseState();
        }

        public MouseState GetCursorState()
        {
            lock (sync)
            {
                if (mice.Count > 0)
                {
                    return ConvertKnownMouseToMouseState(mice[0], true);
                }
            }
            return new MouseState();
        }

        public void SetPosition(double x, double y)
        {
            Debug.Print("Futile attempt to change mouse position programatically to {0}x{1}", x, y);
        }

        #endregion

        #region Private methods

        private MouseState ConvertKnownMouseToMouseState(KnownMouse mouse, bool abs)
        {
            MouseState state = new MouseState();
            if (mouse.ID != -1 || mouse.Device != IntPtr.Zero)
            {
                state.SetIsConnected(true);
                state[Input.MouseButton.Left] = (mouse.Button & MouseButton.SCREEN_LEFT_MOUSE_BUTTON) != 0;
                state[Input.MouseButton.Middle] = (mouse.Button & MouseButton.SCREEN_MIDDLE_MOUSE_BUTTON) != 0;
                state[Input.MouseButton.Right] = (mouse.Button & MouseButton.SCREEN_RIGHT_MOUSE_BUTTON) != 0;
                state.SetScrollAbsolute(mouse.WheelHorz, mouse.WheelVert);
                Point pos = abs ? mouse.LocationAbs : mouse.LocationRel;
                state.X = pos.X;
                state.Y = pos.Y;
            }
            return state;
        }

        private int GetIndex(bool mouse, bool insert, IntPtr id)
        {
            if (!mouse)
            {
                return id.ToInt32();
            }
            if (insert)
            {
                for (int i = 0; i < mice.Count && mouse; i++)
                {
                    if (mice[i].Device == id)
                    {
                        // Mouse already exists
                        return -1;
                    }
                }
                return mice.Count;
            }
            else
            {
                // Find matching ID
                for (int i = 0; i < mice.Count; i++)
                {
                    if (mice[i].Device != id)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        #region Mouse

        private void AddMouse(IntPtr device, int index)
        {
            if (mice.Count > index)
            {
                if (mice[index].ID == index || mice[index].ID == -1)
                {
                    mice[index].Device = device;
                }
            }
            else
            {
                for (int i = mice.Count; i < index; i++)
                {
                    mice.Add(new KnownMouse());
                }
                KnownMouse mouse = new KnownMouse();
                mouse.Device = device;
                mice.Add(mouse);
            }
        }

        private void RemoveMouse(int index)
        {
            if (mice[index].ID == -1 && (mice.Count - 1) <= index)
            {
                mice.RemoveAt(index);
            }
            else
            {
                mice[index].Device = IntPtr.Zero;
            }
        }

        private void UpdateMouse(int index, int[] locRel, int[] locAbs, MouseButton buttons, int wheelV, int wheelH)
        {
            KnownMouse mouse = mice[index];
            mouse.LocationRel.X = locRel[0];
            mouse.LocationRel.Y = locRel[1];
            mouse.LocationAbs.X = locAbs[0];
            mouse.LocationAbs.Y = locAbs[1];
            mouse.WheelVert = wheelV;
            mouse.WheelHorz = wheelH;
            mouse.Button = buttons;
        }

        #endregion

        #region Touch

        private void AddTouch(int index)
        {
            if (index >= mice.Count)
            {
                for (int i = mice.Count; i < index; i++)
                {
                    mice.Add(new KnownMouse());
                }
                KnownMouse mouse = new KnownMouse();
                mouse.ID = index;
                mice.Add(mouse);
            }
        }

        private void RemoveTouch(int index)
        {
            if (mice[index].Device == IntPtr.Zero && (mice.Count - 1) <= index)
            {
                mice.RemoveAt(index);
            }
            else
            {
                mice[index].ID = -1;
            }
        }

        private void UpdateTouch(int index, int[] locRel, int[] locAbs, bool isDown)
        {
            KnownMouse mouse = mice[index];
            mouse.LocationRel.X = locRel[0];
            mouse.LocationRel.Y = locRel[1];
            mouse.LocationAbs.X = locAbs[0];
            mouse.LocationAbs.Y = locAbs[1];
            mouse.WheelVert = 0;
            mouse.WheelHorz = 0;
            mouse.Button = isDown ? MouseButton.SCREEN_LEFT_MOUSE_BUTTON : (MouseButton)0;
        }

        #endregion

        #endregion

        #region Events

        internal void ProcessEvent(IntPtr ev, bool isMouse)
        {
            int[] rel = new int[2];
            int[] abs = new int[2];

            if (isMouse)
            {
                IntPtr device;
                int buttons;
                int wheelV;
                int wheelH;
                Screen.EventGetIntPtr(ev, Screen.SCREEN_EVENT_DEVICE, out device);
                Screen.EventGetInt(ev, Screen.SCREEN_PROPERTY_BUTTONS, out buttons);
                Screen.EventGetInt(ev, Screen.SCREEN_PROPERTY_MOUSE_WHEEL, out wheelV);
                Screen.EventGetInt(ev, Screen.SCREEN_PROPERTY_MOUSE_HORIZONTAL_WHEEL, out wheelH);
                Screen.EventGetInts(ev, Screen.SCREEN_PROPERTY_SOURCE_POSITION, ref rel);
                Screen.EventGetInts(ev, Screen.SCREEN_PROPERTY_POSITION, ref abs);

                lock (sync)
                {
                    int index = GetIndex(true, false, device);
                    if (index < 0)
                    {
                        index = GetIndex(true, true, device);
                        if (index < 0)
                        {
                            return;
                        }
                        AddMouse(device, index);
                    }

                    UpdateMouse(index, rel, abs, (MouseButton)buttons, wheelV, wheelH);
                }
            }
            else
            {
                int id;
                int type;
                Screen.EventGetInt(ev, Screen.SCREEN_PROPERTY_TOUCH_ID, out id);
                Screen.EventGetInt(ev, Screen.SCREEN_PROPERTY_TYPE, out type);
                Screen.EventGetInts(ev, Screen.SCREEN_PROPERTY_SOURCE_POSITION, ref rel);
                Screen.EventGetInts(ev, Screen.SCREEN_PROPERTY_POSITION, ref abs);

                lock (sync)
                {
                    IntPtr pid = new IntPtr(id);
                    int index = GetIndex(false, false, pid);
                    bool createdTouch = false;
                    if (index < 0)
                    {
                        index = GetIndex(false, true, pid);
                        if (index < 0)
                        {
                            return;
                        }
                        AddTouch(index);
                        createdTouch = true;
                    }

                    UpdateTouch(index, rel, abs, (type != Screen.SCREEN_EVENT_MTOUCH_RELEASE) && !createdTouch);
                }
            }
        }

        internal bool HandleDeviceConnection(IntPtr device, bool connected)
        {
            int type;
            Screen.DeviceGetInt(device, Screen.SCREEN_PROPERTY_TYPE, out type);
            switch (type)
            {
                case Screen.SCREEN_EVENT_POINTER:
                    lock (sync)
                    {
                        int index = GetIndex(true, connected, device);
                        if (index >= 0)
                        {
                            if (connected)
                            {
                                AddMouse(device, index);
                            }
                            else
                            {
                                RemoveMouse(index);
                            }
                        }
                    }
                    return true;
                case Screen.SCREEN_EVENT_MTOUCH_PRETOUCH:
                case Screen.SCREEN_EVENT_MTOUCH_TOUCH:
                case Screen.SCREEN_EVENT_MTOUCH_MOVE:
                case Screen.SCREEN_EVENT_MTOUCH_RELEASE:
                    return true;
                default:
                    return false;
            }
        }

        #endregion
    }
}

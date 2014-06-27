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

namespace OpenTK.Platform.BlackBerry
{
    class BlackBerryKeyboardDriver : IKeyboardDriver2
    {
        class KnownKeyboards
        {
            public IntPtr Keyboard;
            public string Name;
            public KeyboardState State;
        }

        readonly object sync = new object();
        readonly IList<KnownKeyboards> keyboards = new List<KnownKeyboards>();

        public BlackBerryKeyboardDriver()
        {
            // Aren't able to get keystate from device, so instead of generating all keyboards at startup, just record once an event for a keyboard shows up
        }

        #region IKeyboardDriver2 members

        public KeyboardState GetState()
        {
            KeyboardState state = new KeyboardState();
            lock (sync)
            {
                foreach (KnownKeyboards keyboard in keyboards)
                {
                    state.MergeBits(keyboard.State);
                }
            }
            return state;
        }

        public KeyboardState GetState(int index)
        {
            lock (sync)
            {
                if (index >= 0 || index < keyboards.Count)
                {
                    return keyboards[index].State;
                }
            }
            return new KeyboardState();
        }

        public string GetDeviceName(int index)
        {
            lock (sync)
            {
                if (index >= 0 || index < keyboards.Count)
                {
                    return keyboards[index].Name;
                }
            }
            return string.Empty;
        }

        #endregion

        #region Private methods

        private int AddKeyboard(IntPtr keyboard)
        {
            KnownKeyboards kb = new KnownKeyboards();
            kb.Keyboard = keyboard;
            kb.Name = Screen.DeviceGetString(keyboard, Screen.SCREEN_PROPERTY_PRODUCT);
            keyboards.Add(kb);
            return keyboards.Count - 1;
        }

        private int IndexOfDevice(IntPtr keyboard)
        {
            for (int i = 0; i < keyboards.Count; i++)
            {
                if (keyboards[i].Keyboard == keyboard)
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion

        #region Events

        internal void ProcessEvent(IntPtr ev)
        {
            IntPtr device;
            Screen.EventGetIntPtr(ev, Screen.SCREEN_EVENT_DEVICE, out device);

            int flags;
            int value;
            //int mod;
            Screen.EventGetInt(ev, Screen.SCREEN_PROPERTY_KEY_FLAGS, out flags);
            Screen.EventGetInt(ev, Screen.SCREEN_PROPERTY_KEY_SYM, out value);
            //Screen.EventGetInt(ev, Screen.SCREEN_PROPERTY_KEY_MODIFIERS, out mod);

            //KeyModifiers modifiers = BlackBerryKeyMap.GetModifiers(mod);
            Key key = BlackBerryKeyMap.GetKey(value);
            lock (sync)
            {
                int index = IndexOfDevice(device);
                if (index < 0)
                {
                    index = AddKeyboard(device);
                }
                keyboards[index].State.SetKeyState(key, (flags & KeyConst.KEY_DOWN) != 0);
            }
        }

        internal bool HandleDeviceConnection(IntPtr device, bool connected)
        {
            int type;
            Screen.DeviceGetInt(device, Screen.SCREEN_PROPERTY_TYPE, out type);
            if (type == Screen.SCREEN_EVENT_KEYBOARD)
            {
                lock (sync)
                {
                    if (connected)
                    {
                        AddKeyboard(device);
                    }
                    else
                    {
                        int index = IndexOfDevice(device);
                        if (index >= 0)
                        {
                            keyboards.RemoveAt(index);
                        }
                    }
                }
            }
            return false;
        }

        #endregion
    }
}

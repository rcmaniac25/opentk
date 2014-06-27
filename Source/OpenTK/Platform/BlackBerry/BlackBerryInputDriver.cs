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
using System.Diagnostics;
using System.Threading;

using OpenTK.Input;

namespace OpenTK.Platform.BlackBerry
{
    class BlackBerryInputDriver : IInputDriver2
    {
        readonly object sync = new object();

        public const double POLL_RATE = 16.6; //~60 FPS

        bool disposed = false;
        Timer timer = null;

        readonly BlackBerryMouseDriver mouseDriver = new BlackBerryMouseDriver();
        readonly BlackBerryKeyboardDriver keyboardDriver = new BlackBerryKeyboardDriver();
        BlackBerryGamePadDriver gamepadJoystickDriver = null;

        public BlackBerryInputDriver()
        {
            Debug.Print("BlackBerry: creating input driver");
        }

        #region IInputDriver2 Members

        public IMouseDriver2 MouseDriver
        {
            get
            {
                return mouseDriver;
            }
        }

        public IKeyboardDriver2 KeyboardDriver
        {
            get
            {
                return keyboardDriver;
            }
        }

        public IGamePadDriver GamePadDriver
        {
            get
            {
                SetupGamepadJoystickDriver();
                return gamepadJoystickDriver;
            }
        }

        public IJoystickDriver2 JoystickDriver
        {
            get
            {
                SetupGamepadJoystickDriver();
                return gamepadJoystickDriver;
            }
        }

        #endregion

        internal void HandleScreenEvents(IntPtr screenEvent)
        {
            int type;
            Screen.EventGetInt(screenEvent, Screen.SCREEN_PROPERTY_TYPE, out type);
            switch (type)
            {
                case Screen.SCREEN_EVENT_POINTER:
                case Screen.SCREEN_EVENT_MTOUCH_PRETOUCH:
                case Screen.SCREEN_EVENT_MTOUCH_TOUCH:
                case Screen.SCREEN_EVENT_MTOUCH_MOVE:
                case Screen.SCREEN_EVENT_MTOUCH_RELEASE:
                    mouseDriver.ProcessEvent(screenEvent, type == Screen.SCREEN_EVENT_POINTER);
                    break;
                case Screen.SCREEN_EVENT_KEYBOARD:
                    keyboardDriver.ProcessEvent(screenEvent);
                    break;
                case Screen.SCREEN_EVENT_DEVICE:
                    int attached;
                    IntPtr device;
                    Screen.EventGetInt(screenEvent, Screen.SCREEN_PROPERTY_ATTACHED, out attached);
                    Screen.EventGetIntPtr(screenEvent, Screen.SCREEN_PROPERTY_DEVICE, out device);
                    if (!mouseDriver.HandleDeviceConnection(device, attached == 1))
                    {
                        if (!keyboardDriver.HandleDeviceConnection(device, attached == 1))
                        {
                            if (!gamepadJoystickDriver.HandleDeviceConnection(device, attached == 1))
                            {
                                Debug.Print("No input handler is handling a device event.");
                            }
                        }
                    }
                    break;
            }
        }

        #region Private members

        private void InitPollTimer()
        {
            if (!disposed && timer == null)
            {
                Debug.Print("Starting polling timer");
                TimeSpan pollRate = TimeSpan.FromMilliseconds(POLL_RATE);
                timer = new Timer(PollDrivers, null, pollRate, pollRate);
            }
        }

        private void SetupGamepadJoystickDriver()
        {
            lock (sync)
            {
                if (!disposed && gamepadJoystickDriver == null)
                {
                    BlackBerryGamePadDriver driver = new BlackBerryGamePadDriver();
                    driver.InitialPoll();
                    InitPollTimer();
                    gamepadJoystickDriver = driver;
                }
            }
        }

        private void PollDrivers(object state)
        {
            BlackBerryGamePadDriver gamepadJoyDriver;
            lock (sync)
            {
                gamepadJoyDriver = gamepadJoystickDriver;
            }
            if (gamepadJoyDriver != null)
            {
                try
                {
                    gamepadJoyDriver.Poll();
                }
                catch (Exception ex)
                {
                    Debug.Print("BlackBerry Gamepad polling threw exception: {0}", ex);
                }
            }
        }

        #endregion

        #region IDisposable Members

        void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }

                if (manual)
                {
                    //TODO
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BlackBerryInputDriver()
        {
            Dispose(false);
        }

        #endregion
    }
}

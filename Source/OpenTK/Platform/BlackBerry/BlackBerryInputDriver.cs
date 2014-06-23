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

        BlackBerryGamePadDriver gamepadDriver = null;

        public BlackBerryInputDriver()
        {
            Debug.Print("BlackBerry: creating input driver");
        }

        #region IInputDriver2 Members

        public IMouseDriver2 MouseDriver
        {
            //Need screen
            get { throw new NotImplementedException(); }
        }

        public IKeyboardDriver2 KeyboardDriver
        {
            //Need screen
            get { throw new NotImplementedException(); }
        }

        public IGamePadDriver GamePadDriver
        {
            get
            {
                if (!disposed && gamepadDriver == null)
                {
                    BlackBerryGamePadDriver driver = new BlackBerryGamePadDriver();
                    driver.InitialPoll();
                    InitPollTimer();
                    gamepadDriver = driver;
                }
                return gamepadDriver;
            }
        }

        public IJoystickDriver2 JoystickDriver
        {
            //Pollable
            get { throw new NotImplementedException(); }
        }

        #endregion

        internal void HandleScreenEvents(IntPtr screenEvent)
        {
            lock (sync)
            {
                //TODO
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

        private void PollDrivers(object state)
        {
            if (gamepadDriver != null)
            {
                try
                {
                    gamepadDriver.Poll();
                }
                catch (Exception ex)
                {
                    Debug.Print("BlackBerry Gamepad polling threw exception: {0}", ex);
                }
            }
            //TODO
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
                    /*
                    if (gamepadDriver != null)
                    {
                        gamepadDriver.Dispose();
                        gamepadDriver = null;
                    }
                     */
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

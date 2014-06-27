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
    class BlackBerryMouseDriver : IMouseDriver2
    {
        public BlackBerryMouseDriver()
        {
        }

        #region IMouseDriver2 members

        public MouseState GetState()
        {
            //TODO: combination of all mice (which is a little odd...)
            throw new NotImplementedException();
        }

        public MouseState GetState(int index)
        {
            //TODO: each unique mouse
            throw new NotImplementedException();
        }

        public MouseState GetCursorState()
        {
            //TODO: same as GetState(0), but with abs. coord
            throw new NotImplementedException();
        }

        public void SetPosition(double x, double y)
        {
            Debug.Print("Futile attempt to change mouse position programatically to {0}x{1}", x, y);
        }

        #endregion

        internal void ProcessEvent(IntPtr ev, bool isMouse)
        {
            //TODO: mouse AND touch events
        }

        internal bool HandleDeviceConnection(IntPtr device, bool connected)
        {
            //TODO
            return false;
        }
    }
}

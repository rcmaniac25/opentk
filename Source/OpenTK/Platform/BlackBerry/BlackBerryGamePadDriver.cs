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
    class BlackBerryGamePadDriver : BlackBerryIPollingInputDriverBase, IGamePadDriver, IDisposable
    {
        public BlackBerryGamePadDriver()
        {
        }

        #region IGamePadDriver members

        public GamePadState GetState(int index)
        {
            //TODO
            throw new NotImplementedException();
        }

        public GamePadCapabilities GetCapabilities(int index)
        {
            //TODO
            throw new NotImplementedException();
        }

        public string GetName(int index)
        {
            //TODO
            throw new NotImplementedException();
        }

        public bool SetVibration(int index, float left, float right)
        {
            return false;
        }

        #endregion

        #region BlackBerryIPollingInputDriverBase members

        public void InitialPoll()
        {
            //TODO
            throw new NotImplementedException();
        }

        public void Poll()
        {
            //TODO
            throw new NotImplementedException();
        }

        #endregion

        public void Dispose()
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}

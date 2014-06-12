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
using System.Text;

namespace OpenTK.Platform.BlackBerry
{
    /// \internal
    /// <summary>Describes an BlackBerry window.</summary>
    sealed class BlackBerryWindowInfo : IWindowInfo
    {
        IntPtr handle;
        bool disposed;

        #region --- Constructors ---

        /// <summary>
        /// Constructs a new instance with the specified window handle and paren.t
        /// </summary>
        /// <param name="handle">The window handle for this instance.</param>
        /// <param name="parent">The parent window of this instance (may be null).</param>
        public BlackBerryWindowInfo(IntPtr handle)
        {
            this.handle = handle;
        }

        #endregion

        #region --- Public Methods ---

        /// <summary>
        /// Gets or sets the handle of the window.
        /// </summary>
        public IntPtr Handle { get { return handle; } set { handle = value; } }

        // For compatibility with whoever thought it would be
        // a good idea to access internal APIs through reflection
        // (e.g. MonoGame)
        public IntPtr WindowHandle { get { return Handle; } set { Handle = value; } }

        #endregion

        #region --- Overrides ---

        /// <summary>Returns a System.String that represents the current window.</summary>
        /// <returns>A System.String that represents the current window.</returns>
        public override string ToString()
        {
            return String.Format("BlackBerry.WindowInfo: Handle {0}", this.Handle);
        }

        /// <summary>Checks if <c>this</c> and <c>obj</c> reference the same win32 window.</summary>
        /// <param name="obj">The object to check against.</param>
        /// <returns>True if <c>this</c> and <c>obj</c> reference the same win32 window; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is BlackBerryWindowInfo)
            {
                return ((BlackBerryWindowInfo)obj).handle.Equals(handle);
            }
            return false;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A hash code for the current <c>WinWindowInfo</c>.</returns>
        public override int GetHashCode()
        {
            return handle.GetHashCode();
        }

        #endregion

        #region --- IDisposable ---

        /// <summary>
        /// Disposes of this BlackBerryWindowInfo instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BlackBerryWindowInfo()
        {
            this.Dispose(false);
        }

        void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (this.handle != IntPtr.Zero)
                    if (Screen.DestroyWindow(this.handle) != Screen.SCREEN_SUCCESS)
                        Debug.Print("[Warning] Failed to release screen {0}.", this.handle);

                if (manual)
                {
                    //TODO: ?
                }

                disposed = true;
            }
        }

        #endregion
    }
}

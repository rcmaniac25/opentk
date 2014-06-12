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
using System.Runtime.InteropServices;

namespace OpenTK.Platform.BlackBerry
{
    using Context = IntPtr;
    using Window = IntPtr;

    class Screen
    {
        const string lib = "libscreen";

        public const int SCREEN_SUCCESS = 0;
        public const int SCREEN_ERROR = -1;

        #region --- Context ---

        const int SCREEN_APPLICATION_CONTEXT = 0;

        [DllImport(lib, EntryPoint = "screen_create_context")]
        static extern int CreateContext(out Context pctx, int flags);

        public static Context CreateContext()
        {
            Context ctx;
            if (CreateContext(out ctx, SCREEN_APPLICATION_CONTEXT) != SCREEN_SUCCESS)
            {
                return Context.Zero;
            }
            return ctx;
        }

        [DllImport(lib, EntryPoint = "screen_destroy_context")]
        public static extern int DestroyContext(Context ctx);

        //TODO

        #endregion

        #region --- Window ---

        [DllImport(lib, EntryPoint = "screen_create_window")]
        static extern int CreateWindow(out Window pwin, Context ctx);

        public static Window CreateWindow(Context ctx)
        {
            Window win;
            if (CreateWindow(out win, ctx) != SCREEN_SUCCESS)
            {
                return Window.Zero;
            }
            return win;
        }

        [DllImport(lib, EntryPoint = "screen_destroy_window")]
        public static extern int DestroyWindow(Window win);

        //TODO

        #endregion
    }
}

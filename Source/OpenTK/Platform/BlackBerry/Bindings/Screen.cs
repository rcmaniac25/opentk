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
    using Display = IntPtr;
    using Window = IntPtr;

    [StructLayout(LayoutKind.Sequential, Size = 56)]
    struct DisplayMode
    {
        public UInt32 width;
        public UInt32 height;
        public UInt32 refresh;
        public UInt32 interlaced;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public UInt32[] aspect_ratio;
        public UInt32 flags;
        public UInt32 index;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public UInt32[] reserved;
    }

    enum PixelFormat : int
    {
        SCREEN_FORMAT_BYTE = 1,
        SCREEN_FORMAT_RGBA4444 = 2,
        SCREEN_FORMAT_RGBX4444 = 3,
        SCREEN_FORMAT_RGBA5551 = 4,
        SCREEN_FORMAT_RGBX5551 = 5,
        SCREEN_FORMAT_RGB565 = 6,
        SCREEN_FORMAT_RGB888 = 7,
        SCREEN_FORMAT_RGBA8888 = 8,
        SCREEN_FORMAT_RGBX8888 = 9,
        SCREEN_FORMAT_YVU9 = 10,
        SCREEN_FORMAT_YUV420 = 11,
        SCREEN_FORMAT_NV12 = 12,
        SCREEN_FORMAT_YV12 = 13,
        SCREEN_FORMAT_UYVY = 14,
        SCREEN_FORMAT_YUY2 = 15,
        SCREEN_FORMAT_YVYU = 16,
        SCREEN_FORMAT_V422 = 17,
        SCREEN_FORMAT_AYUV = 18
    }

    class Screen
    {
        const string lib = "libscreen";

        public const int SCREEN_SUCCESS = 0;
        public const int SCREEN_ERROR = -1;

        public const int SCREEN_PROPERTY_DISPLAY_COUNT = 59;
        public const int SCREEN_PROPERTY_DISPLAYS = 60;
        public const int SCREEN_PROPERTY_ATTACHED = 64;
        public const int SCREEN_PROPERTY_FORMAT_COUNT = 70;
        public const int SCREEN_PROPERTY_FORMATS = 71;
        public const int SCREEN_PROPERTY_MODE_COUNT = 89;
        public const int SCREEN_PROPERTY_MODE = 90;

        public const int SCREEN_MODE_PREFERRED_INDEX = -1;

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

        [DllImport(lib, EntryPoint = "screen_get_context_property_iv")]
        public static extern int ContextGetInt(Context ctx, int pname, out int param);

        [DllImport(lib, EntryPoint = "screen_get_context_property_pv")]
        public static extern int ContextGetIntPtr(Context ctx, int pname, [In, Out]ref IntPtr[] param);

        //TODO

        #endregion

        #region --- Display ---

        [DllImport(lib, EntryPoint = "screen_get_display_property_iv")]
        public static extern int DisplayGetInt(Display disp, int pname, out int param);

        [DllImport(lib, EntryPoint = "screen_get_display_property_iv")]
        public static extern int DisplayGetInts(Display disp, int pname, [In, Out]ref int[] param);

        [DllImport(lib, EntryPoint = "screen_get_display_property_pv")]
        static extern int DisplayGetMode(Display disp, int pname, [In, Out]ref DisplayMode mode);

        public static bool DisplayGetMode(Display disp, out DisplayMode mode)
        {
            mode = new DisplayMode();
            return DisplayGetMode(disp, SCREEN_PROPERTY_MODE, ref mode) == SCREEN_SUCCESS;
        }

        // -----------------

        [DllImport(lib, EntryPoint = "screen_set_display_property_iv")]
        static extern int DisplaySetInt(Display disp, int pname, [In] ref int param);

        public static bool DisplaySetInt(Display disp, int pname, int param)
        {
            return DisplaySetInt(disp, pname, ref param) == SCREEN_SUCCESS;
        }

        // -----------------

        [DllImport(lib, EntryPoint = "screen_get_display_modes")]
        static extern int GetDisplayModes(Display disp, int max, [In, Out] ref DisplayMode[] param);

        public static bool GetDisplayModes(Display disp, ref DisplayMode[] param)
        {
            return GetDisplayModes(disp, param.Length, ref param) == SCREEN_SUCCESS;
        }

        public static DisplayMode[] GetDisplayModes(Display disp)
        {
            int count;
            if (DisplayGetInt(disp, SCREEN_PROPERTY_MODE_COUNT, out count) != SCREEN_SUCCESS)
            {
                return null;
            }
            DisplayMode[] modes = new DisplayMode[count];
            if (GetDisplayModes(disp, count, ref modes) != SCREEN_SUCCESS)
            {
                return null;
            }
            return modes;
        }

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

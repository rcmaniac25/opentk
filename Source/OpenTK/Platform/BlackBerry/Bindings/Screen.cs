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
    using Event = IntPtr;

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

    [Flags]
    enum MouseButton : int
    {
        SCREEN_LEFT_MOUSE_BUTTON = (1 << 0),
        SCREEN_MIDDLE_MOUSE_BUTTON = (1 << 1),
        SCREEN_RIGHT_MOUSE_BUTTON = (1 << 2)
    }

    [Flags]
    enum GamepadButtons : int
    {
        SCREEN_A_GAME_BUTTON = (1 << 0),
        SCREEN_B_GAME_BUTTON = (1 << 1),
        SCREEN_C_GAME_BUTTON = (1 << 2),
        SCREEN_X_GAME_BUTTON = (1 << 3),
        SCREEN_Y_GAME_BUTTON = (1 << 4),
        SCREEN_Z_GAME_BUTTON = (1 << 5),
        SCREEN_MENU1_GAME_BUTTON = (1 << 6),
        SCREEN_MENU2_GAME_BUTTON = (1 << 7),
        SCREEN_MENU3_GAME_BUTTON = (1 << 8),
        SCREEN_MENU4_GAME_BUTTON = (1 << 9),
        SCREEN_L1_GAME_BUTTON = (1 << 10),
        SCREEN_L2_GAME_BUTTON = (1 << 11),
        SCREEN_L3_GAME_BUTTON = (1 << 12),
        SCREEN_R1_GAME_BUTTON = (1 << 13),
        SCREEN_R2_GAME_BUTTON = (1 << 14),
        SCREEN_R3_GAME_BUTTON = (1 << 15),
        SCREEN_DPAD_UP_GAME_BUTTON = (1 << 16),
        SCREEN_DPAD_DOWN_GAME_BUTTON = (1 << 17),
        SCREEN_DPAD_LEFT_GAME_BUTTON = (1 << 18),
        SCREEN_DPAD_RIGHT_GAME_BUTTON = (1 << 19)
    }

    class Screen
    {
        const string lib = "libscreen";

        public const int SCREEN_SUCCESS = 0;
        public const int SCREEN_ERROR = -1;

        public const int SCREEN_PROPERTY_BUFFER_SIZE = 5;
        public const int SCREEN_PROPERTY_BUTTONS = 6;
        public const int SCREEN_PROPERTY_DISPLAY = 11;
        public const int SCREEN_PROPERTY_FORMAT = 14;
        public const int SCREEN_PROPERTY_ID_STRING = 20;
        public const int SCREEN_PROPERTY_FLAGS = 25;
        public const int SCREEN_PROPERTY_KEY_FLAGS = SCREEN_PROPERTY_FLAGS;
        public const int SCREEN_PROPERTY_MODIFIERS = 26;
        public const int SCREEN_PROPERTY_KEY_MODIFIERS = SCREEN_PROPERTY_MODIFIERS;
        public const int SCREEN_PROPERTY_SYM = 28;
        public const int SCREEN_PROPERTY_KEY_SYM = SCREEN_PROPERTY_SYM;
        public const int SCREEN_PROPERTY_POSITION = 35;
        public const int SCREEN_PROPERTY_ROTATION = 38;
        public const int SCREEN_PROPERTY_SOURCE_POSITION = 41;
        public const int SCREEN_PROPERTY_TRANSPARENCY = 46;
        public const int SCREEN_PROPERTY_TYPE = 47;
        public const int SCREEN_PROPERTY_USAGE = 48;
        public const int SCREEN_PROPERTY_VISIBLE = 51;
        public const int SCREEN_PROPERTY_DISPLAY_COUNT = 59;
        public const int SCREEN_PROPERTY_DISPLAYS = 60;
        public const int SCREEN_PROPERTY_ATTACHED = 64;
        public const int SCREEN_PROPERTY_FORMAT_COUNT = 70;
        public const int SCREEN_PROPERTY_FORMATS = 71;
        public const int SCREEN_PROPERTY_TOUCH_ID = 73;
        public const int SCREEN_PROPERTY_MODE_COUNT = 89;
        public const int SCREEN_PROPERTY_MODE = 90;
        public const int SCREEN_PROPERTY_MOUSE_WHEEL = 94;
        public const int SCREEN_PROPERTY_MOUSE_HORIZONTAL_WHEEL = 111;

        public const int SCREEN_MODE_PREFERRED_INDEX = -1;

        public const int SCREEN_USAGE_DISPLAY = (1 << 0);
        public const int SCREEN_USAGE_OPENGL_ES1 = (1 << 4);
        public const int SCREEN_USAGE_OPENGL_ES2 = (1 << 5);
        public const int SCREEN_USAGE_OPENGL_ES3 = (1 << 11);

        public const int SCREEN_EVENT_POINTER = 6;
        public const int SCREEN_EVENT_KEYBOARD = 7;
        public const int SCREEN_EVENT_MTOUCH_TOUCH = 100;
        public const int SCREEN_EVENT_MTOUCH_MOVE = 101;
        public const int SCREEN_EVENT_MTOUCH_RELEASE = 102;

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

        [DllImport(lib, EntryPoint = "screen_create_window_group")]
        public static extern int WindowCreateGroup(Window win, [MarshalAs(UnmanagedType.LPStr)]string name);

        [DllImport(lib, EntryPoint = "screen_create_window_buffers")]
        public static extern int WindowCreateBuffers(Window win, int count);

        #region Strings

        [DllImport(lib, EntryPoint = "screen_get_window_property_cv")]
        static extern int WindowGetString(Window win, int pname, int len, System.Text.StringBuilder param);

        public static string WindowGetString(Window win, int pname)
        {
            return WindowGetString(win, pname, 512);
        }

        public static string WindowGetString(Window win, int pname, int expectedMaxSize)
        {
            System.Text.StringBuilder bu = new System.Text.StringBuilder(expectedMaxSize);
            if (WindowGetString(win, pname, bu.Capacity, bu) == SCREEN_SUCCESS)
            {
                return bu.ToString();
            }
            return null;
        }

        // -----------------

        [DllImport(lib, EntryPoint = "screen_set_window_property_cv")]
        static extern int WindowSetString(Window win, int pname, int len, [MarshalAs(UnmanagedType.LPStr)]string param);

        public static bool WindowSetString(Window win, int pname, string param)
        {
            int len = System.Text.Encoding.Default.GetByteCount(param);
            return WindowSetString(win, pname, len, param) == SCREEN_SUCCESS;
        }

        #endregion

        #region Int

        [DllImport(lib, EntryPoint = "screen_get_window_property_iv")]
        public static extern int WindowGetInt(Window win, int pname, out int param);

        [DllImport(lib, EntryPoint = "screen_get_window_property_iv")]
        public static extern int WindowGetInts(Window win, int pname, [In, Out]ref int[] param);

        // -----------------

        [DllImport(lib, EntryPoint = "screen_set_window_property_iv")]
        static extern int WindowSetInt(Window win, int pname, [In] ref int param);

        public static bool WindowSetInt(Window win, int pname, int param)
        {
            return WindowSetInt(win, pname, ref param) == SCREEN_SUCCESS;
        }

        [DllImport(lib, EntryPoint = "screen_set_window_property_iv")]
        public static extern int WindowSetInts(Window win, int pname, [In] int[] param);

        #endregion

        #region IntPtr

        [DllImport(lib, EntryPoint = "screen_get_display_property_pv")]
        static extern int WindowSetIntPtr(Window win, int pname, [In] ref IntPtr param);

        public static bool WindowSetIntPtr(Window win, int pname, IntPtr param)
        {
            return WindowSetIntPtr(win, pname, ref param) == SCREEN_SUCCESS;
        }

        #endregion

        #endregion

        #region --- Event ---

        [DllImport(lib, EntryPoint = "screen_get_event_property_iv")]
        public static extern int EventGetInt(Event ev, int pname, out int param);

        [DllImport(lib, EntryPoint = "screen_get_event_property_iv")]
        public static extern int EventGetInts(Event ev, int pname, [In, Out]ref int[] param);

        #endregion
    }
}

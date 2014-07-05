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
    using Event = IntPtr;

    enum NavigatorWindowState : int
    {
        Fullscreen = 0,
        Thumbnail = 1,
        Invisible = 2
    }

    class BPS
    {
        const string lib = "libbps";

        public const int BPS_SUCCESS = 0;
        public const int BPS_FAILURE = -1;

        #region --- BPS ---

        [DllImport(lib, EntryPoint = "bps_initialize")]
        public static extern int Initialize();

        [DllImport(lib, EntryPoint = "bps_shutdown")]
        public static extern void Shutdown();

        [DllImport(lib, EntryPoint = "bps_get_event")]
        public static extern int GetEvent(out Event ev, int timeout_ms);

        #endregion

        #region --- Event ---

        [DllImport(lib, EntryPoint = "bps_event_get_domain")]
        public static extern int GetDomain(Event ev);

        [DllImport(lib, EntryPoint = "bps_event_get_code")]
        public static extern uint GetCode(Event ev);

        #endregion

        #region --- Navigator ---

        private const int NAVIGATOR_EXTENDED_DATA = 0x01;

        public const int NAVIGATOR_EXIT = 0x02;
        public const int NAVIGATOR_WINDOW_STATE = 0x03;
        public const int NAVIGATOR_WINDOW_ACTIVE = 0x0a;
        public const int NAVIGATOR_WINDOW_INACTIVE = 0x0b;

        [DllImport(lib, EntryPoint = "navigator_request_events")]
        static extern int NavigatorRequestEvents(int flags);

        public static bool NavigatorRequestEvents(bool extendedData)
        {
            return NavigatorRequestEvents(extendedData ? NAVIGATOR_EXTENDED_DATA : 0) == BPS_SUCCESS;
        }

        [DllImport(lib, EntryPoint = "navigator_stop_events")]
        static extern int NavigatorStopEvents(int flags);

        public static bool NavigatorStopEvents()
        {
            return NavigatorStopEvents(0) == BPS_SUCCESS;
        }

        [DllImport(lib, EntryPoint = "navigator_get_domain")]
        public static extern int NavigatorGetDomain();

        [DllImport(lib, EntryPoint = "navigator_close_window")]
        public static extern int NavigatorRequestExit();

        [DllImport(lib, EntryPoint = "navigator_rotation_lock")]
        public static extern int LockRotation(bool locked);

        [DllImport(lib, EntryPoint = "navigator_event_get_window_state")]
        public static extern NavigatorWindowState NavigatorEventWindowState(Event ev);

        [DllImport(lib, EntryPoint = "navigator_raw_write")]
        public static extern int RawWrite(IntPtr data, uint length);

        #endregion

        #region --- Screen ---

        [DllImport(lib, EntryPoint = "screen_request_events")]
        public static extern int ScreenRequestEvents(IntPtr context);

        [DllImport(lib, EntryPoint = "screen_stop_events")]
        public static extern int ScreenStopEvents(IntPtr context);

        [DllImport(lib, EntryPoint = "screen_get_domain")]
        public static extern int ScreenGetDomain();

        [DllImport(lib, EntryPoint = "screen_event_get_event")]
        public static extern IntPtr ScreenGetEvent(Event ev);

        #endregion
    }
}

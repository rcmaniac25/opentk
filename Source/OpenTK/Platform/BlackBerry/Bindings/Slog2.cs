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
    using Buffer = IntPtr;

    internal struct BufferSetConfig
    {
        public int BufferCount;
        public string BufferSetName;
        public byte Verbosity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Slog2.SLOG2_MAX_BUFFERS)]
        public BufferConfig[] Configs;
    }

    internal struct BufferConfig
    {
        public string BufferName;
        public int PageCount;
    }

    internal class Slog2
    {
        public const int SLOG2_MAX_BUFFERS = 4;

        public const int SLOG2_SUCCESS = 0;
        public const int SLOG2_FAILURE = -1;

        public const byte SLOG2_INFO = 5;
        public const byte SLOG2_DEBUG1 = 6;

        [DllImport("libslog2", EntryPoint = "slog2c")]
        public static extern int Log(IntPtr buffer, ushort code, byte severity, [MarshalAs(UnmanagedType.LPStr)]string data);

        [DllImport("libslog2", EntryPoint = "slog2_register")]
        public static extern int Register([In]ref BufferSetConfig config, [In, Out]Buffer[] handles, uint flags);

        [DllImport("libslog2", EntryPoint = "slog2_set_default_buffer")]
        public static extern Buffer SetDefaultBuffer(Buffer buffer);
    }
}

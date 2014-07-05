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
    enum PPSEncodedError
    {
        Ok = 0,
        /* Failed to allocate memory during encoding */
        NoMem = 1,
        /* Improper object/array nesting */
        BadNesting = 2,
        /* Attempt to add an invalid value to an encoder */
        InvalidValue = 3,
        /* Attempt to add a PPS attribute with no attribute name */
        MissingAttributeName = 4,
        /* Attempt to add a non-existent property of a decoder */
        NotFound = 5
    }

    [StructLayout(LayoutKind.Sequential)]
    struct PPSEncoder
    {
        public IntPtr buffer;
        public int allocated;
        public int length;
        /* Nesting level for objects.  Determines if objects are encoded PPS or
         * JSON style
         */
        public int level;
        public int options;
        /* Tracks if pps_encoder_start_object is called with a PPS object name. */
        public bool in_pps_obj;
        public PPSEncodedError status;
    }

    class PPS
    {
        const string lib = "libpps";

        [DllImport(lib, EntryPoint = "pps_encoder_initialize")]
        public static extern void EncoderInitialize([In, Out]ref PPSEncoder encoder, bool encodeJSON);

        [DllImport(lib, EntryPoint = "pps_encoder_add_string")]
        public static extern PPSEncodedError EncoderAddString([In, Out]ref PPSEncoder encoder, [MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]string value);

        [DllImport(lib, EntryPoint = "pps_encoder_buffer")]
        public static extern IntPtr EncoderBuffer([In, Out]ref PPSEncoder encoder);

        [DllImport(lib, EntryPoint = "pps_encoder_length")]
        public static extern int EncoderLength([In, Out]ref PPSEncoder encoder);

        [DllImport(lib, EntryPoint = "pps_encoder_cleanup")]
        public static extern void EncoderCleanup([In, Out]ref PPSEncoder encoder);
    }
}

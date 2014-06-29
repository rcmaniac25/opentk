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
    sealed class BlackBerryDisplayDeviceDriver : DisplayDeviceBase
    {
        readonly object display_lock = new object();

        #region Constructors

        public BlackBerryDisplayDeviceDriver()
        {
            RefreshDisplayDevices();
        }

        #endregion

        #region IDisplayDeviceDriver Members

        public override bool TryChangeResolution(DisplayDevice device, DisplayResolution resolution)
        {
            if (resolution == null)
            {
                return Screen.DisplaySetInt((IntPtr)device.Id, Screen.SCREEN_PROPERTY_MODE, Screen.SCREEN_MODE_PREFERRED_INDEX);
            }
            int index = device.AvailableResolutions.IndexOf(resolution);
            if (index >= 0)
            {
                return Screen.DisplaySetInt((IntPtr)device.Id, Screen.SCREEN_PROPERTY_MODE, index);
            }
            return false;
        }

        public override bool TryRestoreResolution(DisplayDevice device)
        {
            return TryChangeResolution(device, null);
        }

        #endregion

        #region Private Members

        private int GetBPPForFormat(int format)
        {
            switch ((PixelFormat)format)
            {
                case PixelFormat.SCREEN_FORMAT_YVU9:
                    return 9;
                case PixelFormat.SCREEN_FORMAT_NV12:
                case PixelFormat.SCREEN_FORMAT_YV12:
                case PixelFormat.SCREEN_FORMAT_YUV420:
                    return 12;
                case PixelFormat.SCREEN_FORMAT_RGBA4444:
                case PixelFormat.SCREEN_FORMAT_RGBX4444:
                case PixelFormat.SCREEN_FORMAT_RGBA5551:
                case PixelFormat.SCREEN_FORMAT_RGBX5551:
                case PixelFormat.SCREEN_FORMAT_RGB565:
                case PixelFormat.SCREEN_FORMAT_UYVY:
                case PixelFormat.SCREEN_FORMAT_YUY2:
                case PixelFormat.SCREEN_FORMAT_YVYU:
                case PixelFormat.SCREEN_FORMAT_V422:
                    return 16;
                case PixelFormat.SCREEN_FORMAT_RGB888:
                    return 24;
                case PixelFormat.SCREEN_FORMAT_RGBA8888:
                case PixelFormat.SCREEN_FORMAT_RGBX8888:
                case PixelFormat.SCREEN_FORMAT_AYUV:
                    return 32;
            }
            return -1;
        }

        private int[] GetAvaliableBPP(IntPtr disp)
        {
            int formatCount;
            if (Screen.DisplayGetInt(disp, Screen.SCREEN_PROPERTY_FORMAT_COUNT, out formatCount) != Screen.SCREEN_SUCCESS)
            {
                Debug.Print("Could not get display format count");
                return null;
            }
            int[] formats = new int[formatCount];
            if (Screen.DisplayGetInts(disp, Screen.SCREEN_PROPERTY_FORMATS, formats) != Screen.SCREEN_SUCCESS)
            {
                Debug.Print("Could not get display formats");
                return null;
            }
            List<int> bpps = new List<int>();
            foreach (int format in formats)
            {
                int bpp = GetBPPForFormat(format);
                if (bpp != -1 && !bpps.Contains(bpp))
                {
                    bpps.Add(bpp);
                }
            }
            return bpps.ToArray();
        }

        private int GetPreferredBPP(int[] bpps)
        {
            if (bpps == null || bpps.Length == 0)
            {
                return 32;
            }
            if (Array.IndexOf(bpps, 24) >= 0)
            {
                return 24;
            }
            else if (Array.IndexOf(bpps, 32) >= 0)
            {
                return 32;
            }
            else if (Array.IndexOf(bpps, 16) >= 0)
            {
                return 16;
            }
            return bpps[0];
        }

        private DisplayResolution CreateResolutionFromMode(DisplayMode mode, int bpp)
        {
            return new DisplayResolution(0, 0, (int)mode.width, (int)mode.height, bpp, mode.refresh);
        }

        private void RefreshDisplayDevices()
        {
            lock (display_lock)
            {
                IntPtr ctx = BlackBerryFactory.InitialContext;

                AvailableDevices.Clear();

                int displayCount;
                if (Screen.ContextGetInt(ctx, Screen.SCREEN_PROPERTY_DISPLAY_COUNT, out displayCount) == Screen.SCREEN_SUCCESS)
                {
                    IntPtr[] displays = new IntPtr[displayCount];
                    if (Screen.ContextGetIntPtr(ctx, Screen.SCREEN_PROPERTY_DISPLAYS, displays) == Screen.SCREEN_SUCCESS)
                    {
                        List<DisplayDevice> avDisplays = new List<DisplayDevice>();

                        int connected;
                        DisplayMode mode;
                        List<DisplayResolution> avaResolutions = new List<DisplayResolution>();
                        foreach (IntPtr disp in displays)
                        {
                            if (Screen.DisplayGetInt(disp, Screen.SCREEN_PROPERTY_ATTACHED, out connected) != Screen.SCREEN_SUCCESS || connected == 0)
                            {
                                // Screen not connected
                                continue;
                            }

                            // Get bits per pixel
                            int bpp = GetPreferredBPP(GetAvaliableBPP(disp));

                            // Determine what mode it is currently in
                            Screen.DisplayGetMode(disp, out mode);
                            DisplayResolution currentResolution = CreateResolutionFromMode(mode, bpp);

                            // Get all avaliable modes
                            avaResolutions.Clear();
                            foreach (DisplayMode dMode in Screen.GetDisplayModes(disp))
                            {
                                avaResolutions.Add(CreateResolutionFromMode(dMode, bpp));
                            }

                            // Create the display
                            avDisplays.Add(new DisplayDevice(currentResolution, avDisplays.Count == 0, avaResolutions, currentResolution.Bounds, disp));
                        }

                        AvailableDevices.AddRange(avDisplays);
                        Primary = avDisplays[0];
                    }
                    else
                    {
                        Debug.Print("Could not get displays");
                    }
                }
                else
                {
                    Debug.Print("Could not get display count");
                }
            }
        }

        #endregion
    }
}

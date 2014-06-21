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
using OpenTK.Input;

namespace OpenTK.Platform.BlackBerry
{
    static class KeyConst
    {
        // unicode.h
        public const int UNICODE_PRIVATE_USE_AREA_FIRST = 0xe000;
        public const int UNICODE_PRIVATE_USE_AREA_LAST = 0xf8ff;

        // keycodes.h
        private const int KEYMODBIT_SHIFT = 0;
        private const int KEYMODBIT_CTRL = 1;
        private const int KEYMODBIT_ALT = 2;

        public const int KEYMOD_SHIFT = (1 << KEYMODBIT_SHIFT);
        public const int KEYMOD_CTRL = (1 << KEYMODBIT_CTRL);
        public const int KEYMOD_ALT = (1 << KEYMODBIT_ALT);

        public const int KEY_DOWN = 0x00000001;
        public const int KEY_REPEAT = 0x00000002;
        public const int KEY_SYM_VALID = 0x00000040;

        public const int KEYCODE_PC_KEYS = UNICODE_PRIVATE_USE_AREA_FIRST + 0x1000;
        public const int KEYCODE_CONSUMER_KEYS = KEYCODE_PC_KEYS + 0x200;
    }

    enum BBKeys : int
    {
        KEYCODE_PAUSE = (KeyConst.KEYCODE_PC_KEYS + 0x13),
        KEYCODE_SCROLL_LOCK = (KeyConst.KEYCODE_PC_KEYS + 0x14),
        KEYCODE_PRINT = (KeyConst.KEYCODE_PC_KEYS + 0x61),

        KEYCODE_ESCAPE = (KeyConst.KEYCODE_PC_KEYS + 0x1b),
        KEYCODE_BACKSPACE = (KeyConst.KEYCODE_PC_KEYS + 0x08),
        KEYCODE_TAB = (KeyConst.KEYCODE_PC_KEYS + 0x09),
        KEYCODE_RETURN = (KeyConst.KEYCODE_PC_KEYS + 0x0d),
        KEYCODE_CAPS_LOCK = (KeyConst.KEYCODE_PC_KEYS + 0xe5),
        KEYCODE_LEFT_SHIFT = (KeyConst.KEYCODE_PC_KEYS + 0xe1),
        KEYCODE_RIGHT_SHIFT = (KeyConst.KEYCODE_PC_KEYS + 0xe2),
        KEYCODE_LEFT_CTRL = (KeyConst.KEYCODE_PC_KEYS + 0xe3),
        KEYCODE_RIGHT_CTRL = (KeyConst.KEYCODE_PC_KEYS + 0xe4),
        KEYCODE_LEFT_ALT = (KeyConst.KEYCODE_PC_KEYS + 0xe9),
        KEYCODE_RIGHT_ALT = (KeyConst.KEYCODE_PC_KEYS + 0xea),
        KEYCODE_MENU = (KeyConst.KEYCODE_PC_KEYS + 0x67),
        KEYCODE_LEFT_HYPER = (KeyConst.KEYCODE_PC_KEYS + 0xed),
        KEYCODE_RIGHT_HYPER = (KeyConst.KEYCODE_PC_KEYS + 0xee),

        KEYCODE_INSERT = (KeyConst.KEYCODE_PC_KEYS + 0x63),
        KEYCODE_HOME = (KeyConst.KEYCODE_PC_KEYS + 0x50),
        KEYCODE_PG_UP = (KeyConst.KEYCODE_PC_KEYS + 0x55),
        KEYCODE_DELETE = (KeyConst.KEYCODE_PC_KEYS + 0xff),
        KEYCODE_END = (KeyConst.KEYCODE_PC_KEYS + 0x57),
        KEYCODE_PG_DOWN = (KeyConst.KEYCODE_PC_KEYS + 0x56),
        KEYCODE_LEFT = (KeyConst.KEYCODE_PC_KEYS + 0x51),
        KEYCODE_RIGHT = (KeyConst.KEYCODE_PC_KEYS + 0x53),
        KEYCODE_UP = (KeyConst.KEYCODE_PC_KEYS + 0x52),
        KEYCODE_DOWN = (KeyConst.KEYCODE_PC_KEYS + 0x54),

        KEYCODE_NUM_LOCK = (KeyConst.KEYCODE_PC_KEYS + 0x7f),
        KEYCODE_KP_PLUS = (KeyConst.KEYCODE_PC_KEYS + 0xab),
        KEYCODE_KP_MINUS = (KeyConst.KEYCODE_PC_KEYS + 0xad),
        KEYCODE_KP_MULTIPLY = (KeyConst.KEYCODE_PC_KEYS + 0xaa),
        KEYCODE_KP_DIVIDE = (KeyConst.KEYCODE_PC_KEYS + 0xaf),
        KEYCODE_KP_ENTER = (KeyConst.KEYCODE_PC_KEYS + 0x8d),
        KEYCODE_KP_HOME = (KeyConst.KEYCODE_PC_KEYS + 0xb7),
        KEYCODE_KP_UP = (KeyConst.KEYCODE_PC_KEYS + 0xb8),
        KEYCODE_KP_PG_UP = (KeyConst.KEYCODE_PC_KEYS + 0xb9),
        KEYCODE_KP_LEFT = (KeyConst.KEYCODE_PC_KEYS + 0xb4),
        KEYCODE_KP_FIVE = (KeyConst.KEYCODE_PC_KEYS + 0xb5),
        KEYCODE_KP_RIGHT = (KeyConst.KEYCODE_PC_KEYS + 0xb6),
        KEYCODE_KP_END = (KeyConst.KEYCODE_PC_KEYS + 0xb1),
        KEYCODE_KP_DOWN = (KeyConst.KEYCODE_PC_KEYS + 0xb2),
        KEYCODE_KP_PG_DOWN = (KeyConst.KEYCODE_PC_KEYS + 0xb3),
        KEYCODE_KP_INSERT = (KeyConst.KEYCODE_PC_KEYS + 0xb0),
        KEYCODE_KP_DELETE = (KeyConst.KEYCODE_PC_KEYS + 0xae),

        KEYCODE_F1 = (KeyConst.KEYCODE_PC_KEYS + 0xbe),
        KEYCODE_F2 = (KeyConst.KEYCODE_PC_KEYS + 0xbf),
        KEYCODE_F3 = (KeyConst.KEYCODE_PC_KEYS + 0xc0),
        KEYCODE_F4 = (KeyConst.KEYCODE_PC_KEYS + 0xc1),
        KEYCODE_F5 = (KeyConst.KEYCODE_PC_KEYS + 0xc2),
        KEYCODE_F6 = (KeyConst.KEYCODE_PC_KEYS + 0xc3),
        KEYCODE_F7 = (KeyConst.KEYCODE_PC_KEYS + 0xc4),
        KEYCODE_F8 = (KeyConst.KEYCODE_PC_KEYS + 0xc5),
        KEYCODE_F9 = (KeyConst.KEYCODE_PC_KEYS + 0xc6),
        KEYCODE_F10 = (KeyConst.KEYCODE_PC_KEYS + 0xc7),
        KEYCODE_F11 = (KeyConst.KEYCODE_PC_KEYS + 0xc8),
        KEYCODE_F12 = (KeyConst.KEYCODE_PC_KEYS + 0xc9),

        KEYCODE_SLEEP = (KeyConst.KEYCODE_CONSUMER_KEYS + 0x02),

        KEYCODE_SPACE = 0x0020,
        KEYCODE_QUOTE = 0x0022,
        KEYCODE_PLUS = 0x002b,
        KEYCODE_COMMA = 0x002c,
        KEYCODE_MINUS = 0x002d,
        KEYCODE_PERIOD = 0x002e,
        KEYCODE_SLASH = 0x002f,
        KEYCODE_ZERO = 0x0030,
        KEYCODE_ONE = 0x0031,
        KEYCODE_TWO = 0x0032,
        KEYCODE_THREE = 0x0033,
        KEYCODE_FOUR = 0x0034,
        KEYCODE_FIVE = 0x0035,
        KEYCODE_SIX = 0x0036,
        KEYCODE_SEVEN = 0x0037,
        KEYCODE_EIGHT = 0x0038,
        KEYCODE_NINE = 0x0039,
        KEYCODE_SEMICOLON = 0x003b,
        KEYCODE_CAPITAL_A = 0x0041,
        KEYCODE_CAPITAL_B = 0x0042,
        KEYCODE_CAPITAL_C = 0x0043,
        KEYCODE_CAPITAL_D = 0x0044,
        KEYCODE_CAPITAL_E = 0x0045,
        KEYCODE_CAPITAL_F = 0x0046,
        KEYCODE_CAPITAL_G = 0x0047,
        KEYCODE_CAPITAL_H = 0x0048,
        KEYCODE_CAPITAL_I = 0x0049,
        KEYCODE_CAPITAL_J = 0x004a,
        KEYCODE_CAPITAL_K = 0x004b,
        KEYCODE_CAPITAL_L = 0x004c,
        KEYCODE_CAPITAL_M = 0x004d,
        KEYCODE_CAPITAL_N = 0x004e,
        KEYCODE_CAPITAL_O = 0x004f,
        KEYCODE_CAPITAL_P = 0x0050,
        KEYCODE_CAPITAL_Q = 0x0051,
        KEYCODE_CAPITAL_R = 0x0052,
        KEYCODE_CAPITAL_S = 0x0053,
        KEYCODE_CAPITAL_T = 0x0054,
        KEYCODE_CAPITAL_U = 0x0055,
        KEYCODE_CAPITAL_V = 0x0056,
        KEYCODE_CAPITAL_W = 0x0057,
        KEYCODE_CAPITAL_X = 0x0058,
        KEYCODE_CAPITAL_Y = 0x0059,
        KEYCODE_CAPITAL_Z = 0x005a,
        KEYCODE_LEFT_BRACKET = 0x005b,
        KEYCODE_BACK_SLASH = 0x005c,
        KEYCODE_RIGHT_BRACKET = 0x005d,
        KEYCODE_GRAVE = 0x0060,
        KEYCODE_A = 0x0061,
        KEYCODE_B = 0x0062,
        KEYCODE_C = 0x0063,
        KEYCODE_D = 0x0064,
        KEYCODE_E = 0x0065,
        KEYCODE_F = 0x0066,
        KEYCODE_G = 0x0067,
        KEYCODE_H = 0x0068,
        KEYCODE_I = 0x0069,
        KEYCODE_J = 0x006a,
        KEYCODE_K = 0x006b,
        KEYCODE_L = 0x006c,
        KEYCODE_M = 0x006d,
        KEYCODE_N = 0x006e,
        KEYCODE_O = 0x006f,
        KEYCODE_P = 0x0070,
        KEYCODE_Q = 0x0071,
        KEYCODE_R = 0x0072,
        KEYCODE_S = 0x0073,
        KEYCODE_T = 0x0074,
        KEYCODE_U = 0x0075,
        KEYCODE_V = 0x0076,
        KEYCODE_W = 0x0077,
        KEYCODE_X = 0x0078,
        KEYCODE_Y = 0x0079,
        KEYCODE_Z = 0x007a,
    }

    static class BlackBerryKeyMap
    {
        public static Key GetKey(int code)
        {
            switch ((BBKeys)code)
            {
                // Modifiers
                case BBKeys.KEYCODE_LEFT_SHIFT: return Key.ShiftLeft;
                case BBKeys.KEYCODE_RIGHT_SHIFT: return Key.ShiftRight;
                case BBKeys.KEYCODE_LEFT_CTRL: return Key.ControlLeft;
                case BBKeys.KEYCODE_RIGHT_CTRL: return Key.ControlRight;
                case BBKeys.KEYCODE_LEFT_ALT: return Key.AltLeft;
                case BBKeys.KEYCODE_RIGHT_ALT: return Key.AltRight;
                case BBKeys.KEYCODE_LEFT_HYPER: return Key.WinLeft;
                case BBKeys.KEYCODE_RIGHT_HYPER: return Key.WinRight;
                case BBKeys.KEYCODE_MENU: return Key.Menu;

                // Function keys
                case BBKeys.KEYCODE_F1: return Key.F1;
                case BBKeys.KEYCODE_F2: return Key.F2;
                case BBKeys.KEYCODE_F3: return Key.F3;
                case BBKeys.KEYCODE_F4: return Key.F4;
                case BBKeys.KEYCODE_F5: return Key.F5;
                case BBKeys.KEYCODE_F6: return Key.F6;
                case BBKeys.KEYCODE_F7: return Key.F7;
                case BBKeys.KEYCODE_F8: return Key.F8;
                case BBKeys.KEYCODE_F9: return Key.F9;
                case BBKeys.KEYCODE_F10: return Key.F10;
                case BBKeys.KEYCODE_F11: return Key.F11;
                case BBKeys.KEYCODE_F12: return Key.F12;

                // Direction arrows
                case BBKeys.KEYCODE_LEFT: return Key.Left;
                case BBKeys.KEYCODE_RIGHT: return Key.Right;
                case BBKeys.KEYCODE_UP: return Key.Up;
                case BBKeys.KEYCODE_DOWN: return Key.Down;

                case BBKeys.KEYCODE_RETURN: return Key.Enter;
                case BBKeys.KEYCODE_ESCAPE: return Key.Escape;
                case BBKeys.KEYCODE_SPACE: return Key.Space;
                case BBKeys.KEYCODE_TAB: return Key.Tab;
                case BBKeys.KEYCODE_BACKSPACE: return Key.BackSpace;
                case BBKeys.KEYCODE_INSERT: return Key.Insert;
                case BBKeys.KEYCODE_DELETE: return Key.Delete;
                case BBKeys.KEYCODE_PG_UP: return Key.PageUp;
                case BBKeys.KEYCODE_PG_DOWN: return Key.PageDown;
                case BBKeys.KEYCODE_HOME: return Key.Home;
                case BBKeys.KEYCODE_END: return Key.End;
                case BBKeys.KEYCODE_CAPS_LOCK: return Key.CapsLock;
                case BBKeys.KEYCODE_SCROLL_LOCK: return Key.ScrollLock;
                case BBKeys.KEYCODE_PRINT: return Key.PrintScreen;
                case BBKeys.KEYCODE_PAUSE: return Key.Pause;
                case BBKeys.KEYCODE_NUM_LOCK: return Key.NumLock;

                // Special keys
                //case BBKeys.: return Key.Clear;
                case BBKeys.KEYCODE_SLEEP: return Key.Sleep;

                // Keypad keys
                case BBKeys.KEYCODE_KP_INSERT: return Key.Keypad0;
                case BBKeys.KEYCODE_KP_END: return Key.Keypad1;
                case BBKeys.KEYCODE_KP_DOWN: return Key.Keypad2;
                case BBKeys.KEYCODE_KP_PG_DOWN: return Key.Keypad3;
                case BBKeys.KEYCODE_KP_LEFT: return Key.Keypad4;
                case BBKeys.KEYCODE_KP_FIVE: return Key.Keypad5;
                case BBKeys.KEYCODE_KP_RIGHT: return Key.Keypad6;
                case BBKeys.KEYCODE_KP_HOME: return Key.Keypad7;
                case BBKeys.KEYCODE_KP_UP: return Key.Keypad8;
                case BBKeys.KEYCODE_KP_PG_UP: return Key.Keypad9;
                case BBKeys.KEYCODE_KP_DIVIDE: return Key.KeypadDivide;
                case BBKeys.KEYCODE_KP_MULTIPLY: return Key.KeypadMultiply;
                case BBKeys.KEYCODE_KP_MINUS: return Key.KeypadMinus;
                case BBKeys.KEYCODE_KP_PLUS: return Key.KeypadPlus;
                case BBKeys.KEYCODE_KP_DELETE: return Key.KeypadPeriod;
                case BBKeys.KEYCODE_KP_ENTER: return Key.KeypadEnter;

                // Letters (A-Z)
                case BBKeys.KEYCODE_CAPITAL_A:
                case BBKeys.KEYCODE_A: return Key.A;
                case BBKeys.KEYCODE_CAPITAL_B:
                case BBKeys.KEYCODE_B: return Key.B;
                case BBKeys.KEYCODE_CAPITAL_C:
                case BBKeys.KEYCODE_C: return Key.C;
                case BBKeys.KEYCODE_CAPITAL_D:
                case BBKeys.KEYCODE_D: return Key.D;
                case BBKeys.KEYCODE_CAPITAL_E:
                case BBKeys.KEYCODE_E: return Key.E;
                case BBKeys.KEYCODE_CAPITAL_F:
                case BBKeys.KEYCODE_F: return Key.F;
                case BBKeys.KEYCODE_CAPITAL_G:
                case BBKeys.KEYCODE_G: return Key.G;
                case BBKeys.KEYCODE_CAPITAL_H:
                case BBKeys.KEYCODE_H: return Key.H;
                case BBKeys.KEYCODE_CAPITAL_I:
                case BBKeys.KEYCODE_I: return Key.I;
                case BBKeys.KEYCODE_CAPITAL_J:
                case BBKeys.KEYCODE_J: return Key.J;
                case BBKeys.KEYCODE_CAPITAL_K:
                case BBKeys.KEYCODE_K: return Key.K;
                case BBKeys.KEYCODE_CAPITAL_L:
                case BBKeys.KEYCODE_L: return Key.L;
                case BBKeys.KEYCODE_CAPITAL_M:
                case BBKeys.KEYCODE_M: return Key.M;
                case BBKeys.KEYCODE_CAPITAL_N:
                case BBKeys.KEYCODE_N: return Key.N;
                case BBKeys.KEYCODE_CAPITAL_O:
                case BBKeys.KEYCODE_O: return Key.O;
                case BBKeys.KEYCODE_CAPITAL_P:
                case BBKeys.KEYCODE_P: return Key.P;
                case BBKeys.KEYCODE_CAPITAL_Q:
                case BBKeys.KEYCODE_Q: return Key.Q;
                case BBKeys.KEYCODE_CAPITAL_R:
                case BBKeys.KEYCODE_R: return Key.R;
                case BBKeys.KEYCODE_CAPITAL_S:
                case BBKeys.KEYCODE_S: return Key.S;
                case BBKeys.KEYCODE_CAPITAL_T:
                case BBKeys.KEYCODE_T: return Key.T;
                case BBKeys.KEYCODE_CAPITAL_U:
                case BBKeys.KEYCODE_U: return Key.U;
                case BBKeys.KEYCODE_CAPITAL_V:
                case BBKeys.KEYCODE_V: return Key.V;
                case BBKeys.KEYCODE_CAPITAL_W:
                case BBKeys.KEYCODE_W: return Key.W;
                case BBKeys.KEYCODE_CAPITAL_X:
                case BBKeys.KEYCODE_X: return Key.X;
                case BBKeys.KEYCODE_CAPITAL_Y:
                case BBKeys.KEYCODE_Y: return Key.Y;
                case BBKeys.KEYCODE_CAPITAL_Z:
                case BBKeys.KEYCODE_Z: return Key.Z;
                    
                // Number keys (0-9)
                case BBKeys.KEYCODE_ZERO: return Key.Number0;
                case BBKeys.KEYCODE_ONE: return Key.Number1;
                case BBKeys.KEYCODE_TWO: return Key.Number2;
                case BBKeys.KEYCODE_THREE: return Key.Number3;
                case BBKeys.KEYCODE_FOUR: return Key.Number4;
                case BBKeys.KEYCODE_FIVE: return Key.Number5;
                case BBKeys.KEYCODE_SIX: return Key.Number6;
                case BBKeys.KEYCODE_SEVEN: return Key.Number7;
                case BBKeys.KEYCODE_EIGHT: return Key.Number8;
                case BBKeys.KEYCODE_NINE: return Key.Number9;

                // Symbols
                case BBKeys.KEYCODE_GRAVE: return Key.Grave;
                case BBKeys.KEYCODE_MINUS: return Key.Minus;
                case BBKeys.KEYCODE_PLUS: return Key.Plus;
                case BBKeys.KEYCODE_LEFT_BRACKET: return Key.BracketLeft;
                case BBKeys.KEYCODE_RIGHT_BRACKET: return Key.BracketRight;
                case BBKeys.KEYCODE_SEMICOLON: return Key.Semicolon;
                case BBKeys.KEYCODE_QUOTE: return Key.Quote;
                case BBKeys.KEYCODE_COMMA: return Key.Comma;
                case BBKeys.KEYCODE_PERIOD: return Key.Period;
                case BBKeys.KEYCODE_SLASH: return Key.Slash;
                case BBKeys.KEYCODE_BACK_SLASH: return Key.BackSlash;

                default: return Key.Unknown;
            }
        }

        public static KeyModifiers GetModifiers(int mod)
        {
            KeyModifiers result = 0;
            result |= (mod & KeyConst.KEYMOD_ALT) != 0 ? KeyModifiers.Alt : 0;
            result |= (mod & KeyConst.KEYMOD_CTRL) != 0 ? KeyModifiers.Control : 0;
            result |= (mod & KeyConst.KEYMOD_SHIFT) != 0 ? KeyModifiers.Shift : 0;
            return result;
        }

        public static char GetAscii(int code)
        {
            if (code >= KeyConst.KEYCODE_PC_KEYS && code <= KeyConst.UNICODE_PRIVATE_USE_AREA_LAST)
            {
                switch ((BBKeys)code)
                {
                    case BBKeys.KEYCODE_BACKSPACE: return '\b';
                    case BBKeys.KEYCODE_TAB: return '\t';
                    case BBKeys.KEYCODE_KP_ENTER:
                    case BBKeys.KEYCODE_RETURN: return '\n';
                    case BBKeys.KEYCODE_ESCAPE: return (char)0x001B;
                }
            }
            return (char)code;
        }
    }
}

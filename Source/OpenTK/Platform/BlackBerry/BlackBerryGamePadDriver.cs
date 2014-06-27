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
    class BlackBerryGamePadDriver : IGamePadDriver, IJoystickDriver2
    {
        #region Known gamepads

        // Could use GamePadConfigurationDatabase, but don't have a full GUID to be able to map it correctly

        class KnownGamepad
        {
            public int VID;
            public int DID;

            public string Vendor;
            public string Name;
            public bool HasAnalogTriggers;
            public bool HasVibrate;

            public KnownGamepad(int vid, int did, string vendor, string name, bool analog, bool vibrate)
            {
                this.VID = vid;
                this.DID = did;
                this.Vendor = vendor;
                this.Name = name;
                this.HasAnalogTriggers = analog;
                this.HasVibrate = vibrate;
            }
        }

        static readonly IList<KnownGamepad> knownGamepads = new List<KnownGamepad>(11);

        static BlackBerryGamePadDriver()
        {
            // Built off https://github.com/rcmaniac25/DOOM-3-BFG/blob/blackberry/neo/sys/qnx/qnx_input.cpp
            knownGamepads.Add(new KnownGamepad(0x057E, 0x0306, "Nintendo", "Wii Remote", false, true));
            knownGamepads.Add(new KnownGamepad(0x20D6, 0x0DAD, "MOGA", "Pro", true, false));
            //knownGamepads.Add(new KnownGamepad(0x20D6, ?, "MOGA", "Hero Power", true, true));
            knownGamepads.Add(new KnownGamepad(0x20D6, 0x6271, "MOGA", "Pro Power", true, true));
            knownGamepads.Add(new KnownGamepad(0x1038, 0x1412, "SteelSeries", "FREE", false, false));
            knownGamepads.Add(new KnownGamepad(0x25B6, 0x0001, "Fructel", "Gametel", false, false));
            knownGamepads.Add(new KnownGamepad(0x046D, 0xC21D, "Logitech", "F310", true, false));
            //knownGamepads.Add(new KnownGamepad(0x046D, ?, "Logitech", "F710", true, true)); // DID may be 0xC21F based on GamePadConfigurationDatabase. Need confirmation
            knownGamepads.Add(new KnownGamepad(0x045E, 0x028E, "Microsoft", "Xbox 360 Controller", true, true));
            knownGamepads.Add(new KnownGamepad(0x045E, 0x0291, "Microsoft", "Xbox 360 Controller Wireless", true, true));
            knownGamepads.Add(new KnownGamepad(0x1689, 0xFD01, "Razer", "Sabertooth", true, true));
        }

        private static int AddKnownGamepad(int vid, int did, string vendor, string name, bool analog)
        {
            for (int i = 0; i < knownGamepads.Count; i++)
            {
                if (knownGamepads[i].VID == vid && knownGamepads[i].DID == did)
                {
                    Debug.Print("Attempted to add a known gamepad");
                    return i;
                }
            }
            knownGamepads.Add(new KnownGamepad(vid, did, vendor, name, analog, false));
            Debug.Print("Adding new gamepad: {0}-{1}, ({2}, {3}), HasAnalog: {4}", vid, did, vendor, name, analog);
            return knownGamepads.Count - 1;
        }

        private static bool IsRealKnownGamepad(int index)
        {
            return index < 9;
        }

        private static void UpdateKnownGamepad(int index, bool analog)
        {
            if (IsRealKnownGamepad(index))
            {
                Debug.Print("Attempted to change an \"actual\" known gamepad");
                return;
            }
            if (!knownGamepads[index].HasAnalogTriggers && analog)
            {
                Debug.Print("Updating gamepad {0}-{1} with analog trigger", knownGamepads[index].DID, knownGamepads[index].VID);
                knownGamepads[index].HasAnalogTriggers |= analog;
            }
        }

        #endregion

        class GamepadDevice
        {
            public IntPtr Handle;
            public string Id;
            public string Product;

            public int KnownInfo = -1;

            public int ButtonCount;
            public int AnalogCount;
            public GamepadButtons Buttons;
            public int[] Analog0 = new int[3];
            public int[] Analog1 = new int[3];
        }

        readonly object sync = new object();
        int packet_id;
        readonly IList<GamepadDevice> gamepads = new List<GamepadDevice>(4);
        readonly IList<GamepadDevice> joysticks = new List<GamepadDevice>(4);

        public BlackBerryGamePadDriver()
        {
        }

        #region IGamePadDriver members

        public GamePadState GetState(int index)
        {
            GamePadState state = new GamePadState();
            lock (sync)
            {
                if (index >= 0 && index < gamepads.Count)
                {
                    GamepadDevice device = gamepads[index];
                    state.SetConnected(true);
                    state.SetPacketNumber(packet_id);

                    Buttons pressedButtons = TranslateButtons(device.Buttons);
                    state.SetButton(pressedButtons, true);
                    state.SetButton(~pressedButtons, false);

                    if (device.KnownInfo >= 0 ? knownGamepads[device.KnownInfo].HasAnalogTriggers : false)
                    {
                        state.SetTriggers((byte)device.Analog0[2], (byte)device.Analog1[2]);
                    }

                    if (device.AnalogCount > 0)
                    {
                        state.SetAxis(GamePadAxes.LeftX, (short)(device.Analog0[0] * 128));
                        state.SetAxis(GamePadAxes.LeftY, (short)(device.Analog0[1] * 128));
                    }
                    if (device.AnalogCount > 1)
                    {
                        state.SetAxis(GamePadAxes.RightX, (short)(device.Analog1[0] * 128));
                        state.SetAxis(GamePadAxes.RightY, (short)(device.Analog1[1] * 128));
                    }
                }
            }
            return state;
        }

        public GamePadCapabilities GetCapabilities(int index)
        {
            lock (sync)
            {
                if (index >= 0 && index < gamepads.Count)
                {
                    GamepadDevice device = gamepads[index];
                    bool hasAnalogTriggers = device.KnownInfo >= 0 ? knownGamepads[device.KnownInfo].HasAnalogTriggers : false;
                    return new GamePadCapabilities(GamePadType.GamePad,
                        GenerateAxis(device.AnalogCount, hasAnalogTriggers),
                        GenerateButtons(device.ButtonCount, device.AnalogCount, hasAnalogTriggers),
                        true);
                }
            }
            return new GamePadCapabilities();
        }

        public string GetName(int index)
        {
            lock (sync)
            {
                if (index >= 0 && index < gamepads.Count)
                {
                    return gamepads[index].Product;
                }
            }
            return string.Empty;
        }

        public bool SetVibration(int index, float left, float right)
        {
            return false;
        }

        #endregion

        #region IJoystickDriver2

        JoystickState IJoystickDriver2.GetState(int index)
        {
            JoystickState state = new JoystickState();
            lock (sync)
            {
                if (index >= 0 && index < joysticks.Count)
                {
                    GamepadDevice device = joysticks[index];
                    state.SetIsConnected(true);
                    state.SetPacketNumber(packet_id);

                    TranslateJoystickButtons(device.Buttons, device.ButtonCount, ref state);

                    if (device.AnalogCount > 0)
                    {
                        state.SetAxis(JoystickAxis.Axis0, (short)(device.Analog0[0] * 128));
                        state.SetAxis(JoystickAxis.Axis1, (short)(device.Analog0[1] * 128));
                    }
                }
            }
            return state;
        }

        JoystickCapabilities IJoystickDriver2.GetCapabilities(int index)
        {
            lock (sync)
            {
                if (index >= 0 && index < joysticks.Count)
                {
                    GamepadDevice device = joysticks[index];
                    return new JoystickCapabilities(device.AnalogCount > 0 ? 2 : 0, device.ButtonCount, 0, true);
                }
            }
            return new JoystickCapabilities();
        }

        public Guid GetGuid(int index)
        {
            lock (sync)
            {
                if (index >= 0 && index < joysticks.Count)
                {
                    return new Guid(joysticks[index].Id);
                }
            }
            return Guid.Empty;
        }

        #endregion

        #region Private members

        #region Buttons/Joysticks

        #region Gamepad

        private Buttons TranslateButtons(GamepadButtons buttons)
        {
            Buttons but = (Buttons)0;
            if ((buttons & GamepadButtons.SCREEN_A_GAME_BUTTON) == GamepadButtons.SCREEN_A_GAME_BUTTON)
            {
                but |= Buttons.A;
            }
            if ((buttons & GamepadButtons.SCREEN_B_GAME_BUTTON) == GamepadButtons.SCREEN_B_GAME_BUTTON)
            {
                but |= Buttons.A;
            }
            if ((buttons & GamepadButtons.SCREEN_X_GAME_BUTTON) == GamepadButtons.SCREEN_X_GAME_BUTTON)
            {
                but |= Buttons.X;
            }
            if ((buttons & GamepadButtons.SCREEN_Y_GAME_BUTTON) == GamepadButtons.SCREEN_Y_GAME_BUTTON)
            {
                but |= Buttons.Y;
            }

            if ((buttons & GamepadButtons.SCREEN_DPAD_DOWN_GAME_BUTTON) == GamepadButtons.SCREEN_DPAD_DOWN_GAME_BUTTON)
            {
                but |= Buttons.DPadDown;
            }
            if ((buttons & GamepadButtons.SCREEN_DPAD_LEFT_GAME_BUTTON) == GamepadButtons.SCREEN_DPAD_LEFT_GAME_BUTTON)
            {
                but |= Buttons.DPadLeft;
            }
            if ((buttons & GamepadButtons.SCREEN_DPAD_RIGHT_GAME_BUTTON) == GamepadButtons.SCREEN_DPAD_RIGHT_GAME_BUTTON)
            {
                but |= Buttons.DPadRight;
            }
            if ((buttons & GamepadButtons.SCREEN_DPAD_UP_GAME_BUTTON) == GamepadButtons.SCREEN_DPAD_UP_GAME_BUTTON)
            {
                but |= Buttons.DPadUp;
            }

            if ((buttons & GamepadButtons.SCREEN_MENU1_GAME_BUTTON) == GamepadButtons.SCREEN_MENU1_GAME_BUTTON)
            {
                but |= Buttons.Back;
            }
            if ((buttons & GamepadButtons.SCREEN_MENU2_GAME_BUTTON) == GamepadButtons.SCREEN_MENU2_GAME_BUTTON)
            {
                but |= Buttons.Start;
            }
            if ((buttons & GamepadButtons.SCREEN_MENU3_GAME_BUTTON) == GamepadButtons.SCREEN_MENU3_GAME_BUTTON)
            {
                but |= Buttons.Home;
            }

            if ((buttons & GamepadButtons.SCREEN_R1_GAME_BUTTON) == GamepadButtons.SCREEN_R1_GAME_BUTTON)
            {
                but |= Buttons.RightShoulder;
            }
            if ((buttons & GamepadButtons.SCREEN_L1_GAME_BUTTON) == GamepadButtons.SCREEN_L1_GAME_BUTTON)
            {
                but |= Buttons.LeftShoulder;
            }
            if ((buttons & GamepadButtons.SCREEN_R2_GAME_BUTTON) == GamepadButtons.SCREEN_R2_GAME_BUTTON)
            {
                but |= Buttons.RightTrigger;
            }
            if ((buttons & GamepadButtons.SCREEN_L2_GAME_BUTTON) == GamepadButtons.SCREEN_L2_GAME_BUTTON)
            {
                but |= Buttons.LeftTrigger;
            }
            if ((buttons & GamepadButtons.SCREEN_R3_GAME_BUTTON) == GamepadButtons.SCREEN_R3_GAME_BUTTON)
            {
                but |= Buttons.RightStick;
            }
            if ((buttons & GamepadButtons.SCREEN_L3_GAME_BUTTON) == GamepadButtons.SCREEN_L3_GAME_BUTTON)
            {
                but |= Buttons.LeftStick;
            }
            return but;
        }

        private GamePadAxes GenerateAxis(int analogCount, bool hasTriggers)
        {
            GamePadAxes axis = (GamePadAxes)0;
            if (hasTriggers)
            {
                axis |= GamePadAxes.LeftTrigger;
                axis |= GamePadAxes.RightTrigger;
            }
            if (analogCount > 0)
            {
                axis |= GamePadAxes.LeftX;
                axis |= GamePadAxes.LeftY;
            }
            if (analogCount > 1)
            {
                axis |= GamePadAxes.RightX;
                axis |= GamePadAxes.RightY;
            }
            return axis;
        }

        private Buttons GenerateButtons(int buttonCount, int analogCount, bool hasTriggers)
        {
            Buttons but = Buttons.DPadDown | Buttons.DPadLeft | Buttons.DPadRight | Buttons.DPadUp | Buttons.Start | Buttons.Back | Buttons.A | Buttons.B; // Every controller that BlackBerry supports has these buttons
            int buttonsNeeded = buttonCount - 8;
            if (buttonCount > 0)
            {
                // Build up buttons based on assumptions... not the best idea, but follows common gamepad setup and symmetry.
                if (buttonCount > 2)
                {
                    but |= Buttons.X | Buttons.Y;
                    buttonCount -= 2;
                }
                if (buttonCount > 2)
                {
                    but |= Buttons.RightShoulder | Buttons.LeftShoulder;
                    buttonCount -= 2;
                }
                if (buttonCount > 0)
                {
                    if (analogCount > 0)
                    {
                        but |= Buttons.LeftStick;
                        buttonCount--;
                    }
                    if (buttonCount > 0 && analogCount > 1)
                    {
                        but |= Buttons.RightStick;
                        buttonCount--;
                    }
                    if (buttonCount > 0 && !hasTriggers)
                    {
                        but |= Buttons.LeftTrigger;
                        buttonCount--;
                        if (buttonCount > 0)
                        {
                            but |= Buttons.RightTrigger;
                            buttonCount--;
                        }
                    }
                    if (buttonCount > 0)
                    {
                        but |= Buttons.Home;
                        buttonCount--;
                    }
                }
            }
            return but;
        }

        #endregion

        #region Joystick

        private void TranslateJoystickButtons(GamepadButtons buttons, int count, ref JoystickState state)
        {
            for (int i = 0; i < count; i++) // XXX Is this correct? Would think that button count wouldn't be 1 << i and instead would need to go through every button.
            {
                GamepadButtons buttonTest = (GamepadButtons)(1 << i);
                state.SetButton((JoystickButton)i, (buttons & buttonTest) == buttonTest);
            }
        }

        #endregion

        #endregion

        #region Devices

        private GamepadDevice GenerateDevice(IntPtr handle)
        {
            GamepadDevice device = new GamepadDevice();
            device.Handle = handle;
            device.Id = Screen.DeviceGetString(handle, Screen.SCREEN_PROPERTY_ID_STRING, 64);
            device.Product = Screen.DeviceGetString(handle, Screen.SCREEN_PROPERTY_PRODUCT);
            Screen.DeviceGetInt(handle, Screen.SCREEN_PROPERTY_BUTTON_COUNT, out device.ButtonCount);

            return device;
        }

        private int AddGamePad(IntPtr gamepad)
        {
            GamepadDevice device = GenerateDevice(gamepad);

            string tmp = device.Id.Substring(device.Id.IndexOf('-') + 1);
            int vid = int.Parse(tmp.Substring(0, tmp.IndexOf('-')), System.Globalization.NumberStyles.HexNumber);
            tmp = tmp.Substring(tmp.IndexOf('-') + 1);
            int did = int.Parse(tmp.Substring(0, tmp.IndexOf('-')), System.Globalization.NumberStyles.HexNumber);
            for (int i = 0; i < knownGamepads.Count; i++)
            {
                if (knownGamepads[i].VID == vid && knownGamepads[i].DID == did)
                {
                    device.KnownInfo = i;
                    break;
                }
            }
            if (device.KnownInfo < 0)
            {
                device.KnownInfo = AddKnownGamepad(vid, did, Screen.DeviceGetString(gamepad, Screen.SCREEN_PROPERTY_VENDOR), device.Product, false);
            }

            gamepads.Add(device);

            return UpdateGamepadJoystick(gamepad, gamepads.Count - 1, true, false, gamepads); // Use false for "isGamepad" so it doesn't change anything at this point
        }

        private int AddJoystick(IntPtr joystick)
        {
            GamepadDevice device = GenerateDevice(joystick);

            // Attempt to parse Guid
            Guid guid;
            try
            {
                guid = new Guid(device.Id);
            }
            catch
            {
                string tmp = device.Id.Substring(device.Id.IndexOf('-') + 1);
                ushort vid = ushort.Parse(tmp.Substring(0, tmp.IndexOf('-')), System.Globalization.NumberStyles.HexNumber);
                tmp = tmp.Substring(tmp.IndexOf('-') + 1);
                ushort did = ushort.Parse(tmp.Substring(0, tmp.IndexOf('-')), System.Globalization.NumberStyles.HexNumber);
                guid = new Guid(0, vid, did, 0, 0, 0, 0, 0, 0, 0, 0);
            }
            device.Id = guid.ToString();

            joysticks.Add(device);

            return UpdateGamepadJoystick(joystick, joysticks.Count - 1, true, false, joysticks);
        }

        private void IndexOfDevice(IntPtr device, ref int knownIndex, IList<GamepadDevice> devices)
        {
            for (int i = 0; knownIndex < 0 && i < devices.Count; i++)
            {
                if (devices[i].Handle == device)
                {
                    knownIndex = i;
                }
            }
        }

        private int UpdateGamepadJoystick(IntPtr gamepad, int knownIndex, bool updateAnalogCount, bool isGamepad, IList<GamepadDevice> devices)
        {
            IndexOfDevice(gamepad, ref knownIndex, devices);

            if (knownIndex < 0)
            {
                return knownIndex;
            }

            GamepadDevice device = devices[knownIndex];

            int buttons;
            Screen.DeviceGetInt(gamepad, Screen.SCREEN_PROPERTY_BUTTONS, out buttons);
            device.Buttons = (GamepadButtons)buttons;
            if (Screen.DeviceGetInts(gamepad, Screen.SCREEN_PROPERTY_ANALOG0, ref device.Analog0) == Screen.SCREEN_SUCCESS && updateAnalogCount)
            {
                device.AnalogCount++;
            }
            if (Screen.DeviceGetInts(gamepad, Screen.SCREEN_PROPERTY_ANALOG1, ref device.Analog1) == Screen.SCREEN_SUCCESS && updateAnalogCount)
            {
                device.AnalogCount++;
            }
            if (isGamepad && device.KnownInfo >= 0 && device.AnalogCount > 0 && 
                !IsRealKnownGamepad(device.KnownInfo) && !knownGamepads[device.KnownInfo].HasAnalogTriggers)
            {
                // Might want to update gamepad info
                bool hasAnalogTriggers = device.Analog0[2] != 0 || (device.AnalogCount > 1 ? device.Analog1[2] != 0 : false);
                UpdateKnownGamepad(device.KnownInfo, hasAnalogTriggers);
            }
            return knownIndex;
        }

        #endregion

        #region Polling/Events

        internal void InitialPoll()
        {
            lock (sync)
            {
                int count;
                Screen.ContextGetInt(BlackBerryFactory.InitialContext, Screen.SCREEN_PROPERTY_DEVICE_COUNT, out count);
                IntPtr[] devices = new IntPtr[count];
                Screen.ContextGetIntPtr(BlackBerryFactory.InitialContext, Screen.SCREEN_PROPERTY_DEVICES, ref devices);

                packet_id = 0;
                gamepads.Clear();
                joysticks.Clear();
                foreach (IntPtr device in devices)
                {
                    int type;
                    Screen.DeviceGetInt(device, Screen.SCREEN_PROPERTY_TYPE, out type);

                    if (type == Screen.SCREEN_EVENT_GAMEPAD)
                    {
                        AddGamePad(device);
                    }
                    else if (type == Screen.SCREEN_EVENT_JOYSTICK)
                    {
                        AddJoystick(device);
                    }
                }
            }
        }

        internal void Poll()
        {
            lock (sync)
            {
                int count;
                Screen.ContextGetInt(BlackBerryFactory.InitialContext, Screen.SCREEN_PROPERTY_DEVICE_COUNT, out count);
                IntPtr[] devices = new IntPtr[count];
                Screen.ContextGetIntPtr(BlackBerryFactory.InitialContext, Screen.SCREEN_PROPERTY_DEVICES, ref devices);

                packet_id++;
                List<int> processedGamepads = new List<int>(gamepads.Count);
                List<int> processedJoysticks = new List<int>(joysticks.Count);
                foreach (IntPtr device in devices)
                {
                    int type;
                    Screen.DeviceGetInt(device, Screen.SCREEN_PROPERTY_TYPE, out type);

                    if (type == Screen.SCREEN_EVENT_GAMEPAD)
                    {
                        int index = UpdateGamepadJoystick(device, -1, false, true, gamepads);
                        if (index < 0)
                        {
                            processedGamepads.Add(AddGamePad(device));
                        }
                        else
                        {
                            processedGamepads.Add(index);
                        }
                    }
                    else if (type == Screen.SCREEN_EVENT_JOYSTICK)
                    {
                        int index = UpdateGamepadJoystick(device, -1, false, false, joysticks);
                        if (index < 0)
                        {
                            processedJoysticks.Add(AddJoystick(device));
                        }
                        else
                        {
                            processedJoysticks.Add(index);
                        }
                    }
                }
                if (processedGamepads.Count != gamepads.Count)
                {
                    for (int i = gamepads.Count - 1; i >= 0; i--)
                    {
                        if (!processedGamepads.Contains(i))
                        {
                            gamepads.RemoveAt(i);
                        }
                    }
                }
                if (processedJoysticks.Count != joysticks.Count)
                {
                    for (int i = joysticks.Count - 1; i >= 0; i--)
                    {
                        if (!processedJoysticks.Contains(i))
                        {
                            joysticks.RemoveAt(i);
                        }
                    }
                }
            }
        }

        internal bool HandleDeviceConnection(IntPtr device, bool connected)
        {
            int type;
            Screen.DeviceGetInt(device, Screen.SCREEN_PROPERTY_TYPE, out type);
            if (type == Screen.SCREEN_EVENT_GAMEPAD || type == Screen.SCREEN_EVENT_JOYSTICK)
            {
                int index = -1;
                lock (sync)
                {
                    if (type == Screen.SCREEN_EVENT_GAMEPAD)
                    {
                        if (connected)
                        {
                            index = AddGamePad(device);
                        }
                        else
                        {
                            IndexOfDevice(device, ref index, gamepads);
                            if (index != -1)
                            {
                                gamepads.RemoveAt(index);
                            }
                        }
                    }
                    else
                    {
                        if (connected)
                        {
                            index = AddJoystick(device);
                        }
                        else
                        {
                            IndexOfDevice(device, ref index, joysticks);
                            if (index != -1)
                            {
                                joysticks.RemoveAt(index);
                            }
                        }
                    }
                }
                return index != -1;
            }
            return false;
        }

        #endregion

        #endregion
    }
}

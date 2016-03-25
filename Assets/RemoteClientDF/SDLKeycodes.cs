using System;

/*
    SDL - Simple DirectMedia Layer
    Copyright (C) 1997-2009 Sam Lantinga

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

    Sam Lantinga
    slouken@libsdl.org
*/

// Fake - only structs. Shamelessly pilfered from the SDL library.
// Needed for processing its event types without polluting our namespaces with C garbage

namespace SDL
{
    /** What we really want is a mapping of every raw key on the keyboard.
     *  To support international keyboards, we use the range 0xA1 - 0xFF
     *  as international virtual keycodes.  We'll follow in the footsteps of X11...
     *  @brief The names of the keys
     */
    enum Key
    {
        /** @name ASCII mapped keysyms
         *  The keyboard syms have been cleverly chosen to map to ASCII
         */
        /*@{*/
        K_UNKNOWN = 0,
        K_FIRST = 0,
        K_BACKSPACE = 8,
        K_TAB = 9,
        K_CLEAR = 12,
        K_RETURN = 13,
        K_PAUSE = 19,
        K_ESCAPE = 27,
        K_SPACE = 32,
        K_EXCLAIM = 33,
        K_QUOTEDBL = 34,
        K_HASH = 35,
        K_DOLLAR = 36,
        K_AMPERSAND = 38,
        K_QUOTE = 39,
        K_LEFTPAREN = 40,
        K_RIGHTPAREN = 41,
        K_ASTERISK = 42,
        K_PLUS = 43,
        K_COMMA = 44,
        K_MINUS = 45,
        K_PERIOD = 46,
        K_SLASH = 47,
        K_0 = 48,
        K_1 = 49,
        K_2 = 50,
        K_3 = 51,
        K_4 = 52,
        K_5 = 53,
        K_6 = 54,
        K_7 = 55,
        K_8 = 56,
        K_9 = 57,
        K_COLON = 58,
        K_SEMICOLON = 59,
        K_LESS = 60,
        K_EQUALS = 61,
        K_GREATER = 62,
        K_QUESTION = 63,
        K_AT = 64,
        /*
           Skip uppercase letters
         */
        K_LEFTBRACKET = 91,
        K_BACKSLASH = 92,
        K_RIGHTBRACKET = 93,
        K_CARET = 94,
        K_UNDERSCORE = 95,
        K_BACKQUOTE = 96,
        K_a = 97,
        K_b = 98,
        K_c = 99,
        K_d = 100,
        K_e = 101,
        K_f = 102,
        K_g = 103,
        K_h = 104,
        K_i = 105,
        K_j = 106,
        K_k = 107,
        K_l = 108,
        K_m = 109,
        K_n = 110,
        K_o = 111,
        K_p = 112,
        K_q = 113,
        K_r = 114,
        K_s = 115,
        K_t = 116,
        K_u = 117,
        K_v = 118,
        K_w = 119,
        K_x = 120,
        K_y = 121,
        K_z = 122,
        K_DELETE = 127,
        /* End of ASCII mapped keysyms */
        /*@}*/

        /** @name International keyboard syms */
        /*@{*/
        K_WORLD_0 = 160,      /* 0xA0 */
        K_WORLD_1 = 161,
        K_WORLD_2 = 162,
        K_WORLD_3 = 163,
        K_WORLD_4 = 164,
        K_WORLD_5 = 165,
        K_WORLD_6 = 166,
        K_WORLD_7 = 167,
        K_WORLD_8 = 168,
        K_WORLD_9 = 169,
        K_WORLD_10 = 170,
        K_WORLD_11 = 171,
        K_WORLD_12 = 172,
        K_WORLD_13 = 173,
        K_WORLD_14 = 174,
        K_WORLD_15 = 175,
        K_WORLD_16 = 176,
        K_WORLD_17 = 177,
        K_WORLD_18 = 178,
        K_WORLD_19 = 179,
        K_WORLD_20 = 180,
        K_WORLD_21 = 181,
        K_WORLD_22 = 182,
        K_WORLD_23 = 183,
        K_WORLD_24 = 184,
        K_WORLD_25 = 185,
        K_WORLD_26 = 186,
        K_WORLD_27 = 187,
        K_WORLD_28 = 188,
        K_WORLD_29 = 189,
        K_WORLD_30 = 190,
        K_WORLD_31 = 191,
        K_WORLD_32 = 192,
        K_WORLD_33 = 193,
        K_WORLD_34 = 194,
        K_WORLD_35 = 195,
        K_WORLD_36 = 196,
        K_WORLD_37 = 197,
        K_WORLD_38 = 198,
        K_WORLD_39 = 199,
        K_WORLD_40 = 200,
        K_WORLD_41 = 201,
        K_WORLD_42 = 202,
        K_WORLD_43 = 203,
        K_WORLD_44 = 204,
        K_WORLD_45 = 205,
        K_WORLD_46 = 206,
        K_WORLD_47 = 207,
        K_WORLD_48 = 208,
        K_WORLD_49 = 209,
        K_WORLD_50 = 210,
        K_WORLD_51 = 211,
        K_WORLD_52 = 212,
        K_WORLD_53 = 213,
        K_WORLD_54 = 214,
        K_WORLD_55 = 215,
        K_WORLD_56 = 216,
        K_WORLD_57 = 217,
        K_WORLD_58 = 218,
        K_WORLD_59 = 219,
        K_WORLD_60 = 220,
        K_WORLD_61 = 221,
        K_WORLD_62 = 222,
        K_WORLD_63 = 223,
        K_WORLD_64 = 224,
        K_WORLD_65 = 225,
        K_WORLD_66 = 226,
        K_WORLD_67 = 227,
        K_WORLD_68 = 228,
        K_WORLD_69 = 229,
        K_WORLD_70 = 230,
        K_WORLD_71 = 231,
        K_WORLD_72 = 232,
        K_WORLD_73 = 233,
        K_WORLD_74 = 234,
        K_WORLD_75 = 235,
        K_WORLD_76 = 236,
        K_WORLD_77 = 237,
        K_WORLD_78 = 238,
        K_WORLD_79 = 239,
        K_WORLD_80 = 240,
        K_WORLD_81 = 241,
        K_WORLD_82 = 242,
        K_WORLD_83 = 243,
        K_WORLD_84 = 244,
        K_WORLD_85 = 245,
        K_WORLD_86 = 246,
        K_WORLD_87 = 247,
        K_WORLD_88 = 248,
        K_WORLD_89 = 249,
        K_WORLD_90 = 250,
        K_WORLD_91 = 251,
        K_WORLD_92 = 252,
        K_WORLD_93 = 253,
        K_WORLD_94 = 254,
        K_WORLD_95 = 255,      /* 0xFF */
        /*@}*/

        /** @name Numeric keypad */
        /*@{*/
        K_KP0 = 256,
        K_KP1 = 257,
        K_KP2 = 258,
        K_KP3 = 259,
        K_KP4 = 260,
        K_KP5 = 261,
        K_KP6 = 262,
        K_KP7 = 263,
        K_KP8 = 264,
        K_KP9 = 265,
        K_KP_PERIOD = 266,
        K_KP_DIVIDE = 267,
        K_KP_MULTIPLY = 268,
        K_KP_MINUS = 269,
        K_KP_PLUS = 270,
        K_KP_ENTER = 271,
        K_KP_EQUALS = 272,
        /*@}*/

        /** @name Arrows + Home/End pad */
        /*@{*/
        K_UP = 273,
        K_DOWN = 274,
        K_RIGHT = 275,
        K_LEFT = 276,
        K_INSERT = 277,
        K_HOME = 278,
        K_END = 279,
        K_PAGEUP = 280,
        K_PAGEDOWN = 281,
        /*@}*/

        /** @name Function keys */
        /*@{*/
        K_F1 = 282,
        K_F2 = 283,
        K_F3 = 284,
        K_F4 = 285,
        K_F5 = 286,
        K_F6 = 287,
        K_F7 = 288,
        K_F8 = 289,
        K_F9 = 290,
        K_F10 = 291,
        K_F11 = 292,
        K_F12 = 293,
        K_F13 = 294,
        K_F14 = 295,
        K_F15 = 296,
        /*@}*/

        /** @name Key state modifier keys */
        /*@{*/
        K_NUMLOCK = 300,
        K_CAPSLOCK = 301,
        K_SCROLLOCK = 302,
        K_RSHIFT = 303,
        K_LSHIFT = 304,
        K_RCTRL = 305,
        K_LCTRL = 306,
        K_RALT = 307,
        K_LALT = 308,
        K_RMETA = 309,
        K_LMETA = 310,
        K_LSUPER = 311,      /**< Left "Windows" key */
        K_RSUPER = 312,      /**< Right "Windows" key */
        K_MODE = 313,      /**< "Alt Gr" key */
        K_COMPOSE = 314,      /**< Multi-key compose key */
        /*@}*/

        /** @name Miscellaneous function keys */
        /*@{*/
        K_HELP = 315,
        K_PRINT = 316,
        K_SYSREQ = 317,
        K_BREAK = 318,
        K_MENU = 319,
        K_POWER = 320,      /**< Power Macintosh power key */
        K_EURO = 321,      /**< Some european keyboards */
        K_UNDO = 322,      /**< Atari keyboard has Undo */
        /*@}*/

        /* Add any other keys here */

        K_LAST
    };

    /** Enumeration of valid key mods (possibly OR'd together) */
    [Flags]
    enum Mod
    {
        KMOD_NONE = 0x0000,
        KMOD_LSHIFT = 0x0001,
        KMOD_RSHIFT = 0x0002,
        KMOD_LCTRL = 0x0040,
        KMOD_RCTRL = 0x0080,
        KMOD_LALT = 0x0100,
        KMOD_RALT = 0x0200,
        KMOD_LMETA = 0x0400,
        KMOD_RMETA = 0x0800,
        KMOD_NUM = 0x1000,
        KMOD_CAPS = 0x2000,
        KMOD_MODE = 0x4000,
        KMOD_RESERVED = 0x8000,
        KMOD_CTRL = (KMOD_LCTRL | KMOD_RCTRL),
        KMOD_SHIFT = (KMOD_LSHIFT | KMOD_RSHIFT),
        KMOD_ALT = (KMOD_LALT | KMOD_RALT),
        KMOD_META = (KMOD_LMETA | KMOD_RMETA)
    };

    public class SDLKeycodes
    {
        static Key ConvertKeycode(UnityEngine.KeyCode inputCode)
        {
            switch (inputCode)
            {
                case UnityEngine.KeyCode.None:
                    return Key.K_UNKNOWN;
                case UnityEngine.KeyCode.Backspace:
                    return Key.K_BACKSPACE;
                case UnityEngine.KeyCode.Delete:
                    return Key.K_DELETE;
                case UnityEngine.KeyCode.Tab:
                    return Key.K_TAB;
                case UnityEngine.KeyCode.Clear:
                    return Key.K_UNKNOWN;
                case UnityEngine.KeyCode.Return:
                    return Key.K_RETURN;
                case UnityEngine.KeyCode.Pause:
                    return Key.K_PAUSE;
                case UnityEngine.KeyCode.Escape:
                    return Key.K_ESCAPE;
                case UnityEngine.KeyCode.Space:
                    return Key.K_SPACE;
                case UnityEngine.KeyCode.Keypad0:
                    return Key.K_KP0;
                case UnityEngine.KeyCode.Keypad1:
                    return Key.K_KP1;
                case UnityEngine.KeyCode.Keypad2:
                    return Key.K_KP2;
                case UnityEngine.KeyCode.Keypad3:
                    return Key.K_KP3;
                case UnityEngine.KeyCode.Keypad4:
                    return Key.K_KP4;
                case UnityEngine.KeyCode.Keypad5:
                    return Key.K_KP5;
                case UnityEngine.KeyCode.Keypad6:
                    return Key.K_KP6;
                case UnityEngine.KeyCode.Keypad7:
                    return Key.K_KP7;
                case UnityEngine.KeyCode.Keypad8:
                    return Key.K_KP8;
                case UnityEngine.KeyCode.Keypad9:
                    return Key.K_KP9;
                case UnityEngine.KeyCode.KeypadPeriod:
                    return Key.K_KP_PERIOD;
                case UnityEngine.KeyCode.KeypadDivide:
                    return Key.K_KP_DIVIDE;
                case UnityEngine.KeyCode.KeypadMultiply:
                    return Key.K_KP_MULTIPLY;
                case UnityEngine.KeyCode.KeypadMinus:
                    return Key.K_KP_MINUS;
                case UnityEngine.KeyCode.KeypadPlus:
                    return Key.K_KP_PLUS;
                case UnityEngine.KeyCode.KeypadEnter:
                    return Key.K_KP_ENTER;
                case UnityEngine.KeyCode.KeypadEquals:
                    return Key.K_KP_EQUALS;
                case UnityEngine.KeyCode.UpArrow:
                    return Key.K_UP;
                case UnityEngine.KeyCode.DownArrow:
                    return Key.K_DOWN;
                case UnityEngine.KeyCode.RightArrow:
                    return Key.K_RIGHT;
                case UnityEngine.KeyCode.LeftArrow:
                    return Key.K_LEFT;
                case UnityEngine.KeyCode.Insert:
                    return Key.K_INSERT;
                case UnityEngine.KeyCode.Home:
                    return Key.K_HOME;
                case UnityEngine.KeyCode.End:
                    return Key.K_END;
                case UnityEngine.KeyCode.PageUp:
                    return Key.K_PAGEUP;
                case UnityEngine.KeyCode.PageDown:
                    return Key.K_PAGEDOWN;
                case UnityEngine.KeyCode.F1:
                    return Key.K_F1;
                case UnityEngine.KeyCode.F2:
                    return Key.K_F2;
                case UnityEngine.KeyCode.F3:
                    return Key.K_F3;
                case UnityEngine.KeyCode.F4:
                    return Key.K_F4;
                case UnityEngine.KeyCode.F5:
                    return Key.K_F5;
                case UnityEngine.KeyCode.F6:
                    return Key.K_F6;
                case UnityEngine.KeyCode.F7:
                    return Key.K_F7;
                case UnityEngine.KeyCode.F8:
                    return Key.K_F8;
                case UnityEngine.KeyCode.F9:
                    return Key.K_F9;
                case UnityEngine.KeyCode.F10:
                    return Key.K_F10;
                case UnityEngine.KeyCode.F11:
                    return Key.K_F11;
                case UnityEngine.KeyCode.F12:
                    return Key.K_F12;
                case UnityEngine.KeyCode.F13:
                    return Key.K_F13;
                case UnityEngine.KeyCode.F14:
                    return Key.K_F14;
                case UnityEngine.KeyCode.F15:
                    return Key.K_F15;
                case UnityEngine.KeyCode.Alpha0:
                    return Key.K_0;
                case UnityEngine.KeyCode.Alpha1:
                    return Key.K_1;
                case UnityEngine.KeyCode.Alpha2:
                    break;
                case UnityEngine.KeyCode.Alpha3:
                    break;
                case UnityEngine.KeyCode.Alpha4:
                    break;
                case UnityEngine.KeyCode.Alpha5:
                    break;
                case UnityEngine.KeyCode.Alpha6:
                    break;
                case UnityEngine.KeyCode.Alpha7:
                    break;
                case UnityEngine.KeyCode.Alpha8:
                    break;
                case UnityEngine.KeyCode.Alpha9:
                    break;
                case UnityEngine.KeyCode.Exclaim:
                    break;
                case UnityEngine.KeyCode.DoubleQuote:
                    break;
                case UnityEngine.KeyCode.Hash:
                    break;
                case UnityEngine.KeyCode.Dollar:
                    break;
                case UnityEngine.KeyCode.Ampersand:
                    break;
                case UnityEngine.KeyCode.Quote:
                    break;
                case UnityEngine.KeyCode.LeftParen:
                    break;
                case UnityEngine.KeyCode.RightParen:
                    break;
                case UnityEngine.KeyCode.Asterisk:
                    break;
                case UnityEngine.KeyCode.Plus:
                    break;
                case UnityEngine.KeyCode.Comma:
                    break;
                case UnityEngine.KeyCode.Minus:
                    break;
                case UnityEngine.KeyCode.Period:
                    break;
                case UnityEngine.KeyCode.Slash:
                    break;
                case UnityEngine.KeyCode.Colon:
                    break;
                case UnityEngine.KeyCode.Semicolon:
                    break;
                case UnityEngine.KeyCode.Less:
                    break;
                case UnityEngine.KeyCode.Equals:
                    break;
                case UnityEngine.KeyCode.Greater:
                    break;
                case UnityEngine.KeyCode.Question:
                    break;
                case UnityEngine.KeyCode.At:
                    break;
                case UnityEngine.KeyCode.LeftBracket:
                    break;
                case UnityEngine.KeyCode.Backslash:
                    break;
                case UnityEngine.KeyCode.RightBracket:
                    break;
                case UnityEngine.KeyCode.Caret:
                    break;
                case UnityEngine.KeyCode.Underscore:
                    break;
                case UnityEngine.KeyCode.BackQuote:
                    break;
                case UnityEngine.KeyCode.A:
                    break;
                case UnityEngine.KeyCode.B:
                    break;
                case UnityEngine.KeyCode.C:
                    break;
                case UnityEngine.KeyCode.D:
                    break;
                case UnityEngine.KeyCode.E:
                    break;
                case UnityEngine.KeyCode.F:
                    break;
                case UnityEngine.KeyCode.G:
                    break;
                case UnityEngine.KeyCode.H:
                    break;
                case UnityEngine.KeyCode.I:
                    break;
                case UnityEngine.KeyCode.J:
                    break;
                case UnityEngine.KeyCode.K:
                    break;
                case UnityEngine.KeyCode.L:
                    break;
                case UnityEngine.KeyCode.M:
                    break;
                case UnityEngine.KeyCode.N:
                    break;
                case UnityEngine.KeyCode.O:
                    break;
                case UnityEngine.KeyCode.P:
                    break;
                case UnityEngine.KeyCode.Q:
                    break;
                case UnityEngine.KeyCode.R:
                    break;
                case UnityEngine.KeyCode.S:
                    break;
                case UnityEngine.KeyCode.T:
                    break;
                case UnityEngine.KeyCode.U:
                    break;
                case UnityEngine.KeyCode.V:
                    break;
                case UnityEngine.KeyCode.W:
                    break;
                case UnityEngine.KeyCode.X:
                    break;
                case UnityEngine.KeyCode.Y:
                    break;
                case UnityEngine.KeyCode.Z:
                    break;
                case UnityEngine.KeyCode.Numlock:
                    break;
                case UnityEngine.KeyCode.CapsLock:
                    break;
                case UnityEngine.KeyCode.ScrollLock:
                    break;
                case UnityEngine.KeyCode.RightShift:
                    break;
                case UnityEngine.KeyCode.LeftShift:
                    break;
                case UnityEngine.KeyCode.RightControl:
                    break;
                case UnityEngine.KeyCode.LeftControl:
                    break;
                case UnityEngine.KeyCode.RightAlt:
                    break;
                case UnityEngine.KeyCode.LeftAlt:
                    break;
                case UnityEngine.KeyCode.LeftCommand:
                    break;
                case UnityEngine.KeyCode.LeftWindows:
                    break;
                case UnityEngine.KeyCode.RightCommand:
                    break;
                case UnityEngine.KeyCode.RightWindows:
                    break;
                case UnityEngine.KeyCode.AltGr:
                    break;
                case UnityEngine.KeyCode.Help:
                    break;
                case UnityEngine.KeyCode.Print:
                    break;
                case UnityEngine.KeyCode.SysReq:
                    break;
                case UnityEngine.KeyCode.Break:
                    break;
                case UnityEngine.KeyCode.Menu:
                    break;
                case UnityEngine.KeyCode.Mouse0:
                    break;
                case UnityEngine.KeyCode.Mouse1:
                    break;
                case UnityEngine.KeyCode.Mouse2:
                    break;
                case UnityEngine.KeyCode.Mouse3:
                    break;
                case UnityEngine.KeyCode.Mouse4:
                    break;
                case UnityEngine.KeyCode.Mouse5:
                    break;
                case UnityEngine.KeyCode.Mouse6:
                    break;
                case UnityEngine.KeyCode.JoystickButton0:
                    break;
                case UnityEngine.KeyCode.JoystickButton1:
                    break;
                case UnityEngine.KeyCode.JoystickButton2:
                    break;
                case UnityEngine.KeyCode.JoystickButton3:
                    break;
                case UnityEngine.KeyCode.JoystickButton4:
                    break;
                case UnityEngine.KeyCode.JoystickButton5:
                    break;
                case UnityEngine.KeyCode.JoystickButton6:
                    break;
                case UnityEngine.KeyCode.JoystickButton7:
                    break;
                case UnityEngine.KeyCode.JoystickButton8:
                    break;
                case UnityEngine.KeyCode.JoystickButton9:
                    break;
                case UnityEngine.KeyCode.JoystickButton10:
                    break;
                case UnityEngine.KeyCode.JoystickButton11:
                    break;
                case UnityEngine.KeyCode.JoystickButton12:
                    break;
                case UnityEngine.KeyCode.JoystickButton13:
                    break;
                case UnityEngine.KeyCode.JoystickButton14:
                    break;
                case UnityEngine.KeyCode.JoystickButton15:
                    break;
                case UnityEngine.KeyCode.JoystickButton16:
                    break;
                case UnityEngine.KeyCode.JoystickButton17:
                    break;
                case UnityEngine.KeyCode.JoystickButton18:
                    break;
                case UnityEngine.KeyCode.JoystickButton19:
                    break;
                case UnityEngine.KeyCode.Joystick1Button0:
                    break;
                case UnityEngine.KeyCode.Joystick1Button1:
                    break;
                case UnityEngine.KeyCode.Joystick1Button2:
                    break;
                case UnityEngine.KeyCode.Joystick1Button3:
                    break;
                case UnityEngine.KeyCode.Joystick1Button4:
                    break;
                case UnityEngine.KeyCode.Joystick1Button5:
                    break;
                case UnityEngine.KeyCode.Joystick1Button6:
                    break;
                case UnityEngine.KeyCode.Joystick1Button7:
                    break;
                case UnityEngine.KeyCode.Joystick1Button8:
                    break;
                case UnityEngine.KeyCode.Joystick1Button9:
                    break;
                case UnityEngine.KeyCode.Joystick1Button10:
                    break;
                case UnityEngine.KeyCode.Joystick1Button11:
                    break;
                case UnityEngine.KeyCode.Joystick1Button12:
                    break;
                case UnityEngine.KeyCode.Joystick1Button13:
                    break;
                case UnityEngine.KeyCode.Joystick1Button14:
                    break;
                case UnityEngine.KeyCode.Joystick1Button15:
                    break;
                case UnityEngine.KeyCode.Joystick1Button16:
                    break;
                case UnityEngine.KeyCode.Joystick1Button17:
                    break;
                case UnityEngine.KeyCode.Joystick1Button18:
                    break;
                case UnityEngine.KeyCode.Joystick1Button19:
                    break;
                case UnityEngine.KeyCode.Joystick2Button0:
                    break;
                case UnityEngine.KeyCode.Joystick2Button1:
                    break;
                case UnityEngine.KeyCode.Joystick2Button2:
                    break;
                case UnityEngine.KeyCode.Joystick2Button3:
                    break;
                case UnityEngine.KeyCode.Joystick2Button4:
                    break;
                case UnityEngine.KeyCode.Joystick2Button5:
                    break;
                case UnityEngine.KeyCode.Joystick2Button6:
                    break;
                case UnityEngine.KeyCode.Joystick2Button7:
                    break;
                case UnityEngine.KeyCode.Joystick2Button8:
                    break;
                case UnityEngine.KeyCode.Joystick2Button9:
                    break;
                case UnityEngine.KeyCode.Joystick2Button10:
                    break;
                case UnityEngine.KeyCode.Joystick2Button11:
                    break;
                case UnityEngine.KeyCode.Joystick2Button12:
                    break;
                case UnityEngine.KeyCode.Joystick2Button13:
                    break;
                case UnityEngine.KeyCode.Joystick2Button14:
                    break;
                case UnityEngine.KeyCode.Joystick2Button15:
                    break;
                case UnityEngine.KeyCode.Joystick2Button16:
                    break;
                case UnityEngine.KeyCode.Joystick2Button17:
                    break;
                case UnityEngine.KeyCode.Joystick2Button18:
                    break;
                case UnityEngine.KeyCode.Joystick2Button19:
                    break;
                case UnityEngine.KeyCode.Joystick3Button0:
                    break;
                case UnityEngine.KeyCode.Joystick3Button1:
                    break;
                case UnityEngine.KeyCode.Joystick3Button2:
                    break;
                case UnityEngine.KeyCode.Joystick3Button3:
                    break;
                case UnityEngine.KeyCode.Joystick3Button4:
                    break;
                case UnityEngine.KeyCode.Joystick3Button5:
                    break;
                case UnityEngine.KeyCode.Joystick3Button6:
                    break;
                case UnityEngine.KeyCode.Joystick3Button7:
                    break;
                case UnityEngine.KeyCode.Joystick3Button8:
                    break;
                case UnityEngine.KeyCode.Joystick3Button9:
                    break;
                case UnityEngine.KeyCode.Joystick3Button10:
                    break;
                case UnityEngine.KeyCode.Joystick3Button11:
                    break;
                case UnityEngine.KeyCode.Joystick3Button12:
                    break;
                case UnityEngine.KeyCode.Joystick3Button13:
                    break;
                case UnityEngine.KeyCode.Joystick3Button14:
                    break;
                case UnityEngine.KeyCode.Joystick3Button15:
                    break;
                case UnityEngine.KeyCode.Joystick3Button16:
                    break;
                case UnityEngine.KeyCode.Joystick3Button17:
                    break;
                case UnityEngine.KeyCode.Joystick3Button18:
                    break;
                case UnityEngine.KeyCode.Joystick3Button19:
                    break;
                case UnityEngine.KeyCode.Joystick4Button0:
                    break;
                case UnityEngine.KeyCode.Joystick4Button1:
                    break;
                case UnityEngine.KeyCode.Joystick4Button2:
                    break;
                case UnityEngine.KeyCode.Joystick4Button3:
                    break;
                case UnityEngine.KeyCode.Joystick4Button4:
                    break;
                case UnityEngine.KeyCode.Joystick4Button5:
                    break;
                case UnityEngine.KeyCode.Joystick4Button6:
                    break;
                case UnityEngine.KeyCode.Joystick4Button7:
                    break;
                case UnityEngine.KeyCode.Joystick4Button8:
                    break;
                case UnityEngine.KeyCode.Joystick4Button9:
                    break;
                case UnityEngine.KeyCode.Joystick4Button10:
                    break;
                case UnityEngine.KeyCode.Joystick4Button11:
                    break;
                case UnityEngine.KeyCode.Joystick4Button12:
                    break;
                case UnityEngine.KeyCode.Joystick4Button13:
                    break;
                case UnityEngine.KeyCode.Joystick4Button14:
                    break;
                case UnityEngine.KeyCode.Joystick4Button15:
                    break;
                case UnityEngine.KeyCode.Joystick4Button16:
                    break;
                case UnityEngine.KeyCode.Joystick4Button17:
                    break;
                case UnityEngine.KeyCode.Joystick4Button18:
                    break;
                case UnityEngine.KeyCode.Joystick4Button19:
                    break;
                case UnityEngine.KeyCode.Joystick5Button0:
                    break;
                case UnityEngine.KeyCode.Joystick5Button1:
                    break;
                case UnityEngine.KeyCode.Joystick5Button2:
                    break;
                case UnityEngine.KeyCode.Joystick5Button3:
                    break;
                case UnityEngine.KeyCode.Joystick5Button4:
                    break;
                case UnityEngine.KeyCode.Joystick5Button5:
                    break;
                case UnityEngine.KeyCode.Joystick5Button6:
                    break;
                case UnityEngine.KeyCode.Joystick5Button7:
                    break;
                case UnityEngine.KeyCode.Joystick5Button8:
                    break;
                case UnityEngine.KeyCode.Joystick5Button9:
                    break;
                case UnityEngine.KeyCode.Joystick5Button10:
                    break;
                case UnityEngine.KeyCode.Joystick5Button11:
                    break;
                case UnityEngine.KeyCode.Joystick5Button12:
                    break;
                case UnityEngine.KeyCode.Joystick5Button13:
                    break;
                case UnityEngine.KeyCode.Joystick5Button14:
                    break;
                case UnityEngine.KeyCode.Joystick5Button15:
                    break;
                case UnityEngine.KeyCode.Joystick5Button16:
                    break;
                case UnityEngine.KeyCode.Joystick5Button17:
                    break;
                case UnityEngine.KeyCode.Joystick5Button18:
                    break;
                case UnityEngine.KeyCode.Joystick5Button19:
                    break;
                case UnityEngine.KeyCode.Joystick6Button0:
                    break;
                case UnityEngine.KeyCode.Joystick6Button1:
                    break;
                case UnityEngine.KeyCode.Joystick6Button2:
                    break;
                case UnityEngine.KeyCode.Joystick6Button3:
                    break;
                case UnityEngine.KeyCode.Joystick6Button4:
                    break;
                case UnityEngine.KeyCode.Joystick6Button5:
                    break;
                case UnityEngine.KeyCode.Joystick6Button6:
                    break;
                case UnityEngine.KeyCode.Joystick6Button7:
                    break;
                case UnityEngine.KeyCode.Joystick6Button8:
                    break;
                case UnityEngine.KeyCode.Joystick6Button9:
                    break;
                case UnityEngine.KeyCode.Joystick6Button10:
                    break;
                case UnityEngine.KeyCode.Joystick6Button11:
                    break;
                case UnityEngine.KeyCode.Joystick6Button12:
                    break;
                case UnityEngine.KeyCode.Joystick6Button13:
                    break;
                case UnityEngine.KeyCode.Joystick6Button14:
                    break;
                case UnityEngine.KeyCode.Joystick6Button15:
                    break;
                case UnityEngine.KeyCode.Joystick6Button16:
                    break;
                case UnityEngine.KeyCode.Joystick6Button17:
                    break;
                case UnityEngine.KeyCode.Joystick6Button18:
                    break;
                case UnityEngine.KeyCode.Joystick6Button19:
                    break;
                case UnityEngine.KeyCode.Joystick7Button0:
                    break;
                case UnityEngine.KeyCode.Joystick7Button1:
                    break;
                case UnityEngine.KeyCode.Joystick7Button2:
                    break;
                case UnityEngine.KeyCode.Joystick7Button3:
                    break;
                case UnityEngine.KeyCode.Joystick7Button4:
                    break;
                case UnityEngine.KeyCode.Joystick7Button5:
                    break;
                case UnityEngine.KeyCode.Joystick7Button6:
                    break;
                case UnityEngine.KeyCode.Joystick7Button7:
                    break;
                case UnityEngine.KeyCode.Joystick7Button8:
                    break;
                case UnityEngine.KeyCode.Joystick7Button9:
                    break;
                case UnityEngine.KeyCode.Joystick7Button10:
                    break;
                case UnityEngine.KeyCode.Joystick7Button11:
                    break;
                case UnityEngine.KeyCode.Joystick7Button12:
                    break;
                case UnityEngine.KeyCode.Joystick7Button13:
                    break;
                case UnityEngine.KeyCode.Joystick7Button14:
                    break;
                case UnityEngine.KeyCode.Joystick7Button15:
                    break;
                case UnityEngine.KeyCode.Joystick7Button16:
                    break;
                case UnityEngine.KeyCode.Joystick7Button17:
                    break;
                case UnityEngine.KeyCode.Joystick7Button18:
                    break;
                case UnityEngine.KeyCode.Joystick7Button19:
                    break;
                case UnityEngine.KeyCode.Joystick8Button0:
                    break;
                case UnityEngine.KeyCode.Joystick8Button1:
                    break;
                case UnityEngine.KeyCode.Joystick8Button2:
                    break;
                case UnityEngine.KeyCode.Joystick8Button3:
                    break;
                case UnityEngine.KeyCode.Joystick8Button4:
                    break;
                case UnityEngine.KeyCode.Joystick8Button5:
                    break;
                case UnityEngine.KeyCode.Joystick8Button6:
                    break;
                case UnityEngine.KeyCode.Joystick8Button7:
                    break;
                case UnityEngine.KeyCode.Joystick8Button8:
                    break;
                case UnityEngine.KeyCode.Joystick8Button9:
                    break;
                case UnityEngine.KeyCode.Joystick8Button10:
                    break;
                case UnityEngine.KeyCode.Joystick8Button11:
                    break;
                case UnityEngine.KeyCode.Joystick8Button12:
                    break;
                case UnityEngine.KeyCode.Joystick8Button13:
                    break;
                case UnityEngine.KeyCode.Joystick8Button14:
                    break;
                case UnityEngine.KeyCode.Joystick8Button15:
                    break;
                case UnityEngine.KeyCode.Joystick8Button16:
                    break;
                case UnityEngine.KeyCode.Joystick8Button17:
                    break;
                case UnityEngine.KeyCode.Joystick8Button18:
                    break;
                case UnityEngine.KeyCode.Joystick8Button19:
                    break;
                default:
                    return Key.K_UNKNOWN;
            }
            return Key.K_UNKNOWN;
        }
    }
}
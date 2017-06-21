using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DFHack
{
    public interface IDFStream
    {
        void printerr(string Format, params object[] Parameters);
        void print(string Format, params object[] Parameters);
        void begin_batch();
        void end_batch();
        void add_text(color_value color, string text);
    }
}
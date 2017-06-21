using System;

namespace DFHack
{
    public class DFStringStream : IDFStream
    {
        public string Value { get; private set; }
        public void add_text(color_value color, string text)
        {
            Value += text;
        }

        public void begin_batch()
        {
        }

        public void end_batch()
        {
        }

        public void print(string Format, params object[] Parameters)
        {
            add_text(color_value.COLOR_BLACK, string.Format(Format, Parameters));
        }

        public void printerr(string Format, params object[] Parameters)
        {
            add_text(color_value.COLOR_RED, string.Format(Format, Parameters));
        }
    }
}
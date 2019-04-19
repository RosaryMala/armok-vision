using System;

namespace DFHack
{
    public class DFStringStream : IDFStream
    {
        public string Value { get; private set; }
        public void AddText(ColorValue color, string text)
        {
            Value += text;
        }

        public void BeginBatch()
        {
        }

        public void EndBatch()
        {
        }

        public void Print(string Format, params object[] Parameters)
        {
            AddText(ColorValue.ColorBlack, string.Format(Format, Parameters));
        }

        public void PrintErr(string Format, params object[] Parameters)
        {
            AddText(ColorValue.ColorRed, string.Format(Format, Parameters));
        }
    }
}
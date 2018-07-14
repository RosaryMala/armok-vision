using System;
using UnityEngine;
using AT.MIN;
using dfproto;

namespace  DFHack
{

    public class ColorOstream : IDFStream
    {
        string buffer;
        public void PrintErr(string Format, params object[] Parameters)
        {
            Debug.LogError(Tools.Sprintf(Format, Parameters).TrimEnd('\r', '\n'));
        }
        public void Print(string Format, params object[] Parameters)
        {
            Debug.Log(Tools.Sprintf(Format, Parameters).TrimEnd('\r', '\n'));
        }
        public void BeginBatch()
        {
            buffer = "";
        }
        public void EndBatch()
        {
            Debug.Log(buffer.TrimEnd('\r', '\n'));
            buffer = null;
        }

        public void AddText(ColorValue color, string text)
        {
            //Debug.Log(text);
            buffer += text;
        }

    }
    public class BufferedColorOstream : ColorOstream
    {
    //protected:
    public new void AddText(ColorValue color, string text)
    {
        if (text.Length == 0)
            return;

        if (buffer.Length == 0)
        {
            buffer = text;
        }
        else
        {
            buffer += text;
        }
    }



    //    buffered_color_ostream() {}
    //    ~buffered_color_ostream() {}

    //    const std::list<fragment_type> &fragments() { return buffer; }

    protected string buffer;
    }
}
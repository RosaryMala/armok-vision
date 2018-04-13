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
    public class ColorOstreamProxy : BufferedColorOstream
    {
        protected IDFStream target;

        //virtual void flush_proxy();

        public ColorOstreamProxy(IDFStream targetIn)
        {
            target = targetIn;
        }

        public virtual IDFStream proxy_target() { return target; }

        public void Decode(dfproto.CoreTextNotification data)
        {
            int cnt = data.fragments.Count;

            if (cnt > 0)
            {
                target.BeginBatch();

                for (int i = 0; i < cnt; i++)
                {
                    var frag = data.fragments[i];

                    //color_value color = frag.has_color() ? color_value(frag.color()) : COLOR_RESET;
                    target.AddText(ColorValue.ColorReset, frag.text);
                    //target.printerr(data.fragments[i].text);
                }

                target.EndBatch();
            }
        }
    }
}
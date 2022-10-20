using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledEnum : PropertyAttribute
    {
        public string display = "";
        public string file = "";
        public string options = "";

        public int top = 0;
        public int down = 0;

        public StyledEnum(string file, string options, int top, int down)
        {
            this.file = file;
            this.options = options;

            this.top = top;
            this.down = down;
        }

        public StyledEnum(string display, string file, string options, int top, int down)
        {
            this.display = display;
            this.file = file;
            this.options = options;

            this.top = top;
            this.down = down;
        }
    }
}


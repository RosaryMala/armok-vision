// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledText : PropertyAttribute
    {
        public string text = "";
        public TextAnchor alignment = TextAnchor.MiddleCenter;
        public float top = 0;
        public float down = 0;

        public StyledText()
        {

        }

        public StyledText(TextAnchor alignment)
        {
            this.alignment = alignment;
        }

        public StyledText(TextAnchor alignment, float top, float down)
        {
            this.alignment = alignment;
            this.top = top;
            this.down = down;
        }
    }
}


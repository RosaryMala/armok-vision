// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledButton : PropertyAttribute
    {
        public string Text = "";
        public float Top = 0;
        public float Down = 0;

        public StyledButton(string Text)
        {
            this.Text = Text;
            this.Top = 0;
            this.Down = 0;
        }

        public StyledButton(string Text, float Top, float Down)
        {
            this.Text = Text;
            this.Top = Top;
            this.Down = Down;
        }
    }
}


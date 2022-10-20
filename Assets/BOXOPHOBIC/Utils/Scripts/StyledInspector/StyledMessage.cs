// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledMessage : PropertyAttribute
    {
        public string Type;
        public string Message;
        public float Top;
        public float Down;

        public StyledMessage(string Type, string Message)
        {
            this.Type = Type;
            this.Message = Message;
            this.Top = 0;
            this.Down = 0;
        }

        public StyledMessage(string Type, string Message, float Top, float Down)
        {
            this.Type = Type;
            this.Message = Message;
            this.Top = Top;
            this.Down = Down;
        }
    }
}


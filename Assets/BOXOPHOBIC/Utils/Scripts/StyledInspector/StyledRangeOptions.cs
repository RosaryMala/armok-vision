// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledRangeOptions : PropertyAttribute
    {
        public string display;
        public float min;
        public float max;
        public string[] options;

        public StyledRangeOptions(string display, float min, float max,  string[] options)
        {
            this.display = display;
            this.min = min;
            this.max = max;

            this.options = options;
        }
    }
}


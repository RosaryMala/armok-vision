// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledLayers : PropertyAttribute
    {
        public string display = "";
        public StyledLayers()
        {
        }

        public StyledLayers(string display)
        {
            this.display = display;
        }
    }
}


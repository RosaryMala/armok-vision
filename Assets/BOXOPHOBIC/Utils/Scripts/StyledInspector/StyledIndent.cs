// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledIndent : PropertyAttribute
    {
        public int indent;

        public StyledIndent(int indent)
        {
            this.indent = indent;
        }
    }
}


using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledSpace : PropertyAttribute
    {
        public int space;

        public StyledSpace(int space)
        {
            this.space = space;
        }
    }
}


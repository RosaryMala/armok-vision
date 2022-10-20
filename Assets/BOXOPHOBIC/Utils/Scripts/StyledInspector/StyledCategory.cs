using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledCategory : PropertyAttribute
    {
        public string category;
        public float top;
        public float down;
        public bool colapsable;

        public StyledCategory(string category)
        {
            this.category = category;
            this.top = 10;
            this.down = 10;
            this.colapsable = false;
        }

        public StyledCategory(string category, bool colapsable)
        {
            this.category = category;
            this.top = 10;
            this.down = 10;
            this.colapsable = colapsable;
        }

        public StyledCategory(string category, float top, float down)
        {
            this.category = category;
            this.top = top;
            this.down = down;
            this.colapsable = false;
        }

        public StyledCategory(string category, int top, int down, bool colapsable)
        {
            this.category = category;
            this.top = top;
            this.down = down;
            this.colapsable = colapsable;
        }
    }
}


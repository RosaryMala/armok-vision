using UnityEngine;

namespace Boxophobic.StyledGUI
{
    public class StyledBanner : PropertyAttribute
    {
        public float colorR;
        public float colorG;
        public float colorB;
        public string title;
        public string helpURL;

        public StyledBanner(string title)
        {
            this.colorR = -1;
            this.title = title;
            this.helpURL = "";
        }

        public StyledBanner(float colorR, float colorG, float colorB, string title)
        {
            this.colorR = colorR;
            this.colorG = colorG;
            this.colorB = colorB;
            this.title = title;
            this.helpURL = "";
        }

        // Legacy
        public StyledBanner(string title, string helpURL)
        {
            this.colorR = -1;
            this.title = title;
            this.helpURL = helpURL;
        }

        public StyledBanner(float colorR, float colorG, float colorB, string title, string helpURL)
        {
            this.colorR = colorR;
            this.colorG = colorG;
            this.colorB = colorB;
            this.title = title;
            this.helpURL = helpURL;
        }

        public StyledBanner(string title, string subtitle, string helpURL)
        {
            this.colorR = -1;
            this.title = title;
            this.helpURL = helpURL;
        }

        public StyledBanner(float colorR, float colorG, float colorB, string title, string subtitle, string helpURL)
        {
            this.colorR = colorR;
            this.colorG = colorG;
            this.colorB = colorB;
            this.title = title;
            this.helpURL = helpURL;
        }
    }
}


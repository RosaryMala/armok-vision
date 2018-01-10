using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Building
{
    public class BuildingSelect : MonoBehaviour
    {
        public IClickable root;

        private void OnMouseOver()
        {
            //root.DrawSelection();
        }

        private void OnMouseDown()
        {
            root.HandleClick();
        }
    }
}

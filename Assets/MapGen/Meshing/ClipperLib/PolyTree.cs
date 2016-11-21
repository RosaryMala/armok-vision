using System.Collections.Generic;

namespace ClipperLib
{
    public class PolyTree : PolyNode
    {
        internal List<PolyNode> m_AllPolys = new List<PolyNode>();

        //The GC probably handles this cleanup more efficiently ...
        //~PolyTree(){Clear();}

        public void Clear()
        {
            for (int i = 0; i < m_AllPolys.Count; i++)
                m_AllPolys[i] = null;
            m_AllPolys.Clear();
            m_Childs.Clear();
        }

        public PolyNode GetFirst()
        {
            if (m_Childs.Count > 0)
                return m_Childs[0];
            else
                return null;
        }

        public int Total
        {
            get
            {
                int result = m_AllPolys.Count;
                //with negative offsets, ignore the hidden outer polygon ...
                if (result > 0 && m_Childs[0] != m_AllPolys[0]) result--;
                return result;
            }
        }
    }
}


namespace ClipperLib
{
    public struct IntRect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public IntRect(int l, int t, int r, int b)
        {
            left = l; top = t;
            right = r; bottom = b;
        }
        public IntRect(IntRect ir)
        {
            left = ir.left; top = ir.top;
            right = ir.right; bottom = ir.bottom;
        }
    }
}
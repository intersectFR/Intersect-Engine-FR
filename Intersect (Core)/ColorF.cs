namespace Intersect
{
    public class ColorF
    {
        public ColorF()
        {
        }

        public ColorF(float a, float r, float g, float b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public ColorF(float r, float g, float b)
        {
            A = 255;
            R = r;
            G = g;
            B = b;
        }

        public float A { get; set; }

        public float R { get; set; }

        public float G { get; set; }

        public float B { get; set; }

        public ColorF Clone() => new ColorF(A, R, G, B);
        
        public Color ToColor() => new Color((int) A, (int) R, (int) G, (int) B);

        public static ColorF White => new ColorF(255, 255, 255, 255);

        public static ColorF Black => new ColorF(255, 0, 0, 0);

        public static ColorF Transparent => new ColorF(0, 0, 0, 0);

        public static ColorF Red => new ColorF(255, 255, 0, 0);

        public static ColorF Green => new ColorF(255, 0, 255, 0);

        public static ColorF Blue => new ColorF(255, 0, 0, 255);

        public static ColorF Yellow => new ColorF(255, 255, 255, 0);

        public static ColorF LightCoral => new ColorF(255, 240, 128, 128);

        public static ColorF ForestGreen => new ColorF(255, 34, 139, 34);

        public static ColorF Magenta => new ColorF(255, 255, 0, 255);

        public byte GetHue() => 0;

        public static ColorF FromArgb(float a, float r, float g, float b) => new ColorF(a, r, g, b);

        public static ColorF FromColor(Color color) => color == default ? default : new ColorF(color.A, color.R, color.G, color.B);

        public static implicit operator ColorF(Color color) => FromColor(color);

        public static implicit operator Color(ColorF colorF) => colorF?.ToColor();
    }
}

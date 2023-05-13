using SkiaSharp;
using SkiaSharp.Views.Forms;
using tcMenuControlApi.Serialisation;
using Xamarin.Forms;

namespace embedCONTROL.ControlMgr
{
    public static class PortableColors
    {
        public static readonly PortableColor BLACK = new PortableColor(0, 0, 0);
        public static readonly PortableColor WHITE = new PortableColor(255, 255, 255);
        public static readonly PortableColor RED = new PortableColor(255, 0, 0);
        public static readonly PortableColor INDIGO = new PortableColor("#4B0082");
        public static readonly PortableColor DARK_GREY = new PortableColor(80, 80, 80);
        public static readonly PortableColor GREY = new PortableColor(150, 150, 150);
        public static readonly PortableColor LIGHT_GRAY = new PortableColor(200, 200, 200);
        public static readonly PortableColor DARK_SLATE_BLUE = new PortableColor(72, 61, 139);
        public static readonly PortableColor ANTIQUE_WHITE = new PortableColor(250, 235, 215);
        public static readonly PortableColor DARK_BLUE = new PortableColor(0, 0, 139);
        public static readonly PortableColor CRIMSON = new PortableColor(220, 20, 60);
        public static readonly PortableColor CORAL = new PortableColor(0xff, 0x7f, 0x50);
        public static readonly PortableColor CORNFLOWER_BLUE = new PortableColor(100, 149, 237);
        public static readonly PortableColor BLUE = new PortableColor(0, 0, 255);
        public static readonly PortableColor GREEN = new PortableColor(0, 255, 0);

        public static PortableColor ToPortable(this Color color)
        {
            return new PortableColor((short) (color.R * 255.0), (short) (color.G * 255.0), (short) (color.B * 255.0));
        }

        public static Color AsXamarin(this PortableColor color)
        {
            return new Color(color.red / 255.0, color.green / 255.0, color.blue / 255.0);
        }
        public static SKColor AsSkia(this PortableColor color)
        {
            return new Color(color.red / 255.0, color.green / 255.0, color.blue / 255.0).ToSKColor();
        }

    }
}

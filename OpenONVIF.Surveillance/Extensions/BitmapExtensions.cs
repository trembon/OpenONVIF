using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Surveillance.Extensions
{
    public static class BitmapExtensions
    {
        public static bool IsGreyScale(this Bitmap bitmap, int threshold)
        {
            int rgbDelta;
            Color pixelColor;

            for (Int32 x = 0; x < bitmap.Width; x += 15)
            {
                for (Int32 y = 0; y < bitmap.Height; y += 15)
                {
                    pixelColor = bitmap.GetPixel(x, y);
                    rgbDelta = Math.Abs(pixelColor.R - pixelColor.G) + Math.Abs(pixelColor.G - pixelColor.B) + Math.Abs(pixelColor.B - pixelColor.R);

                    if (rgbDelta > threshold)
                        return false;
                }
            }

            return true;
        }
    }
}

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace Trading212Stick.Helpers
{
    public static class IconHelper
    {
        /// <summary>
        /// Laadt een PNG bestand en converteert het naar een rond icon
        /// </summary>
        public static Icon CreateRoundIconFromPng(string pngPath, int size = 32)
        {
            if (!File.Exists(pngPath))
            {
                return SystemIcons.Application;
            }

            try
            {
                using (var originalImage = Image.FromFile(pngPath))
                {
                    // Maak een rond bitmap
                    using (var roundBitmap = CreateRoundBitmap(originalImage, size))
                    {
                        // Converteer bitmap naar icon
                        return Icon.FromHandle(roundBitmap.GetHicon());
                    }
                }
            }
            catch
            {
                // Fallback naar standaard icon bij fouten
                return SystemIcons.Application;
            }
        }

        /// <summary>
        /// Laadt een embedded PNG resource en converteert het naar een rond icon
        /// </summary>
        public static Icon CreateRoundIconFromEmbeddedPng(string resourceName, int size = 32)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    return SystemIcons.Application;
                }

                using (var originalImage = Image.FromStream(stream))
                {
                    using (var roundBitmap = CreateRoundBitmap(originalImage, size))
                    {
                        return Icon.FromHandle(roundBitmap.GetHicon());
                    }
                }
            }
            catch
            {
                return SystemIcons.Application;
            }
        }

        /// <summary>
        /// Maakt een ronde bitmap van een afbeelding
        /// </summary>
        private static Bitmap CreateRoundBitmap(Image source, int size)
        {
            // Maak een nieuwe bitmap met de gewenste grootte
            var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
            
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Stel hoogwaardige rendering in
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                // Maak de achtergrond transparant
                graphics.Clear(Color.Transparent);

                // Maak een rond pad (cirkel)
                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(0, 0, size, size);
                    
                    // Gebruik het pad als clip region
                    graphics.SetClip(path);
                    
                    // Teken de afbeelding binnen de cirkel
                    graphics.DrawImage(source, 0, 0, size, size);
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Maakt een rond icon met een gekleurde achtergrond (optioneel)
        /// </summary>
        public static Icon CreateRoundIconWithBackground(string pngPath, Color backgroundColor, int size = 32)
        {
            if (!File.Exists(pngPath))
            {
                return SystemIcons.Application;
            }

            try
            {
                using (var originalImage = Image.FromFile(pngPath))
                {
                    var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
                    
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        // Maak transparante achtergrond
                        graphics.Clear(Color.Transparent);

                        // Teken een ronde achtergrond
                        using (var brush = new SolidBrush(backgroundColor))
                        {
                            graphics.FillEllipse(brush, 0, 0, size, size);
                        }

                        // Teken de afbeelding binnen de cirkel (met een kleine margin)
                        int margin = 2;
                        using (var path = new GraphicsPath())
                        {
                            path.AddEllipse(margin, margin, size - (margin * 2), size - (margin * 2));
                            graphics.SetClip(path);
                            graphics.DrawImage(originalImage, margin, margin, size - (margin * 2), size - (margin * 2));
                        }
                    }

                    return Icon.FromHandle(bitmap.GetHicon());
                }
            }
            catch
            {
                return SystemIcons.Application;
            }
        }
    }
}

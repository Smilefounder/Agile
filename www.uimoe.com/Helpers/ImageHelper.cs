using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace www.uimoe.com.Helpers
{
    public class ImageHelper
    {
        public static void Clip(string imgfile, int rows, int cols)
        {
            var filename = System.IO.Path.GetFileName(imgfile);
            var ext = System.IO.Path.GetExtension(imgfile);
            var img = Image.FromFile(imgfile);
            var wt = img.Width;
            var ht = img.Height;

            var cellw = Convert.ToInt32(Math.Round(1.0 * wt / cols));
            var cellh = Convert.ToInt32(Math.Round(1.0 * ht / rows));

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    var bitmap = new Bitmap(cellw, cellh);
                    var g = Graphics.FromImage(bitmap);

                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.Clear(Color.Transparent);
                    g.DrawImage(img,
                        new Rectangle(0, 0, cellw, cellh),
                        new Rectangle(j * cellw, i * cellh, cellw, cellh),
                        GraphicsUnit.Pixel);

                    var filename3 = imgfile.Replace(ext, "_" + i + "x" + j + ext);
                    bitmap.Save(filename3, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }
    }
}
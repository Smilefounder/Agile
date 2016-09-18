using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agile.Web.Helpers
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

        public static void MakeThumbnail(string imgfile, string thumbfile, int limit, bool iswl, out int height, out int width)
        {
            var needmake = false;
            var wt = 0;
            var ht = 0;
            var img = Image.FromFile(imgfile);
            if (img.Width > limit && iswl)
            {
                needmake = true;
                wt = limit;
                var percent = 1.0 * limit / img.Width;
                ht = Convert.ToInt32(percent * img.Height);
            }

            if (img.Height > limit && !iswl)
            {
                needmake = true;
                ht = limit;
                var percent = 1.0 * limit / img.Height;
                wt = Convert.ToInt32(percent * img.Width);
            }

            height = img.Height;
            width = img.Width;

            if (!needmake)
            {
                img.Dispose();
                System.IO.File.Copy(imgfile, thumbfile, true);
                return;
            }

            var bitmap = new Bitmap(wt, ht);
            var g = Graphics.FromImage(bitmap);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(Color.Transparent);
            g.DrawImage(img,
                new Rectangle(0, 0, wt, ht),
                new Rectangle(0, 0, img.Width, img.Height),
                GraphicsUnit.Pixel);

            try
            {
                bitmap.Save(thumbfile, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                img.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        public enum ThumbnailMode
        {
            H = 0,

            W = 1,

            C = 2
        }
    }
}

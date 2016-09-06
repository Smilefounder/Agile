using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace food.uimoe.com.Helpers
{
    public class ImageHelper
    {
        public static void MakeThumbnail(string imgfile, string thumbfile,int limit,bool iswl,out int height,out int width)
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
                System.IO.File.Copy(imgfile, thumbfile,true);
                return;
            }

            var bitmap = new Bitmap(wt,ht);
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
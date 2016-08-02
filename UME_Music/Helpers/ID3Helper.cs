using System.Collections.Generic;
using System.Text;

namespace UME_Music.Helpers
{
    public class ID3Helper
    {
        public static ID3Info Read(string filepath)
        {
            long offset = 0;
            var buffer = new byte[3];

            using (var fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open))
            {
                fs.Read(buffer, 0, buffer.Length);
                if (Encoding.ASCII.GetString(buffer).ToUpper() != "ID3")
                {
                    return null;
                }

                var info = new ID3Info();

                offset += buffer.Length;
                fs.Seek(offset, System.IO.SeekOrigin.Begin);
                buffer = new byte[1];
                fs.Read(buffer, 0, buffer.Length);
                info.Version = Encoding.ASCII.GetString(buffer);

                offset += buffer.Length;
                fs.Seek(offset, System.IO.SeekOrigin.Begin);
                buffer = new byte[1];
                fs.Read(buffer, 0, buffer.Length);
                info.Reversion = Encoding.ASCII.GetString(buffer);

                offset += buffer.Length;
                fs.Seek(offset, System.IO.SeekOrigin.Begin);
                buffer = new byte[1];
                fs.Read(buffer, 0, buffer.Length);
                info.Flag = Encoding.ASCII.GetString(buffer);

                offset += buffer.Length;
                fs.Seek(offset, System.IO.SeekOrigin.Begin);
                buffer = new byte[4];
                fs.Read(buffer, 0, buffer.Length);
                info.Size = 0x200000 * buffer[0] + 0x4000 * buffer[1] + 0x80 * buffer[2] + buffer[3];

                offset += buffer.Length;
                ReadFrames(fs, offset, info);

                return info;
            }
        }

        private static void ReadFrames(System.IO.FileStream fs, long offset, ID3Info info)
        {
            if (offset < info.Size - 10)
            {
                var buffer = new byte[4];
                fs.Seek(offset, System.IO.SeekOrigin.Begin);
                fs.Read(buffer, 0, buffer.Length);
                var framekey = Encoding.ASCII.GetString(buffer);

                offset += buffer.Length;
                fs.Seek(offset, System.IO.SeekOrigin.Begin);
                buffer = new byte[4];
                fs.Read(buffer, 0, buffer.Length);
                var framesize = 0x1000000 * buffer[0] + 0x10000 * buffer[1] + 0x100 * buffer[2] + buffer[3];
                if (framesize == 0)
                {
                    return;
                }

                offset += buffer.Length;
                fs.Seek(offset, System.IO.SeekOrigin.Begin);
                buffer = new byte[2];
                fs.Read(buffer, 0, buffer.Length);
                var frameflag = Encoding.ASCII.GetString(buffer);

                offset += buffer.Length;
                fs.Seek(offset, System.IO.SeekOrigin.Begin);
                buffer = new byte[framesize];
                fs.Read(buffer, 0, buffer.Length);
                var framecontent = "";
                if (buffer[0] == 1 && buffer[1] == 255 && buffer[2] == 254)
                {
                    framecontent = Encoding.Unicode.GetString(buffer, 3, buffer.Length - 3);
                }
                else
                {
                    framecontent = Encoding.ASCII.GetString(buffer, 1, buffer.Length - 1);
                }

                if (!info.Frames.ContainsKey(framekey))
                {
                    info.Frames.Add(framekey, framecontent);
                }
                else
                {
                    info.Frames[framekey] = framecontent;
                }

                offset += buffer.Length;
                ReadFrames(fs, offset, info);
            }
        }
    }

    public class ID3Info
    {
        public string Version { get; set; }

        public string Reversion { get; set; }

        public string Flag { get; set; }

        public long Size { get; set; }

        private Dictionary<string, string> frames;

        public Dictionary<string, string> Frames
        {
            get
            {
                if (frames == null)
                {
                    frames = new Dictionary<string, string>();
                }

                return frames;
            }

            set
            {
                frames = value;
            }
        }

        public string GetFrame(string key)
        {
            if (Frames.ContainsKey(key))
            {
                return Frames[key];
            }

            return "";
        }
    }
}

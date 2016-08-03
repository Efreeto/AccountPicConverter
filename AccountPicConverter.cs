using System;
using System.IO;
using System.Drawing;

namespace accountpicture_ms
{
    class AccountPicConverter
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the .accountpicture-ms file path.");
                return 1;
            }

            string filename = Path.GetFileNameWithoutExtension(args[0]);
            Bitmap image96 = GetImage96(args[0]);
            image96.Save(filename + "-96.bmp");
            Bitmap image448 = GetImage448(args[0]);
            image448.Save(filename + "-448.bmp");

            Console.WriteLine("Extracted images successfully");
            return 0;
        }

        public static Bitmap GetImage96(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            long position = Seek(fs, "JFIF", 0);
            byte[] b = new byte[Convert.ToInt32(fs.Length)];
            fs.Seek(position - 6, SeekOrigin.Begin);
            fs.Read(b, 0, b.Length);
            fs.Close();
            fs.Dispose();
            return GetBitmapImage(b);
        }

        public static Bitmap GetImage448(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            long position = Seek(fs, "JFIF", 100);
            byte[] b = new byte[Convert.ToInt32(fs.Length)];
            fs.Seek(position - 6, SeekOrigin.Begin);
            fs.Read(b, 0, b.Length);
            fs.Close();
            fs.Dispose();
            return GetBitmapImage(b);
        }

        public static Bitmap GetBitmapImage(byte[] imageBytes)
        {
            //var bitmapImage = new Bitmap();
            //bitmapImage.BeginInit();
            //bitmapImage.StreamSource = new MemoryStream(imageBytes);
            //bitmapImage.EndInit();
            //return bitmapImage;
            var ms = new MemoryStream(imageBytes);
            var bitmapImage = new Bitmap(ms);
            return bitmapImage;
        }

        public static long Seek(System.IO.FileStream fs, string searchString, int startIndex)
        {
            char[] search = searchString.ToCharArray();
            long result = -1, position = 0, stored = startIndex,
            begin = fs.Position;
            int c;
            while ((c = fs.ReadByte()) != -1)
            {
                if ((char)c == search[position])
                {
                    if (stored == -1 && position > 0 && (char)c == search[0])
                    {
                        stored = fs.Position;
                    }
                    if (position + 1 == search.Length)
                    {
                        result = fs.Position - search.Length;
                        fs.Position = result;
                        break;
                    }
                    position++;
                }
                else if (stored > -1)
                {
                    fs.Position = stored + 1;
                    position = 1;
                    stored = -1;
                }
                else
                {
                    position = 0;
                }
            }

            if (result == -1)
            {
                fs.Position = begin;
            }
            return result;
        }
    }
}

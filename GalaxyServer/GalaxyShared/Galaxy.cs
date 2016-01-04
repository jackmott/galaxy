using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GalaxyShared
{
    public static class Galaxy
    {
        public static byte[] HoagPixels;
                     
        public const int HOAG_SIZE = 1024;

        public const int GALAXY_SIZE_LIGHTYEARS = 128000; //light years, cube
        public const int SECTOR_SIZE = 25; //light years cubed
        public const float HALF_SECTOR_SIZE = SECTOR_SIZE / 2f;
        public const int SECTOR_SIZE_CUBED = SECTOR_SIZE * SECTOR_SIZE * SECTOR_SIZE;
        public const int GALAXY_SIZE_SECTORS = GALAXY_SIZE_LIGHTYEARS / SECTOR_SIZE;        
        public const int GALAXY_THICKNESS_SECTORS = 20000 / SECTOR_SIZE;
        public const float EXPAND_FACTOR = 1f / 2.5f; //multiplied by galaxy coordinates to get unity coordinates
        

        private static float LightyearsPerPixel;

        public static void Init()
        {
            


            Bitmap Hoag = null;
            if (Hoag == null)
            {

                try
                {
                    Hoag = (Bitmap)Bitmap.FromFile("Assets/Plugins/GalaxyShared/hoag-ring.bmp");
                }
                catch
                {
                    Hoag = (Bitmap)Bitmap.FromFile("assets/hoag-ring.bmp");
                }
                BitmapData hoagdata = Hoag.LockBits(new Rectangle(0, 0, HOAG_SIZE, HOAG_SIZE), ImageLockMode.ReadWrite, Hoag.PixelFormat);                
                HoagPixels = new byte[HOAG_SIZE * hoagdata.Stride];
                IntPtr Iptr = hoagdata.Scan0;
                Marshal.Copy(Iptr, HoagPixels, 0, HoagPixels.Length);
                LightyearsPerPixel = (float)GALAXY_SIZE_LIGHTYEARS / (float)Hoag.Width;
               
            } 
        }



        public static Color GetColorAt(int x, int y, int z)
        {
                         
            int pixelX = Convert.ToInt32(x*SECTOR_SIZE / LightyearsPerPixel)-1;
            int pixelY = Convert.ToInt32(y*SECTOR_SIZE / LightyearsPerPixel)-1;
            pixelX = Math.Max(0, pixelX);
            pixelY = Math.Max(0, pixelY);
            //24bit bitmap, 3 bytes per pixel
            int i = (pixelY*HOAG_SIZE*3) + (pixelX*3);
            Color c1 = Color.FromArgb(255, HoagPixels[i], HoagPixels[i + 1], HoagPixels[i + 2]);

            float distanceFromGalacticPlane = Math.Abs(GALAXY_THICKNESS_SECTORS / 2.0f - z);

            float zFactor = Math.Max(0, (400f - distanceFromGalacticPlane) / 400f);

            return Color.FromArgb(Convert.ToInt32(c1.R * zFactor), Convert.ToInt32(c1.G * zFactor),Convert.ToInt32( c1.B * zFactor));
            

        }


    }
}

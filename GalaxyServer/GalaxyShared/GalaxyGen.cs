using System;
using System.Diagnostics;
using System.Drawing;


namespace GalaxyShared
{


    public class GalaxyGen
    {
        
        Bitmap hoag;

        public GalaxyGen()
        {
            loadGalaxyBitmap();

        }

        public void loadGalaxyBitmap()
        {
            try
            {
                hoag = (Bitmap)Bitmap.FromFile("Assets/GalaxyShared/hoag-ring.bmp");
            }
            catch
            {
                hoag = (Bitmap)Bitmap.FromFile("assets/hoag-ring.bmp");
            }

        }


        public GalaxySector GetSector(SectorCoord coord, int everyNth)
        {
            return new GalaxySector(coord, hoag,everyNth);
        }



    }


}


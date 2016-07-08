using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace EssenceAndroid
{
    public class BitmapHelper
    {
        public static Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    imageBitmap = Bitmap.CreateScaledBitmap(imageBitmap, 90.ToPixels(), 90.ToPixels(), true);
                }
            }
            return imageBitmap;
        }
    }
}

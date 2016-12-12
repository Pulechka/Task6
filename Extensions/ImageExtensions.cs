using System;
using System.Drawing;

namespace UsersKeeper.Common.Extensions
{
    public static class ImageExtension
    {
        public static Image Resize(this Image sourceImage, int newWidth, int maxHeight, bool reduceOnly)
        {
            // Гарантия того, что не будет использована сохранённая внутри изображения миниатюра
            sourceImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
            sourceImage.RotateFlip(RotateFlipType.Rotate180FlipNone);

            if (reduceOnly && sourceImage.Width <= newWidth)
            {
                newWidth = sourceImage.Width;
            }

            int newHeight = sourceImage.Height * newWidth / sourceImage.Width;
            if (newHeight > maxHeight)
            {
                newWidth = sourceImage.Width * maxHeight / sourceImage.Height;
                newHeight = maxHeight;
            }

            Image newImage = sourceImage.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);

            return newImage;
        }
    }
}

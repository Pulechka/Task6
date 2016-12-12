using System;
using System.Drawing;
using System.IO;
using UsersKeeper.BllContracts;
using UsersKeeper.Common.Extensions;
using UsersKeeper.DalContracts;
using UsersKeeper.Entities;

namespace UsersKeeper.Logic
{
    public class AwardImageLogic : IAwardImageLogic
    {
        private IAwardImageDao imageDao;

        public AwardImageLogic(IAwardImageDao dao)
        {
            imageDao = dao;
        }

        public ImageDTO GetAwardImage(Guid awardId)
        {
            return imageDao.GetAwardImage(awardId);
        }

        public bool SetAwardImage(Guid awardId, string type, byte[] binaryData)
        {
            if (awardId == null)
                throw new ArgumentNullException(nameof(awardId));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (binaryData == null)
                throw new ArgumentNullException(nameof(binaryData));

            var image = new ImageDTO
            {
                Id = awardId,
                Type = type,
                BinaryData = binaryData
            };

            ResizeImage(image);

            return imageDao.SetAwardImage(image);
        }

        private void ResizeImage(ImageDTO image)
        {
            Image imgToRezise;
            using (var ms = new MemoryStream(image.BinaryData))
            {
                imgToRezise = Image.FromStream(ms);
            }

            var resizedImage = imgToRezise.Resize(222, 222, true);

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                image.BinaryData = ms.ToArray();
            }
        }
    }
}

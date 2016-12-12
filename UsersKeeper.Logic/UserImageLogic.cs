using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.BllContracts;
using UsersKeeper.Common.Extensions;
using UsersKeeper.DalContracts;
using UsersKeeper.Entities;


namespace UsersKeeper.Logic
{
    public class UserImageLogic : IUserImageLogic
    {
        private IUserImageDao imageDao;

        public UserImageLogic(IUserImageDao dao)
        {
            imageDao = dao;
        }

        public ImageDTO GetUserImage(Guid userId)
        {
            return imageDao.GetUserImage(userId);
        }

        public bool SetUserImage(Guid userId, string type, byte[] binaryData)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (binaryData == null)
                throw new ArgumentNullException(nameof(binaryData));

            var image = new ImageDTO
            {
                Id = userId,
                Type = type,
                BinaryData = binaryData
            };

            ResizeImage(image);

            return imageDao.SetUserImage(image);
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

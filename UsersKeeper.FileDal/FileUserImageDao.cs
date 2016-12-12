﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersKeeper.DalContracts;
using UsersKeeper.Entities;
using System.Drawing;
using UsersKeeper.Common.Extensions;

namespace UsersKeeper.FileDal
{
    public class FileUserImageDao : IUserImageDao
    {
        private string imagesFileName;
        private string imageFolderPath;

        public FileUserImageDao()
        {
            imagesFileName = ConfigurationManager.AppSettings["UserImagesFilePath"];
            if (imagesFileName == null)
                throw new ConfigurationErrorsException("Missed parameter \"UserImagesFilePath\"");
            using (var file = File.Open(imagesFileName, FileMode.OpenOrCreate)) { }

            imageFolderPath = ConfigurationManager.AppSettings["UserImageFolderPath"];
            if (imageFolderPath == null)
                throw new ConfigurationErrorsException("Missed parameter \"UserImagesFilePath\"");
        }

        public ImageDTO GetUserImage(Guid userId)
        {
            ImageDTO userImage = File.ReadAllLines(imagesFileName, Encoding.Unicode).Select(line =>
            {
                var parts = line.Split('|');
                return new ImageDTO
                {
                    Id = Guid.Parse(parts[0]),
                    Type = parts[1],
                };
            }).SingleOrDefault(image=>image.Id == userId);

            if (userImage == null)
            {
                userImage = new ImageDTO
                {
                    Id = userId,
                    Type = "image/png",
                    BinaryData = File.ReadAllBytes($@"{imageFolderPath}\default.png"),
                    };
            }
            else
            {
                var ext = GetExtensionByType(userImage.Type);
                userImage.BinaryData = File.ReadAllBytes($@"{imageFolderPath}\{userImage.Id}.{ext}");
            }
            return userImage;
        }

        public bool SetUserImage(ImageDTO image)
        {
            var ext = GetExtensionByType(image.Type);
            File.WriteAllBytes($@"{imageFolderPath}\{image.Id}.{ext}", image.BinaryData);

            var existingId = File.ReadAllLines(imagesFileName, Encoding.Unicode).
                            Select(line => Guid.Parse(line.Split('|')[0])).
                            FirstOrDefault(id=> id == image.Id);

            if (existingId == new Guid())
            {
                File.AppendAllLines(imagesFileName, new string[] { $"{image.Id}|{image.Type}" }, Encoding.Unicode);
            }
            else
            {
                UpdateImageInfo(image);
            }
            return true;
        }

        private void UpdateImageInfo(ImageDTO image)
        {
            var tempFile = Path.GetTempFileName();

            using (var sr = new StreamReader(imagesFileName, Encoding.Unicode))
            {
                var tempFS = new FileStream(tempFile, FileMode.Open);
                using (var sw = new StreamWriter(tempFS, Encoding.Unicode))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        Guid readedId = Guid.Parse(line.Split('|')[0]);

                        if (readedId != image.Id)
                        {
                            sw.WriteLine(line, Encoding.Unicode);
                        }
                        else
                        {
                            sw.WriteLine($"{image.Id}|{image.Type}", Encoding.Unicode);
                        }
                    }
                }
            }
            File.Delete(imagesFileName);
            File.Move(tempFile, imagesFileName);
        }


        private string GetExtensionByType(string type)
        {
            int startIndex = type.IndexOf('/');
            return type.Substring(++startIndex, type.Length - startIndex);
        }
    }
}

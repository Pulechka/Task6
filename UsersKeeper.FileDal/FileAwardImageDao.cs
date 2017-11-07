using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using UsersKeeper.DalContracts;
using UsersKeeper.Entities;

namespace UsersKeeper.FileDal
{
    public class FileAwardImageDao : IAwardImageDao
    {
        private string imagesFileName;
        private string imageFolderPath;

        public FileAwardImageDao()
        {
            imagesFileName = ConfigurationManager.AppSettings["AwardImagesFilePath"];
            if (imagesFileName == null)
                throw new ConfigurationErrorsException("Missed parameter \"AwardImagesFilePath\"");
            using (var file = File.Open(imagesFileName, FileMode.OpenOrCreate)) { }

            imageFolderPath = ConfigurationManager.AppSettings["AwardImageFolderPath"];
            if (imageFolderPath == null)
                throw new ConfigurationErrorsException("Missed parameter \"AwardImageFolderPath\"");
        }

        public ImageDTO GetAwardImage(Guid awardId)
        {
            ImageDTO userImage = File.ReadAllLines(imagesFileName, Encoding.Unicode).Select(line =>
            {
                var parts = line.Split('|');
                return new ImageDTO
                {
                    OwnerId = Guid.Parse(parts[0]),
                    Type = parts[1],
                };
            }).SingleOrDefault(image => image.OwnerId == awardId);

            if (userImage != null)
            { 
                var ext = GetExtensionByType(userImage.Type);
                userImage.BinaryData = File.ReadAllBytes($@"{imageFolderPath}\{userImage.OwnerId}.{ext}");
            }
            return userImage;
        }

        public bool SetAwardImage(ImageDTO image)
        {
            SetImage(image);
            return true;
        }


        public bool UpdateAwardImage(ImageDTO image)
        {
            SetImage(image);
            return true;
        }

        public ImageDTO GetDefaultImage()
        {
            return new ImageDTO
            {
                Type = "image/png",
                BinaryData = File.ReadAllBytes($@"{imageFolderPath}\default.png"),
            };
        }


        private void SetImage(ImageDTO image)
        {
            var ext = GetExtensionByType(image.Type);
            File.WriteAllBytes($@"{imageFolderPath}\{image.OwnerId}.{ext}", image.BinaryData);

            var existingId = File.ReadAllLines(imagesFileName, Encoding.Unicode).
                            Select(line => Guid.Parse(line.Split('|')[0])).
                            FirstOrDefault(id => id == image.OwnerId);

            if (existingId == new Guid())
            {
                File.AppendAllLines(imagesFileName, new string[] { $"{image.OwnerId}|{image.Type}" }, Encoding.Unicode);
            }
            else
            {
                UpdateImageInfo(image);
            }
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

                        if (readedId != image.OwnerId)
                        {
                            sw.WriteLine(line, Encoding.Unicode);
                        }
                        else
                        {
                            sw.WriteLine($"{image.OwnerId}|{image.Type}", Encoding.Unicode);
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

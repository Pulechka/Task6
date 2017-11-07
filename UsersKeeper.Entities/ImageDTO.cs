using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersKeeper.Entities
{
    public class ImageDTO
    {
        public Guid OwnerId { get; set; }
        public string Type { get; set; }
        public byte[] BinaryData { get; set; }
    }
}

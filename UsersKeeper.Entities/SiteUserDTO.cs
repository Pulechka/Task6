using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersKeeper.Entities
{
    public class SiteUserDTO
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public int[] RolesId { get; set; }
    }
}

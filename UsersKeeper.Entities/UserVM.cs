using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersKeeper.Entities
{
    public class UserVM
    {
        public int Age
        {
            get
            {
                int age = DateTime.Now.Year - BirthDate.Year;
                if (DateTime.Now.Month < BirthDate.Month ||
                    (DateTime.Now.Month == BirthDate.Month && DateTime.Now.Day < BirthDate.Day))
                    age--;
                return age;
            }
        }

        public DateTime BirthDate { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<AwardDTO> Awards { get; set; }
    }
}

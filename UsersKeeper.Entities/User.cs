using System;

namespace UsersKeeper.Entities
{
    public class User
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
    }
}
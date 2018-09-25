using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShanesTestConsoleApp
{
    class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<UserMenu> UserMenus { get; set; }

        public static bool Validate(User user)
        {
            using(DataContext context = new DataContext())
            {
                return !(context.Users.Any(u => u.FirstName == user.FirstName && u.LastName == user.LastName));
            }
        }

        public static bool Exists(string userName)
        {
            using (DataContext context = new DataContext())
            {
                string[] names = userName.Split(' ');
                return context.Users.Any(u => u.FirstName == names[0] && u.LastName == names[1]);
            }
        }
    }
}

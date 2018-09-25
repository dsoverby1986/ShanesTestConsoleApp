using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShanesTestConsoleApp
{
    class UserMenu
    {
        public int UserMenuId { get; set; }
        public int UserId { get; set; }
        public int PageId { get; set; }
        public int Sequence { get; set; }

        public User User { get; set; }
        public Page Page { get; set; }

        public static bool Validate(UserMenu userMenu)
        {
            using(DataContext context = new DataContext())
            {
                return !(context.UserMenus.Any(m => m.UserId == userMenu.UserId && m.PageId == userMenu.PageId));
            }
        }
    }
}

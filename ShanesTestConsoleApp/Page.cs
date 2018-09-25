using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShanesTestConsoleApp
{
    class Page
    {
        public int PageId { get; set; }
        public string Name { get; set; }

        public List<UserMenu> UserMenus { get; set; }

        public static bool Validate(Page page)
        {
            using(DataContext context = new DataContext())
            {
                return !(context.Pages.Any(p => p.Name == page.Name));
            }
        }
    }
}

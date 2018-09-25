using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShanesTestConsoleApp
{
    class Program
    {
        private delegate void DelegateProcessExecutor();

        private static Dictionary<string, Delegate> _MenuOptions = new Dictionary<string, Delegate>()
        {
            { "Add user", new DelegateProcessExecutor(AddUser) },
            { "Add page", new DelegateProcessExecutor(AddPage) },
            { "Add user menu", new DelegateProcessExecutor(AddUserMenu) },
            { "View users", new DelegateProcessExecutor(ViewUsers) },
            { "View pages", new DelegateProcessExecutor(ViewPages) },
            { "View user menu items", new DelegateProcessExecutor(ViewUserMenuItems) },
            { "View users by page", new DelegateProcessExecutor(ViewUsersByPage) },
            { "Exit", new DelegateProcessExecutor(Exit) }
        };

        static void Main(string[] args)
        {
            PrintMenu();
            GetUserMenuOption();
        }

        private static void PrintMenu()
        { 
            Console.WriteLine("What would you like to do?\n");
            for (int i = 1; i < _MenuOptions.Count + 1; i++)
                Console.WriteLine($"{i}.) {_MenuOptions.ElementAt(i - 1).Key}");
            Console.WriteLine();
        }

        private static object GetUserMenuOption(int attempts = 0)
        {
            if (attempts > 0)
                Console.WriteLine("\nInvalid input. Try again...\n");
            Console.Write("Enter an option number from above and press 'Enter'...\n\n");
            string option = Console.ReadLine();
            int optionNum = 0;
            if (Int32.TryParse(option, out optionNum) && optionNum <= _MenuOptions.Count && optionNum > 0)
                return _MenuOptions.ElementAt(optionNum - 1).Value.DynamicInvoke();
            return GetUserMenuOption(++attempts);
        }

        private static void AddUser()
        {
            string firstName = GetUsersFirstName();
            string lastName = GetUsersLastName();
            User newUser = new User { FirstName = firstName, LastName = lastName };
            bool isValid = User.Validate(newUser);
            if (isValid)
            {
                using (DataContext context = new DataContext())
                {
                    context.Users.Add(newUser);
                    context.SaveChanges();

                    Console.WriteLine($"\nNew user, {firstName} {lastName}, has been added.");
                }
            }
            else
            {
                Console.WriteLine("\nA user with the same name already exists. No new user will be added.");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
            Main(new string[] { });
        }

        private static string GetUsersFirstName()
        {
            string firstName = string.Empty;
            bool invalidInput = false;

            do
            {
                Console.WriteLine("\nWhat is the user's first name?\n");
                firstName = Console.ReadLine();
                invalidInput = string.IsNullOrEmpty(firstName);
                if (invalidInput)
                    Console.WriteLine("\nInvalid input. Try again.");
            } while (invalidInput);

            return firstName;
        }

        private static string GetUsersLastName()
        {
            string lastName = string.Empty;
            bool invalidInput = false;

            do
            {
                Console.WriteLine("\nWhat is the user's last name?\n");
                lastName = Console.ReadLine();
                invalidInput = string.IsNullOrEmpty(lastName);
                if (invalidInput)
                    Console.WriteLine("\nInvalid input. Try again.");
            } while (invalidInput);

            return lastName;
        }

        private static void ViewUsers()
        {
            Console.Clear();

            List<User> users = null;

            using (DataContext context = new DataContext())
                users = context.Users.ToList();

            foreach(User user in users)
                Console.WriteLine($"{user.FirstName} {user.LastName}");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadLine();
            Console.Clear();
            Main(new string[] { });
        }

        private static void AddPage()
        {
            string pageName = GetPageName();
            Page newPage = new Page { Name = pageName };
            bool isValid = Page.Validate(newPage);
            if (isValid)
            {
                using(DataContext context = new DataContext())
                {
                    context.Pages.Add(newPage);
                    context.SaveChanges();
                    Console.WriteLine($"\nThe new page, {newPage.Name}, has been added.");
                }
            }
            else
            {
                Console.WriteLine("\nA page with this name already exists.");
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
            Main(new string[] { });
        }

        private static string GetPageName()
        {
            string pageName = string.Empty;
            bool invalidInput = false;

            do
            {
                Console.WriteLine("\nWhat is the page name?\n");
                pageName = Console.ReadLine();
                invalidInput = string.IsNullOrEmpty(pageName);
                if (invalidInput)
                    Console.WriteLine("\nInvalid Input. Try again.");
            } while (invalidInput);

            return pageName;
        }

        private static void ViewPages()
        {
            Console.Clear();

            List<Page> pages = null;

            using(DataContext context = new DataContext())
                pages = context.Pages.ToList();

            foreach (Page page in pages)
                Console.WriteLine($"{page.Name}");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
            Main(new string[] { });
        }

        private static void AddUserMenu()
        {
            User user = GetUser();
            Page page = GetPage();
            int sequence = GetUserMenuOrderSequence();
            UserMenu newUserMenu = new UserMenu { UserId = user.UserId, PageId = page.PageId, Sequence = sequence };
            bool isValid = UserMenu.Validate(newUserMenu);
            if (isValid)
            {
                using (DataContext context = new DataContext())
                {
                    context.UserMenus.Add(newUserMenu);

                    List<UserMenu> higherSqncUserMenus = context.UserMenus.Where(m => m.UserId == user.UserId && m.PageId != page.PageId && m.Sequence >= sequence).ToList();

                    foreach(UserMenu userMenu in higherSqncUserMenus)
                    {
                        userMenu.Sequence += 1;
                        context.Entry(userMenu).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }

                    context.SaveChanges();
                    Console.WriteLine($"\nA new user menu item for the {page.Name} has been added for user {user.FirstName} {user.LastName}.");
                }
            }
            else
            {
                Console.WriteLine("\nThe specified user already has a user menu item for this page.");
            }
            Console.WriteLine("\nPress any key to continue...\n");
            Console.ReadKey();
            Console.Clear();
            Main(new string[] { });
        }

        private static User GetUser()
        {
            using(DataContext context = new DataContext())
            {
                User user = null;
                string userName = string.Empty;

                do
                {
                    Console.WriteLine("\nWhat is the name of the user?  (e.g.: Fred Savage)\n");
                    userName = Console.ReadLine();
                    string[] names = userName.Split(' ');
                    user = context.Users.Where(u => u.FirstName == names[0] && u.LastName == names[1]).Include(u => u.UserMenus).ThenInclude(m => m.Page).FirstOrDefault();
                    if (user == null)
                        Console.WriteLine("\nInvalid input. Try again.");
                } while (user == null);

                return user;
            }            
        }

        private static Page GetPage()
        {
            using (DataContext context = new DataContext())
            {
                Page page = null;
                string pageName = string.Empty;

                do
                {
                    Console.WriteLine("\nWhat is the name of the page? (e.g.: Home)\n");
                    pageName = Console.ReadLine();
                    page = context.Pages.Where(p => p.Name == pageName).Include(p => p.UserMenus).ThenInclude(m => m.User).FirstOrDefault();
                    if (page == null)
                        Console.WriteLine("\nInvalid input. Try again.");
                } while (page == null);

                return page;
            }
        }

        private static int GetUserMenuOrderSequence(int attempts = 0)
        {
            if (attempts > 0)
                Console.WriteLine("\nInvalid input. Try again.");
            Console.WriteLine("\nWhat order sequence should this user menu item have?\n");
            string strSqnc = Console.ReadLine();
            int sequence = 0;
            if (Int32.TryParse(strSqnc, out sequence) && sequence > 0)
                return sequence;
            return GetUserMenuOrderSequence(++attempts);
        }

        private static void ViewUserMenuItems()
        {
            Console.Clear();
            User user = GetUser();
            if (user.UserMenus.Count > 0)
            {
                Console.WriteLine($"\nThese are the user menu items for {user.FirstName} {user.LastName}, in sequence:\n");
                for (int i = 1; i < user.UserMenus.Count; i++)
                    Console.WriteLine($"{i}.) Page: {user.UserMenus[i - 1].Page.Name}");
            }
            else
                Console.WriteLine($"\nThe user, {user.FirstName} {user.LastName}, doesn't have any user menu items.");

            Console.WriteLine("\nPress any key to continue...\n");
            Console.ReadKey();
            Console.Clear();
            Main(new string[] { });
        }

        private static void ViewUsersByPage()
        {
            Console.Clear();
            Page page = GetPage();
            if (page.UserMenus.Count > 0)
            {
                Console.WriteLine($"\nThese are the users that have user menu items associated with the {page.Name} page:\n");
                for (int i = 1; i < page.UserMenus.Count; i++)
                    Console.WriteLine($"{i}.) {page.UserMenus[i - 1].User.FirstName} {page.UserMenus[i - 1].User.LastName}");
            }
            else
                Console.WriteLine($"There are no users having user menu items associated with the {page.Name} page.");

            Console.WriteLine("\nPress any key to continue...\n");
            Console.ReadKey();
            Console.Clear();
            Main(new string[] { });
        }

        private static void Exit()
        {
            return;
        }
    }
}
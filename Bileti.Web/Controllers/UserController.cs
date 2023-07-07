using Bileti.Domain.DTO;
using Bileti.Domain.Identity;
using Bileti.Domain.Models;
using Bileti.Service;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<CustomUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserController(UserManager<CustomUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<bool> ImportUsersListAsync(List<CustomUser> model)
        {
            bool status = true;

            foreach (var item in model)
            {
                var userCheck = userManager.FindByEmailAsync(item.Email).Result;

                if (userCheck == null)
                {   
                    var user = new CustomUser
                    {
                        UserName = item.Email,
                        NormalizedUserName = item.Email,
                        Email = item.Email,
                        Password = item.Password,
                        SelectedRole = item.SelectedRole,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        UserCart = new ShoppingCart()
                    };
                    if (user.SelectedRole == null)
                    {
                        await roleManager.CreateAsync(new IdentityRole(item.SelectedRole));
                    }
                    var result = userManager.CreateAsync(user, item.Password).Result;
                    await userManager.AddToRoleAsync(user, item.SelectedRole);

                    status = status && result.Succeeded;
                }
                else
                {
                    continue;
                }
            }

            return status;
        }
        public IActionResult ImportAllUsers(IFormFile file)
        {

            //make a copy
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";


            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);

                fileStream.Flush();
            }

            //read data from uploaded file

            List<CustomUser> users = getUsersFromExcelFile(file.FileName);


            var result = ImportUsersListAsync(users);

            return RedirectToAction("Index", "User");
        }

        private List<CustomUser> getUsersFromExcelFile(string fileName)
        {

            string pathToFile = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            List<CustomUser> userList = new List<CustomUser>();

            using (var stream = System.IO.File.Open(pathToFile, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        userList.Add(new Bileti.Domain.Identity.CustomUser
                        {
                            Email = reader.GetValue(0).ToString(),
                            Password = reader.GetValue(1).ToString(),
                            SelectedRole = reader.GetValue(2).ToString(),
                        });
                    }
                }
            }

            return userList;

        }
    }
}


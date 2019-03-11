using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Workshop.Mesh.Frontend.Pages
{
    public class IndexModel : PageModel
    {
        private const string RootPath = "C:/data/";
        private const string UsedFolder = RootPath + "usedFolder/";
        private const string sourceTxt = RootPath + "soruce.txt";


        public string UnUsedCode { get; set; }
        [DataType(DataType.MultilineText)]
        [FromForm]
        public string PassesList { get; set; }
        [FromRoute]
        public bool Admin { get; set; } = false;

        public void OnGet(string admin = "")
        {
            if(admin == "paje")
            {
                this.Admin = true;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (this.Admin)
            {
                await newPassesList(PassesList);
                return Page();
            }
            else{
                var code = await GetNewPassCoe();
                if (String.IsNullOrWhiteSpace(code))
                {
                    return new StatusCodeResult(402);
                }
                else
                {
                    this.UnUsedCode = code;
                    return Page();
                }
            }
        }

        private static async Task newPassesList(string passesList)
        {
            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
            }
            if (!Directory.Exists(UsedFolder))
            {
                Directory.CreateDirectory(UsedFolder);
            }
            System.IO.File.Create(sourceTxt).Close();
            await System.IO.File.WriteAllTextAsync(sourceTxt, passesList);
        }

        private static async Task<string> GetNewPassCoe()
        {
            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
            }
            if (!Directory.Exists(UsedFolder))
            {
                Directory.CreateDirectory(UsedFolder);
            }
            if (!System.IO.File.Exists(sourceTxt))
            {
                return String.Empty;
            }
            var content = (await System.IO.File.ReadAllTextAsync(sourceTxt)).Split(Environment.NewLine);
            do
            {
                var unusedCode = content.FirstOrDefault(d =>
                {
                    return !System.IO.File.Exists(Path.Combine(UsedFolder, d));
                });
                if (unusedCode == null)
                {
                    return String.Empty;
                }

                try
                {
                    System.IO.File.Create(Path.Combine(UsedFolder, unusedCode));
                    return unusedCode;                    
                }
                catch (Exception)
                {

                }
            } while (true);
        }
    }
}

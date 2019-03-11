using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Workshop.Mesh.Frontend
{
    public class DefaultController : Controller
    {
        private const string RootPath = "C:/data/";
        private const string UsedFolder = RootPath + "usedFolder/";
        private const string sourceTxt = RootPath + "soruce.txt";
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }

        // GET: Default/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Default/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Default/Create
        [HttpPost]
        public async Task<ActionResult> Create(string collection)
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
                System.IO.File.Create(sourceTxt);
            }

            await System.IO.File.WriteAllTextAsync(sourceTxt, collection);
            return Ok();
        }

        // GET: Default/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Default/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Default/Delete/5
        public async Task<ActionResult> Delete()
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
                return new StatusCodeResult(402);
            }
            var content = (await System.IO.File.ReadAllTextAsync(sourceTxt)).Split(Environment.NewLine);
            do
            {
                var unusedCode = content.FirstOrDefault(d =>
                {
                    return !System.IO.File.Exists(Path.Combine(UsedFolder, d));
                });

                try
                {
                    System.IO.File.Create(Path.Combine(UsedFolder, unusedCode));
                    return View(unusedCode);
                }
                catch (Exception)
                {

                }
            } while (true);
        }

        // POST: Default/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
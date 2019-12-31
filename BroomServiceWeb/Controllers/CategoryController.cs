using BroomServiceWeb.Controllers.Web;
using BroomServiceWeb.Helpers;
using BroomServiceWeb.Models;
using BroomServiceWeb.Resources;
using BroomServiceWeb.Services;
using BroomServiceWeb.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BroomServiceWeb.Controllers
{
    [AuthAttribute]
    public class CategoryController : MyController
    {
        CategoryService categoryService;
        public CategoryController()
        {
            categoryService = new CategoryService();
        }

        #region Category
        public ActionResult CategoryList(int? pageNumber)
         {
            TempData["IndexPage"] = pageNumber ?? 1;
            var categories = categoryService.GetCategories().ToPagedList(pageNumber ?? 1, 10);
            return View(categories);
        }
        public ActionResult AddCategory(long? id, int? pageNumber)
        {
            TempData["IndexPage"] = pageNumber;
            TempData["ErrorMsg"] = null;
            TempData["SuccessMsg"] = null;
            TempData["Title"] = id == null || id == 0 ? Resource.add_service : Resource.edit_service;
            if (id != null)
            {
                var category = categoryService.GetCategoryById(id ?? 0);
                return View(category);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult AddCategory(CategoryViewModel model, HttpPostedFileBase file,
            HttpPostedFileBase fileIcon)
        {
            var pageNumber = TempData["IndexPage"];
            TempData["Title"] = model.Id == 0 ? Resource.add_service : Resource.edit_service;
            var date = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss").Replace("-", "_");
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };
                    var ext = System.IO.Path.GetExtension(file.FileName);

                    var fileName= Path.GetFileNameWithoutExtension(file.FileName) + "_" + date + ext.ToLower();
                    //var fileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName) + ext.ToLower();

                    if (!AllowedFileExtensions.Contains(ext))
                    {
                        ViewBag.ErrorMessage = Resource.file_extension_invalid;
                    }
                    ext = Path.GetExtension(file.FileName);
                    {
                        string path = "~/Images/Category";
                        if (!Directory.Exists(Server.MapPath(path)))
                            Directory.CreateDirectory(Server.MapPath(path));

                        path = System.IO.Path.Combine(Server.MapPath(path),
                                                   System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);
                        model.Picture = fileName;
                    }
                }
                if (fileIcon != null)
                {
                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };
                    var ext = System.IO.Path.GetExtension(fileIcon.FileName);

                    var fileName = System.IO.Path.GetFileNameWithoutExtension(fileIcon.FileName) + ext.ToLower();

                    if (!AllowedFileExtensions.Contains(ext))
                    {
                        ViewBag.ErrorMessage = Resource.file_extension_invalid;
                    }
                    ext = Path.GetExtension(fileIcon.FileName);
                    {
                        string path = "~/Images/Category";
                        if (!Directory.Exists(Server.MapPath(path)))
                            Directory.CreateDirectory(Server.MapPath(path));

                        path = System.IO.Path.Combine(Server.MapPath(path),
                                                   System.IO.Path.GetFileName(fileName));
                        fileIcon.SaveAs(path);
                        model.Icon = fileName;
                    }
                }
                var result = categoryService.AddUpdateCategory(model);
                if (result)
                {
                    TempData["SuccessMsg"] = categoryService.message;
                    return RedirectToAction("CategoryList", "Category", new { pageNumber = pageNumber });
                }
                else
                {
                    TempData["ErrorMsg"] = categoryService.message;
                }
            }
            return View(model);
        }

        public ActionResult UpdateCategoryStatus(long id, int? pageNumber)
        {
            if (categoryService.UpdateCategoryStatus(id))
            {
                TempData["SuccessMsg"] = categoryService.message;
            }
            else
            {
                TempData["ErrorMsg"] = categoryService.message;
            }
            return RedirectToAction("CategoryList", "Category", new { pageNumber = pageNumber });
        }
        #endregion

        #region SubCategory
        public ActionResult SubCategoryList(int? pageNumber)
        {
            TempData["IndexPage"] = pageNumber ?? 1;
            var subcategories = categoryService.GetSubCategories().ToPagedList(pageNumber ?? 1, 10); ;
            return View(subcategories);
        }

        public ActionResult AddSubCategory(long? id, int? pageNumber)
        {
            TempData["IndexPage"] = pageNumber;
            TempData["ErrorMsg"] = null;
            TempData["SuccessMsg"] = null;
            TempData["Title"] = id == 0 || id == null ? Resource.add_sub_service : Resource.edit_sub_service;
            var categories = categoryService.GetCategoriesSelect();
            ViewBag.Categories = categories;
            if (id != null)
            {
                var subcategory = categoryService.GetSubCategoryById(id ?? 0);
                return View(subcategory);
            }
            else
            {
                var model = new SubCategoryViewModel();
                if (categories.Count > 0)
                {
                    var catData = categoryService.GetCategoryById(Convert.ToInt64(categories[0].Value));
                    model.HasPrice = catData != null && catData.HasPrice == true
                        ? Resource.yes : Resource.no;
                }
                return View(model);
            }
        }
        //[HttpGet]
        //public JsonResult HasPrice(long subCatId)
        //{
        //    var status = categoryService.HasPrice(subCatId);
        //    return Json(status == true ? "yes" : "no", JsonRequestBehavior.AllowGet);
        //}
        //[HttpGet]
        //public JsonResult HasPriceFromCat(long catId)
        //{
        //    var catData = categoryService.GetCategoryById(catId);
        //    var status = catData != null && catData.HasPrice == true
        //        ? "yes" : "no";

        //    //var status = categoryService.GetCategoryById(catId).HasPrice;
        //    return Json(status  , JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult AddSubCategory(SubCategoryViewModel model, HttpPostedFileBase file,
            HttpPostedFileBase fileIcon)
        {
            var pageNumber = TempData["IndexPage"];
            TempData["Title"] = model.Id == 0 ? Resource.add_sub_service : Resource.edit_sub_service;
            if (ModelState.IsValid)
            {
                var date = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss").Replace("-", "_");
                if (file != null)
                {
                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };
                    var ext = System.IO.Path.GetExtension(file.FileName);

                    var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + date + ext.ToLower();
                    //var fileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName) + ext.ToLower();

                    if (!AllowedFileExtensions.Contains(ext))
                    {
                        ViewBag.ErrorMessage = Resource.file_extension_invalid;
                    }
                    ext = Path.GetExtension(file.FileName);
                    {
                        string path = "~/Images/SubCategory";
                        if (!Directory.Exists(Server.MapPath(path)))
                            Directory.CreateDirectory(Server.MapPath(path));

                        path = System.IO.Path.Combine(Server.MapPath(path),
                                                   System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);
                        model.Picture = fileName;
                    }
                }
                if (fileIcon != null)
                {
                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };
                    var ext = System.IO.Path.GetExtension(fileIcon.FileName);

                    var fileName = System.IO.Path.GetFileNameWithoutExtension(fileIcon.FileName) + ext.ToLower();

                    if (!AllowedFileExtensions.Contains(ext))
                    {
                        ViewBag.ErrorMessage = Resource.file_extension_invalid;
                    }
                    ext = Path.GetExtension(fileIcon.FileName);
                    {
                        string path = "~/Images/SubCategory";
                        if (!Directory.Exists(Server.MapPath(path)))
                            Directory.CreateDirectory(Server.MapPath(path));

                        path = System.IO.Path.Combine(Server.MapPath(path),
                                                   System.IO.Path.GetFileName(fileName));
                        fileIcon.SaveAs(path);
                        model.Icon = fileName;
                    }
                }
                model.HasSubSubCategories = model.HasSubSubCategoriesStr == "true" ? true : false;
                var result = categoryService.AddUpdateSubCategory(model);
                if (result)
                {
                    TempData["SuccessMsg"] = categoryService.message;
                    return RedirectToAction("SubCategoryList", "Category", new { pageNumber = pageNumber });
                }
                else
                {
                    TempData["ErrorMsg"] = categoryService.message;
                }
            }
            ViewBag.Categories = categoryService.GetCategoriesSelect();
            return View(model);
        }
        public ActionResult UpdateSubCategoryStatus(long id, int? pageNumber)
        {
            if (categoryService.UpdateSubCategoryStatus(id))
            {
                TempData["SuccessMsg"] = categoryService.message;
            }
            else
            {
                TempData["ErrorMsg"] = categoryService.message;
            }
            return RedirectToAction("SubCategoryList", "Category", new { pageNumber = pageNumber });
        }
        #endregion

        #region SubSubCategory
        public ActionResult SubSubCategoryList(int? pageNumber)
        {
            TempData["IndexPage"] = pageNumber ?? 1;
            var subcategories = categoryService.GetSubSubCategories().ToPagedList(pageNumber ?? 1, 10); ;
            return View(subcategories);
        }

        public ActionResult AddSubSubCategory(long? id, int? pageNumber)
        {
            TempData["IndexPage"] = pageNumber;
            TempData["ErrorMsg"] = null;
            TempData["SuccessMsg"] = null;
            TempData["Title"] = id == 0 || id == null ? Resource.add_sub_sub_service : Resource.edit_sub_sub_service;
            var subcategories = categoryService.GetSubCategoriesSelect();
            ViewBag.SubCategories = subcategories;
            if (id != null)
            {
                var subcategory = categoryService.GetSubSubCategoryById(id ?? 0);
                return View(subcategory);
            }
            else
            {
                var model = new SubSubCategoryViewModel();
                if (subcategories.Count > 0)
                {
                    var catData = categoryService.GetCategoryById(Convert.ToInt64(subcategories[0].Value));
                    model.HasPrice = catData != null && catData.HasPrice == true
                        ? Resource.yes : Resource.no;
                }
                return View(model);
            }
        }


        [HttpPost]
        public ActionResult AddSubSubCategory(SubSubCategoryViewModel model, HttpPostedFileBase file,
            HttpPostedFileBase fileIcon)
        {
            var pageNumber = TempData["IndexPage"];
            TempData["Title"] = model.Id == 0 ? Resource.add_sub_sub_service : Resource.edit_sub_sub_service;
            if (ModelState.IsValid)
            {
                var date = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss").Replace("-", "_");
                if (file != null)
                {
                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };
                    var ext = System.IO.Path.GetExtension(file.FileName);

                    var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + date + ext.ToLower();

                    //var fileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName) + ext.ToLower();

                    if (!AllowedFileExtensions.Contains(ext))
                    {
                        ViewBag.ErrorMessage = Resource.file_extension_invalid;
                    }
                    ext = Path.GetExtension(file.FileName);
                    {
                        string path = "~/Images/SubSubCategory";
                        if (!Directory.Exists(Server.MapPath(path)))
                            Directory.CreateDirectory(Server.MapPath(path));

                        path = System.IO.Path.Combine(Server.MapPath(path),
                                                   System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);
                        model.Picture = fileName;
                    }
                }
                if (fileIcon != null)
                {
                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".png", ".jpeg", ".JPG", ".PNG", ".JPEG" };
                    var ext = System.IO.Path.GetExtension(fileIcon.FileName);

                    var fileName = System.IO.Path.GetFileNameWithoutExtension(fileIcon.FileName) + ext.ToLower();

                    if (!AllowedFileExtensions.Contains(ext))
                    {
                        ViewBag.ErrorMessage = Resource.file_extension_invalid;
                    }
                    ext = Path.GetExtension(fileIcon.FileName);
                    {
                        string path = "~/Images/SubSubCategory";
                        if (!Directory.Exists(Server.MapPath(path)))
                            Directory.CreateDirectory(Server.MapPath(path));

                        path = System.IO.Path.Combine(Server.MapPath(path),
                                                   System.IO.Path.GetFileName(fileName));
                        fileIcon.SaveAs(path);
                        model.Icon = fileName;
                    }
                }
                var result = categoryService.AddUpdateSubSubCategory(model);
                if (result)
                {
                    TempData["SuccessMsg"] = categoryService.message;
                    return RedirectToAction("SubSubCategoryList", "Category", new { pageNumber = pageNumber });
                }
                else
                {
                    TempData["ErrorMsg"] = categoryService.message;
                }
            }
            ViewBag.SubCategories = categoryService.GetSubCategoriesSelect();
            return View(model);
        }
        public ActionResult UpdateSubSubCategoryStatus(long id, int? pageNumber)
        {
            if (categoryService.UpdateSubSubCategoryStatus(id))
            {
                TempData["SuccessMsg"] = categoryService.message;
            }
            else
            {
                TempData["ErrorMsg"] = categoryService.message;
            }
            return RedirectToAction("SubSubCategoryList", "Category", new { pageNumber = pageNumber });
        }
        #endregion
    }
}
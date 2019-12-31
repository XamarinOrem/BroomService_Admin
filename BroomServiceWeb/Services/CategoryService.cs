using BroomServiceWeb.Helpers;
using BroomServiceWeb.Models;
using BroomServiceWeb.Resources;
using BroomServiceWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BroomServiceWeb.Services
{
    public class CategoryService
    {
        BroomServiceEntities _db;
        public string message;
        public CategoryService()
        {
            _db = new BroomServiceEntities();
        }
        public List<CategoryViewModel> GetCategories(bool isActive = false)
        {
            List<CategoryViewModel> categories = new List<CategoryViewModel>();
            try
            {
                categories = _db.Categories.Select(x => new CategoryViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Name_French = x.Name_French,
                    Name_Hebrew = x.Name_Hebrew,
                    Name_Russian = x.Name_Russian,
                    Description=x.Description,
                    Description_French = x.Description_French,
                    Description_Hebrew = x.Description_Hebrew,
                    Description_Russian = x.Description_Russian,
                    Picture = x.Picture,
                    //HasPrice=x.HasPrice??false,
                    IsActive = x.IsActive,
                    Icon = x.Icon,
                    ForWorkers = x.ForWorkers ?? false,
                    DisplayOrder = x.DisplayOrder
                }).OrderBy(z => z.DisplayOrder ?? 100000).ToList();
                if (isActive)
                {
                    categories = categories.Where(x => x.IsActive == true).ToList();
                }
            }
            catch (Exception ex)
            {
            }
            return categories;
        }
        public List<SubSubCategoryViewModel> GetSubSubCategoriesByID(int subCatId)
        {
            List<SubSubCategoryViewModel> subsubcategories = new List<SubSubCategoryViewModel>();
            try
            {
                subsubcategories = _db.SubSubCategories.Where(A => A.IsActive == true && A.SubCatId == subCatId).Select(x => new SubSubCategoryViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    SubCategoryId = x.SubCategory.Id,
                    SubCategoryName = x.SubCategory.Name,
                    Name_French=x.Name_French,
                    Name_Hebrew = x.Name_Hebrew,
                    Name_Russian = x.Name_Russian,
                    Description = x.Description,
                    Description_Hebrew = x.Description_Hebrew,
                    Description_French = x.Description_French,
                    Description_Russian = x.Description_Russian,
                    //HasPrice = x.Category.HasPrice ?? false,
                    IsActive = x.IsActive,
                    Picture = x.Picture,
                    Icon = x.Icon,
                    Price = x.Price,
                    ClientPrice = x.ClientPrice,
                }).ToList();
            }
            catch (Exception ex)
            {
            }
            return subsubcategories;
        }
        public List<CategoryViewModel> GetCatSubCategories(int usertype)
        {
            List<CategoryViewModel> categories = new List<CategoryViewModel>();
            try
            {
                bool catForWorkers = false;
                if (usertype == (int)Enums.UserTypeEnum.Worker)
                {
                    catForWorkers = true;
                }
                categories = _db.Categories.Where(x => x.IsActive == true
            && x.ForWorkers == catForWorkers).Select(x => new CategoryViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Picture = x.Picture,
                    //HasPrice = x.HasPrice ?? false,
                    IsActive = x.IsActive,
                Icon = x.Icon,
                SubCategories = x.SubCategories.Where(y => y.IsActive == true).Select(y => new SubCategoryViewModel()
                {
                    Id = y.Id,
                    Name = y.Name,
                    Icon = y.Icon,
                    Picture = y.Picture,
                    Price = y.Price,
                    HasSubSubCategories = y.HasSubSubCategories ?? false,
                    SubSubCategories = y.SubSubCategories.Where(z => z.IsActive == true).Select(z => new SubSubCategoryViewModel()
                    {
                        Id = z.Id,
                        Name = z.Name,
                        Icon = z.Icon,
                        Picture = z.Picture,
                        Price = z.Price
                    }).ToList()
                }).ToList(),
            }).ToList();
            }
            catch (Exception ex)
            {
            }
            return categories;
        }
        public List<CategoryViewModelApp> GetCatSubCategoriesApp()
        {
            List<CategoryViewModelApp> categories = new List<CategoryViewModelApp>();
            try
            {
                categories = _db.Categories.Where(x => x.IsActive == true).OrderBy(p => p.DisplayOrder ?? 100000).Select(x => new CategoryViewModelApp()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Name_French = x.Name_French,
                    Name_Hebrew = x.Name_Hebrew,
                    Name_Russian = x.Name_Russian,
                    Description=x.Description,
                    Description_French = x.Description_French,
                    Description_Hebrew = x.Description_Hebrew,
                    Description_Russian = x.Description_Russian,
                    Picture = x.Picture,
                    //HasPrice = x.HasPrice ?? false, 
                    Icon = x.Icon,
                    SubCategories = x.SubCategories.Where(y => y.IsActive == true).Select(y => new SubCategoryViewModelApp()
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Name_French = y.Name_French,
                        Name_Hebrew = y.Name_Hebrew,
                        Name_Russian = y.Name_Russian,
                        Description = y.Description,
                        Description_French = y.Description_French,
                        Description_Hebrew = y.Description_Hebrew,
                        Description_Russian = y.Description_Russian,
                        Icon = y.Icon,
                        Picture = y.Picture,
                        Price = y.Price,
                        ClientPrice = y.ClientPrice,
                        HasSubSubCategories = y.HasSubSubCategories ?? false,
                        SubSubCategories = y.SubSubCategories.Where(z => z.IsActive == true).Select(z => new SubSubCategoryViewModelApp()
                        {
                            Id = z.Id,
                            Name = z.Name,
                            Name_French = z.Name_French,
                            Name_Hebrew = z.Name_Hebrew,
                            Name_Russian = z.Name_Russian,
                            Description = y.Description,
                            Description_French = y.Description_French,
                            Description_Hebrew = y.Description_Hebrew,
                            Description_Russian = y.Description_Russian,
                            Icon = z.Icon,
                            Picture = z.Picture,
                            Price = z.Price,
                            ClientPrice = z.ClientPrice,
                        }).ToList()
                    }).ToList(),
                }).ToList();
            }
            catch (Exception ex)
            {
            }
            return categories;
        }
        public List<SelectListItem> GetCategoriesSelect()
        {
            var culVal = System.Globalization.CultureInfo.CurrentCulture.Name;

            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems = _db.Categories.Where(x => x.IsActive == true)
                .Select(x => new SelectListItem
                {
                    Text = culVal == "fr-FR" ? x.Name_French
                    : culVal == "ru-RU" ? x.Name_Russian
                    : culVal == "he-IL" ? x.Name_Hebrew
                    : x.Name,
                    Value = x.Id.ToString(),
                }).ToList();
            return listItems;
        }
        public List<SelectListItem> GetSubCategoriesSelect()
        {
            var culVal = System.Globalization.CultureInfo.CurrentCulture.Name;
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems = _db.SubCategories.Where(x => x.IsActive == true
            && x.HasSubSubCategories == true)
                .Select(x => new SelectListItem
                {
                    Text = culVal == "fr-FR" ? x.Name_French
                    : culVal == "ru-RU" ? x.Name_Russian
                    : culVal == "he-IL" ? x.Name_Hebrew
                    : x.Name,
                    Value = x.Id.ToString(),
                }).ToList();
            return listItems;
        }
        public CategoryViewModel GetCategoryById(long id)
        {
            CategoryViewModel category = new CategoryViewModel();
            try
            {
                category = _db.Categories.Where(x => x.Id == id).Select(x => new CategoryViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Name_French = x.Name_French,
                    Name_Hebrew = x.Name_Hebrew,
                    Name_Russian = x.Name_Russian,
                    Picture = x.Picture,
                    Description=x.Description,
                    Description_French=x.Description_French,
                    Description_Hebrew = x.Description_Hebrew,
                    Description_Russian = x.Description_Russian,
                    //HasPrice = x.HasPrice ?? false,
                    IsActive = x.IsActive,
                    Icon = x.Icon,
                    ForWorkers = x.ForWorkers ?? false,
                    DisplayOrder = x.DisplayOrder
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }
            return category;
        }

        public CategoryViewModel GetCategoryBySubCatId(long id)
        {
            CategoryViewModel category = new CategoryViewModel();
            try
            {
                category = _db.SubCategories.Where(x => x.Id == id).Select(x => new CategoryViewModel()
                {
                    Id = x.Category.Id,
                    Name = x.Name,
                    Name_French = x.Name_French,
                    Name_Hebrew = x.Name_Hebrew,
                    Name_Russian = x.Name_Russian,
                    Description = x.Description,
                    Description_French = x.Description_French,
                    Description_Hebrew = x.Description_Hebrew,
                    Description_Russian = x.Description_Russian,
                    Picture = x.Picture,
                    // HasPrice = x.Category.HasPrice ?? false,
                    IsActive = x.IsActive,
                    Icon = x.Icon,
                    ForWorkers = x.Category.ForWorkers ?? false
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }
            return category;
        }
        //public bool HasPrice(long subCatId)
        //{
        //    bool status = false;
        //    try
        //    {
        //        status = _db.SubCategories.Where(x => x.Id == subCatId).FirstOrDefault().Category.HasPrice??false;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return status;
        //}

        public bool AddUpdateCategory(CategoryViewModel model)
        {
            bool status = false;
            try
            {
                if (model.Id != 0)
                {
                    var catData = _db.Categories.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (catData != null)
                    {

                        var validName = _db.Categories.Where(x => x.Name.ToLower().Trim() ==
                        model.Name.ToLower().Trim() && x.Id != model.Id).FirstOrDefault();
                        if (validName != null)
                        {
                            message = Resource.service_name_already_exists;
                        }
                        else
                        {
                            catData.Name = model.Name;
                            catData.Name_French = model.Name_French;
                            catData.Name_Hebrew = model.Name_Hebrew;
                            catData.Name_Russian = model.Name_Russian;
                            catData.Description = model.Description;
                            catData.Description_French = model.Description_French;
                            catData.Description_Hebrew = model.Description_Hebrew;
                            catData.Description_Russian = model.Description_Russian;
                            catData.Picture = !string.IsNullOrEmpty(model.Picture) ? model.Picture : catData.Picture;
                            catData.Icon = !string.IsNullOrEmpty(model.Icon) ? model.Icon : catData.Icon;
                            //catData.HasPrice = model.HasPrice;
                            catData.ForWorkers = model.ForWorkers;
                            catData.DisplayOrder = model.DisplayOrder;
                            catData.ModifiedDate = DateTime.Now;
                            _db.SaveChanges();
                            status = true;
                            message = Resource.service_update_success;
                        }
                    }
                    else
                    {
                        message = Resource.service_not_exists;
                    }
                }
                else
                {
                    var validName = _db.Categories.Where(x => x.Name.ToLower().Trim() ==
                    model.Name.ToLower().Trim()).FirstOrDefault();
                    if (validName != null)
                    {
                        message = Resource.service_name_already_exists;
                    }
                    else
                    {
                        Category category = new Category();
                        category.CreatedDate = DateTime.Now;
                        category.IsActive = true;
                        //category.HasPrice = model.HasPrice;
                        category.ForWorkers = model.ForWorkers;
                        category.Name = model.Name;
                        category.Name_French = model.Name_French;
                        category.Name_Hebrew = model.Name_Hebrew;
                        category.Name_Russian = model.Name_Russian;
                        category.Description = model.Description;
                        category.Description_French = model.Description_French;
                        category.Description_Hebrew = model.Description_Hebrew;
                        category.Description_Russian = model.Description_Russian;
                        category.Picture = model.Picture;
                        category.Icon = model.Icon;
                        category.DisplayOrder = model.DisplayOrder;
                        _db.Categories.Add(category);
                        _db.SaveChanges();

                        status = true;
                        message = Resource.service_add_success;
                    }

                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        public bool UpdateCategoryStatus(long id)
        {
            bool status = false;
            try
            {
                var catData = _db.Categories.Where(x => x.Id == id).FirstOrDefault();
                if (catData != null)
                {
                    if (catData.IsActive == true)
                    {
                        catData.IsActive = false;
                    }
                    else
                    {
                        catData.IsActive = true;
                    }
                    _db.SaveChanges();
                    message = catData.Name + Resource.status_update_sucess;
                    status = true;
                }
                else
                {
                    message = Resource.service_not_exists;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        public List<SubCategoryViewModel> GetSubCategoriesByCatId(long catId)        {            List<SubCategoryViewModel> subcategories = new List<SubCategoryViewModel>();            try            {                subcategories = _db.SubCategories.Where(x => x.IsActive == true && x.CatId == catId)                    .Select(x => new SubCategoryViewModel()                    {                        Id = x.Id,                        Name = x.Name,
                        Name_French = x.Name_French,
                        Name_Hebrew = x.Name_Hebrew,
                        Name_Russian = x.Name_Russian,                        Description=x.Description,                        Description_French=x.Description_French,                        Description_Hebrew=x.Description_Hebrew,                        Description_Russian=x.Description_Russian,                        CategoryId = x.Category.Id,                        CategoryName = x.Category.Name,                        Price = x.Price,                        ClientPrice = x.ClientPrice,                        IsActive = x.IsActive,                        Picture = x.Picture,                        Icon = x.Icon                    }).ToList();            }            catch (Exception ex)            {            }            return subcategories;        }
        public List<SubCategoryViewModel> GetSubCategories()
        {
            List<SubCategoryViewModel> subcategories = new List<SubCategoryViewModel>();
            try
            {
                subcategories = _db.SubCategories.Select(x => new SubCategoryViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Name_French = x.Name_French,
                    Name_Hebrew = x.Name_Hebrew,
                    Name_Russian = x.Name_Russian,
                    Description=x.Description,
                    Description_French=x.Description_French,
                    Description_Russian=x.Description_Russian,
                    Description_Hebrew=x.Description_Hebrew,
                    CategoryId = x.Category.Id,
                    CategoryName = x.Category.Name,
                    //HasPrice = x.Category.HasPrice ?? false,
                    IsActive = x.IsActive,
                    Picture = x.Picture,
                    Icon = x.Icon
                }).ToList();
            }
            catch (Exception ex)
            {
            }
            return subcategories;
        }
        public List<SubSubCategoryViewModel> GetSubSubCategories()
        {
            List<SubSubCategoryViewModel> subsubcategories = new List<SubSubCategoryViewModel>();
            try
            {
                subsubcategories = _db.SubSubCategories.Select(x => new SubSubCategoryViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Name_French = x.Name_French,
                    Name_Hebrew = x.Name_Hebrew,
                    Name_Russian = x.Name_Russian,
                    Description_Hebrew=x.Description_Hebrew,
                    Description = x.Description,
                    Description_French = x.Description_French,
                    Description_Russian = x.Description_Russian,
                    SubCategoryId = x.SubCategory.Id,
                    SubCategoryName = x.SubCategory.Name,
                    //HasPrice = x.Category.HasPrice ?? false,
                    IsActive = x.IsActive,
                    Picture = x.Picture,
                    Icon = x.Icon
                }).ToList();
            }
            catch (Exception ex)
            {
            }
            return subsubcategories;
        }
        public SubCategoryViewModel GetSubCategoryById(long id)
        {
            SubCategoryViewModel category = new SubCategoryViewModel();
            try
            {
                category = _db.SubCategories.Where(x => x.Id == id).Select(x => new SubCategoryViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Name_French = x.Name_French,
                    Name_Hebrew = x.Name_Hebrew,
                    Name_Russian = x.Name_Russian,
                    Description_Hebrew = x.Description_Hebrew,
                    Description = x.Description,
                    Description_French = x.Description_French,
                    Description_Russian = x.Description_Russian,
                    IsActive = x.IsActive,
                    CategoryId = x.Category.Id,
                    CategoryName = x.Category.Name,
                    //  HasPrice= x.Category.HasPrice == true
                    //     ? "yes" : "no", 
                    Picture = x.Picture,
                    Icon = x.Icon,
                    HasSubSubCategories = x.HasSubSubCategories ?? false,
                    Price = x.Price,
                    ClientPrice = x.ClientPrice
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }
            return category;
        }
        public bool AddUpdateSubCategory(SubCategoryViewModel model)
        {
            bool status = false;
            try
            {
                if (model.Id != 0)
                {
                    var subCatData = _db.SubCategories.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (subCatData != null)
                    {

                        var validName = _db.SubCategories.Where(x => x.Name.ToLower().Trim() ==
                        model.Name.ToLower().Trim() && x.Id != model.Id && x.CatId == model.CategoryId).FirstOrDefault();
                        if (validName != null)
                        {
                            message = Resource.sub_service_name_already_exists;
                        }
                        else
                        {
                            subCatData.Name = model.Name;
                            subCatData.Name_French = model.Name_French;
                            subCatData.Name_Hebrew = model.Name_Hebrew;
                            subCatData.Name_Russian = model.Name_Russian;
                            subCatData.Description = model.Description;
                            subCatData.Description_French = model.Description_French;
                            subCatData.Description_Hebrew = model.Description_Hebrew;
                            subCatData.Description_Russian = model.Description_Russian;
                            subCatData.CatId = model.CategoryId;
                            subCatData.Price = model.Price;
                            subCatData.ClientPrice = model.ClientPrice;
                            subCatData.Picture = !string.IsNullOrEmpty(model.Picture) ? model.Picture : subCatData.Picture;
                            subCatData.Icon = !string.IsNullOrEmpty(model.Icon) ? model.Icon : subCatData.Icon;
                            subCatData.ModifiedDate = DateTime.Now;
                            subCatData.HasSubSubCategories = model.HasSubSubCategories;
                            _db.SaveChanges();
                            status = true;
                            message = Resource.sub_service_update_success;
                            status = true;
                        }
                    }
                    else
                    {
                        message = Resource.subcategory_not_exists;
                    }
                }
                else
                {
                    var validName = _db.SubCategories.Where(x => x.Name.ToLower().Trim() ==
                    model.Name.ToLower().Trim() && x.CatId == model.CategoryId).FirstOrDefault();
                    if (validName != null)
                    {
                        message = Resource.sub_service_name_already_exists;
                    }
                    else
                    {
                        SubCategory subcategory = new SubCategory();
                        subcategory.CreatedDate = DateTime.Now;
                        subcategory.IsActive = true;
                        subcategory.CatId = model.CategoryId;
                        subcategory.Name = model.Name;
                        subcategory.Name_French = model.Name_French;
                        subcategory.Name_Hebrew = model.Name_Hebrew;
                        subcategory.Name_Russian = model.Name_Russian;
                        subcategory.Description = model.Description;
                        subcategory.Description_French = model.Description_French;
                        subcategory.Description_Hebrew = model.Description_Hebrew;
                        subcategory.Description_Russian = model.Description_Russian;
                        subcategory.Price = model.Price;
                        subcategory.ClientPrice = model.ClientPrice;
                        subcategory.Picture = model.Picture;
                        subcategory.Icon = model.Icon;
                        subcategory.HasSubSubCategories = model.HasSubSubCategories;
                        _db.SubCategories.Add(subcategory);
                        _db.SaveChanges();

                        status = true;
                        message = Resource.sub_service_add_success;
                    }

                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        public bool UpdateSubCategoryStatus(long id)
        {
            bool status = false;
            try
            {
                var catData = _db.SubCategories.Where(x => x.Id == id).FirstOrDefault();
                if (catData != null)
                {
                    if (catData.IsActive == true)
                    {
                        catData.IsActive = false;
                    }
                    else
                    {
                        catData.IsActive = true;
                    }
                    _db.SaveChanges();
                    message = catData.Name + Resource.status_update_sucess;
                }
                else
                {
                    message = Resource.subcategory_not_exists;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }




        public List<SubSubCategoryViewModel> GetubSubCategories()
        {
            List<SubSubCategoryViewModel> subsubcategories = new List<SubSubCategoryViewModel>();
            try
            {
                subsubcategories = _db.SubSubCategories.Select(x => new SubSubCategoryViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Name_French = x.Name_French,
                    Name_Hebrew = x.Name_Hebrew,
                    Name_Russian = x.Name_Russian,
                    Description = x.Description,
                    Description_French = x.Description_French,
                    Description_Hebrew = x.Description_Hebrew,
                    Description_Russian = x.Description_Russian,
                    SubCategoryId = x.SubCategory.Id,
                    SubCategoryName = x.SubCategory.Name,
                    //HasPrice = x.Category.HasPrice ?? false,
                    IsActive = x.IsActive,
                    Picture = x.Picture,
                    Icon = x.Icon
                }).ToList();
            }
            catch (Exception ex)
            {
            }
            return subsubcategories;
        }
        public SubSubCategoryViewModel GetSubSubCategoryById(long id)
        {
            SubSubCategoryViewModel subcategory = new SubSubCategoryViewModel();
            try
            {
                subcategory = _db.SubSubCategories.Where(x => x.Id == id).Select(x => new SubSubCategoryViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Name_French = x.Name_French,
                    Name_Hebrew = x.Name_Hebrew,
                    Name_Russian = x.Name_Russian,
                    Description = x.Description,
                    Description_French = x.Description_French,
                    Description_Hebrew = x.Description_Hebrew,
                    Description_Russian = x.Description_Russian,
                    IsActive = x.IsActive,
                    SubCategoryId = x.SubCategory.Id,
                    SubCategoryName = x.SubCategory.Name,
                    //HasPrice = x.SubCategory.Category.HasPrice == true
                    //   ? "yes" : "no",
                    Price = x.Price,
                    ClientPrice = x.ClientPrice,
                    Picture = x.Picture,
                    Icon = x.Icon
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }
            return subcategory;
        }
        public bool AddUpdateSubSubCategory(SubSubCategoryViewModel model)
        {
            bool status = false;
            try
            {
                if (model.Id != 0)
                {
                    var subCatData = _db.SubSubCategories.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (subCatData != null)
                    {

                        var validName = _db.SubSubCategories.Where(x => x.Name.ToLower().Trim() ==
                        model.Name.ToLower().Trim() && x.Id != model.Id && x.SubCatId == model.SubCategoryId).FirstOrDefault();
                        if (validName != null)
                        {
                            message = Resource.sub_sub_service_name_already_exists;
                        }
                        else
                        {
                            subCatData.Name = model.Name;
                            subCatData.Name_French = model.Name_French;
                            subCatData.Name_Hebrew = model.Name_Hebrew;
                            subCatData.Name_Russian = model.Name_Russian;
                            subCatData.Description = model.Description;
                            subCatData.Description_French = model.Description_French;
                            subCatData.Description_Hebrew = model.Description_Hebrew;
                            subCatData.Description_Russian = model.Description_Russian;
                            subCatData.Price = model.Price;
                            subCatData.ClientPrice = model.ClientPrice;
                            subCatData.SubCatId = model.SubCategoryId;
                            subCatData.Picture = !string.IsNullOrEmpty(model.Picture) ? model.Picture : subCatData.Picture;
                            subCatData.Icon = !string.IsNullOrEmpty(model.Icon) ? model.Icon : subCatData.Icon;
                            subCatData.ModifiedDate = DateTime.Now;
                            _db.SaveChanges();
                            status = true;
                            message = Resource.sub_sub_service_update_success;
                            status = true;
                        }
                    }
                    else
                    {
                        message = Resource.sub_sub_service_not_exists;
                    }
                }
                else
                {
                    var validName = _db.SubSubCategories.Where(x => x.Name.ToLower().Trim() ==
                    model.Name.ToLower().Trim() && x.SubCatId == model.SubCategoryId).FirstOrDefault();
                    if (validName != null)
                    {
                        message = Resource.sub_sub_service_name_already_exists;
                    }
                    else
                    {
                        SubSubCategory subsubcategory = new SubSubCategory();
                        subsubcategory.CreatedDate = DateTime.Now;
                        subsubcategory.IsActive = true;
                        subsubcategory.Price = model.Price;
                        subsubcategory.ClientPrice = model.ClientPrice;
                        subsubcategory.SubCatId = model.SubCategoryId;
                        subsubcategory.Name = model.Name;
                        subsubcategory.Name_French = model.Name_French;
                        subsubcategory.Name_Hebrew = model.Name_Hebrew;
                        subsubcategory.Name_Russian = model.Name_Russian;
                        subsubcategory.Description = model.Description;
                        subsubcategory.Description_French = model.Description_French;
                        subsubcategory.Description_Hebrew = model.Description_Hebrew;
                        subsubcategory.Description_Russian = model.Description_Russian;
                        subsubcategory.Picture = model.Picture;
                        subsubcategory.Icon = model.Icon;
                        _db.SubSubCategories.Add(subsubcategory);
                        _db.SaveChanges();

                        status = true;
                        message = Resource.sub_sub_service_add_success;
                    }

                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        public bool UpdateSubSubCategoryStatus(long id)
        {
            bool status = false;
            try
            {
                var catData = _db.SubSubCategories.Where(x => x.Id == id).FirstOrDefault();
                if (catData != null)
                {
                    if (catData.IsActive == true)
                    {
                        catData.IsActive = false;
                    }
                    else
                    {
                        catData.IsActive = true;
                    }
                    _db.SaveChanges();
                    message = catData.Name + Resource.status_update_sucess;
                }
                else
                {
                    message = Resource.sub_sub_service_not_exists;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
    }
}
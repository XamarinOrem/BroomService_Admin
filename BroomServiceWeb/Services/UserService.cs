using BroomServiceWeb.Helpers;
using BroomServiceWeb.Models;
using BroomServiceWeb.Resources;
using BroomServiceWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BroomServiceWeb.Services
{
    public class UserService
    {
        BroomServiceEntities _db;
        public UserService()
        {
            _db = new BroomServiceEntities();
        }
        public string message;
        #region Admin Methods
        public UserModel LoginAdminUser(string email, string password)
        {
            UserModel userModel = new UserModel();
            try
            {
                var usertypeA = Enums.UserTypeEnum.Admin.GetHashCode();

                var user = _db.Users.Where(x => x.Email == email && x.IsActive == true && (x.UserType == usertypeA)).FirstOrDefault();
                if (user != null)
                {
                    var gg = Common.DecryptString(user.Password, user.PasswordSalt);
                    if (Common.EncryptString(password, user.PasswordSalt) == user.Password)
                    {
                        userModel.UserId = user.UserId;
                        userModel.Name = user.FirstName + " " + user.LastName;
                        userModel.UserType = user.UserType;
                    }
                }
            }
            catch (Exception ex)
            {
                userModel = new UserModel();
            }
            return userModel;
        }
        public UserModel GetAdminUserDetail(long userId)
        {
            UserModel userModel = new UserModel();
            try
            {
                userModel = _db.Users.Where(x => x.UserId == userId).Select(x => new UserModel()
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email
                }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                userModel = new UserModel();
            }
            return userModel;
        }
        public long GetAdminId()
        {
            long userId = 0;
            try
            {
                var usertypeA = Enums.UserTypeEnum.Admin.GetHashCode();

                var user = _db.Users.Where(x => x.UserType == usertypeA).FirstOrDefault();
                if (user != null)
                {
                    userId = user.UserId;
                }
            }
            catch (Exception ex)
            {
            }
            return userId;
        }
        public bool UpdateAdminProfile(ProfileViewModel model)
        {
            bool status = false;
            try
            {
                var user = _db.Users.Where(x => x.UserId == model.UserId).FirstOrDefault();
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    _db.SaveChanges();
                    message = Resource.profile_update_success;
                    status = true;
                }
                else
                {
                    message = Resource.some_error_occured;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }



        public List<SelectListItem> GetWorkers(long requestId)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            try
            {
                var jobReq = _db.JobRequests.Where(x => x.Id == requestId).FirstOrDefault();
                if (jobReq != null)
                {
                    var userIds = new List<long?>();
                    var jobSubSubCategories = jobReq.JobRequestSubSubCategories.
                        Select(x => x.SubSubCategoryId).ToList();
                    if (jobSubSubCategories != null && jobSubSubCategories.Count > 0)
                    {
                        userIds = _db.UserSubCategories.Where(x => jobSubSubCategories.Contains(x.SubSubCategoryId ?? 0))
                            .Select(y => y.UserId).ToList();
                    }
                    else
                    {
                        userIds = _db.UserSubCategories.Where(x => x.SubCategoryId == jobReq.SubCategoryId)
                            .Select(y => y.UserId).ToList();
                    }

                    listItems = _db.Users.Where(x => x.IsActive == true && x.EmailVerified == true &&
                      x.UserType == (int)Enums.UserTypeEnum.Worker && userIds.Contains(x.UserId))
                        .Select(x => new SelectListItem
                        {
                            Text = x.FirstName + " " + x.LastName,
                            Value = x.UserId.ToString(),
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
            }
            return listItems;
        }
        #endregion
        public List<CountryModel> GetCountries()
        {
            List<CountryModel> data = null;
            try
            {
                data = _db.Countries.Where(x => x.IsActive == true).Select(x => new CountryModel()
                {
                    CountryId = x.CountryId,
                    CountryName = x.Name,
                    CountryCode = x.CountryCode
                }).ToList();
            }
            catch (Exception ex)
            {
            }
            return data;
        }
        public List<UserModel> GetUsers(Enums.UserTypeEnum usertype)
        {
            List<UserModel> users = new List<UserModel>();
            try
            {
                users = _db.Users.Where(x => x.UserType == (int)usertype).Select(x => new UserModel()
                {
                    UserId = x.UserId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    PaymentMethod = x.PaymentMethod
                }).ToList();
            }
            catch (Exception ex)
            {
            }
            return users;
        }
        public int GetCustomerPaymentMethod(long customerId)
        {
            int paymentMethod = 0;
            try
            {

                var user = _db.Users.Where(x => x.UserId == customerId).FirstOrDefault();
                if (user != null)
                {
                    paymentMethod = user.PaymentMethod ?? (int)Enums.PaymentMethod.ByCreditCard;
                }
                else
                {
                    paymentMethod = (int)Enums.PaymentMethod.ByCreditCard;
                }
                message = Resource.success;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return paymentMethod;
        }
        public bool UpdatePaymentMethod(long customerId, int paymentmethod)
        {
            bool status = false;
            try
            {
                var user = _db.Users.Where(x => x.UserId == customerId).FirstOrDefault();
                if (user != null)
                {
                    user.PaymentMethod = paymentmethod;
                    _db.SaveChanges();
                    message = Resource.info_update_success;
                    status = true;
                }
                else
                {
                    message = Resource.customer_not_exists;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        public UserViewModel GetUserDetail(long? userId)
        {
            UserViewModel userModel = new UserViewModel();
            try
            {
                //userModel = _db.Users.Where(x => x.UserId == userId).Select(x => new UserViewModel()
                //{
                //    UserId=x.UserId,
                //    FirstName = x.FirstName,
                //    LastName = x.LastName,
                //    PhoneNumber = x.PhoneNumber,
                //    Email = x.Email,
                //    Address=x.Address, 
                //}).FirstOrDefault();
                var user = _db.Users.Where(x => x.UserId == userId).FirstOrDefault();
                bool catForWorkers = false;
                if (user.UserType == (int)Enums.UserTypeEnum.Worker)
                {
                    catForWorkers = true;
                }

                var userCats = user.UserSubCategories.Select(x => x.SubCategory.Category).Select(y => y.Id).ToList();
                var userSubCats = user.UserSubCategories.Select(x => x.SubCategoryId ?? 0).ToList();
                var userSubSubCats = user.UserSubCategories.Select(x => x.SubSubCategoryId ?? 0).ToList();
                if (user != null)
                {
                    userModel = new UserViewModel()
                    {
                        UserId = user.UserId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        Address = user.Address,
                        UserType = user.UserType
                    };

                    userModel.Categories = _db.Categories.Where(x => x.IsActive == true
                    && x.ForWorkers == catForWorkers).Select(x => new CategoryViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Picture = x.Picture,
                        // HasPrice = x.HasPrice ?? false,
                        IsActive = x.IsActive,
                        IsSelected = userCats.Contains(x.Id),
                        SubCategories = x.SubCategories.Where(y => y.IsActive == true).Select(y => new SubCategoryViewModel()
                        {
                            Id = y.Id,
                            Name = y.Name,
                            IsSelected = userSubCats.Contains(y.Id),
                            SubSubCategories = y.SubSubCategories.Where(z => z.IsActive == true).Select(z => new SubSubCategoryViewModel()
                            {
                                Id = z.Id,
                                Name = z.Name,
                                IsSelected = userSubSubCats.Contains(z.Id),
                            }).ToList()
                        }).ToList()
                    }).ToList();


                }
            }
            catch (Exception ex)
            {
                userModel = new UserViewModel();
            }
            return userModel;
        }
        public CustomerDetail GetCustomerDetail(long? userId)
        {
            CustomerDetail userModel = new CustomerDetail();
            try
            {
                var user = _db.Users.Where(x => x.UserId == userId).FirstOrDefault();
                var userCats = user.UserSubCategories.Select(x => x.SubCategory.Category).Select(y => y.Id).ToList();
                var userSubCats = user.UserSubCategories.Select(x => x.SubCategoryId ?? 0).ToList();
                var userSubSubCats = user.UserSubCategories.Select(x => x.SubSubCategoryId ?? 0).ToList();
                if (user != null)
                {
                    userModel.User = new UserViewModel()
                    {
                        UserId = user.UserId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        Address = user.Address,
                        UserType = user.UserType,
                        PicturePath = user.PicturePath,
                    };

                    userModel.Properties = _db.Properties.Where(x => x.CreatedBy == userId)
                        .Select(x => new PropertyModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            ApartmentNumber = x.ApartmentNumber,
                            AccessToProperty = x.AccessToProperty,
                            Address = x.Address,
                            Balcony = x.Balcony,
                            BuildingCode = x.BuildingCode,
                            CreatedDate = x.CreatedDate,
                            CreatedBy = x.CreatedBy,
                            Dishwasher = x.Dishwasher,
                            Doorman = x.Doorman,
                            DuvetSize = x.DuvetSize,
                            Elevator = x.Elevator,
                            FloorNumber = x.FloorNumber,
                            Garden = x.Garden,
                            Latitude = x.Latitude,
                            Longitude = x.Longitude,
                            NoOfBathrooms = x.NoOfBathrooms,
                            NoOfDoubleBeds = x.NoOfDoubleBeds,
                            NoOfDoubleSofaBeds = x.NoOfDoubleSofaBeds,
                            NoOfDuvet = x.NoOfDuvet,
                            NoOfPillows = x.NoOfPillows,
                            NoOfQueenBeds = x.NoOfQueenBeds,
                            NoOfSingleBeds = x.NoOfSingleBeds,
                            NoOfSingleSofaBeds = x.NoOfSingleSofaBeds,
                            Parking = x.Parking,
                            Pool = x.Pool,
                            ShortTermApartment = x.ShortTermApartment,
                            Type = x.Type,
                            IsActive = x.IsActive
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                userModel = new CustomerDetail();
            }
            return userModel;
        }
        public WorkerDetail GetWorkerDetail(long? userId)
        {
            WorkerDetail userModel = new WorkerDetail();
            try
            {
                var user = _db.Users.Where(x => x.UserId == userId).FirstOrDefault();
                var userCats = user.UserSubCategories.Select(x => x.SubCategory.Category).Select(y => y.Id).ToList();
                var userSubCats = user.UserSubCategories.Select(x => x.SubCategoryId ?? 0).ToList();
                var userSubSubCats = user.UserSubCategories.Select(x => x.SubSubCategoryId ?? 0).ToList();
                if (user != null)
                {
                    userModel.User = new UserViewModel()
                    {
                        UserId = user.UserId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        Address = user.Address,
                        UserType = user.UserType,
                        PicturePath = user.PicturePath,
                    };

                }
            }
            catch (Exception ex)
            {
                userModel = new WorkerDetail();
            }
            return userModel;
        }
        public bool UpdatePropertyStatus(long id)
        {
            bool status = false;
            try
            {
                var uData = _db.Properties.Where(x => x.Id == id).FirstOrDefault();
                if (uData != null)
                {
                    if (uData.IsActive == true)
                    {
                        uData.IsActive = false;
                    }
                    else
                    {
                        uData.IsActive = true;
                    }
                    _db.SaveChanges();
                    message = uData.Name + Resource.status_update_sucess;
                }
                else
                {
                    message = Resource.property_not_exists;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        public bool ForgotPassword(string email)
        {
            bool status = false;
            try
            {
                var result = _db.Users.Where(x => x.Email == email).FirstOrDefault();
                if (result != null)
                {
                    string newPass = Common.FetchRandString(6);
                    bool passCheck = Common.SendEmailResetPassword(result.FirstName + " " + result.LastName,
                        result.Email, newPass);
                    if (passCheck == true)
                    {
                        string salt = string.Empty;
                        string encryptedPswd = string.Empty;
                        Common.GeneratePassword(newPass, "new", ref salt, ref encryptedPswd);

                        result.PasswordSalt = salt;
                        result.Password = encryptedPswd;
                        _db.SaveChanges();
                        message = Resource.new_password_sent_success;
                        status = true;
                    }
                    else
                    {
                        message = Resource.try_again;
                    }
                }
                else
                {
                    message = Resource.error_requesting_password;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        public bool TestPushNotification(TestPushViewModel model)
        {
            bool status = false;
            try
            {
                var userData = _db.Users.Where(x => x.UserId == model.UserId).FirstOrDefault();
                if (userData != null)
                {
                    string result = Common.PushNotification(userData.UserType, userData.DeviceId, userData.DeviceToken, model.Message);
                    message = result;
                    if (!string.IsNullOrEmpty(result))
                    {
                        return true;
                    }

                }
                else
                {
                    message = Resource.error_requesting_password;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        public bool ChangePassword(long userId, ChangePasswordModel model)
        {
            bool status = false;
            try
            {
                var result = _db.Users.Where(x => x.UserId == userId && x.IsActive == true).FirstOrDefault();
                if (result != null)
                {
                    if (Common.EncryptString(model.oldPassword, result.PasswordSalt) == result.Password)
                    {
                        string salt = string.Empty;
                        string encryptedPswd = string.Empty;
                        Common.GeneratePassword(model.newPassword, "new", ref salt, ref encryptedPswd);

                        result.Password = encryptedPswd;
                        result.PasswordSalt = salt;

                        _db.SaveChanges();
                        message = Resource.password_changed_success;
                        status = true;
                    }
                    else
                    {
                        message = Resource.please_enter_valid_old_password;
                    }
                }

                else
                {
                    message = Resource.user_not_exist;
                }
            }
            catch (Exception ex)
            {

            }

            return status;
        }

        public bool CheckVerifyEmail(string userId, out string userName)
        {
            userName = string.Empty;
            bool status = false;
            try
            {
                var user = _db.Users.Where(a => a.UserId.ToString() == userId
                && a.EmailVerified != true).FirstOrDefault();
                if (user != null)
                {
                    userName = user.FirstName + " " + user.LastName;
                    user.EmailVerified = true;
                    _db.SaveChanges();
                    status = true;
                }
            }
            catch (Exception ex)
            {

            }

            return status;
        }
        public bool UpdateUserStatus(long userId, out int? usertype)
        {
            usertype = (int)Enums.UserTypeEnum.Customer;
            bool status = false;
            try
            {
                var uData = _db.Users.Where(x => x.UserId == userId).FirstOrDefault();
                if (uData != null)
                {
                    usertype = uData.UserType;
                    if (uData.IsActive == true)
                    {
                        uData.IsActive = false;
                    }
                    else
                    {
                        uData.IsActive = true;
                    }
                    _db.SaveChanges();
                    message = uData.FirstName + " " + uData.LastName + Resource.status_update_sucess;
                }
                else
                {
                    message = Resource.sub_category_not_exists;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        public bool AddUpdateServiceProvider(UserViewModel model, string subCatIds, string subsubCatIds)
        {
            bool status = false;
            try
            {
                if (model.UserId != 0)
                {
                    var userData = _db.Users.Where(x => x.UserId == model.UserId).FirstOrDefault();
                    if (userData != null)
                    {

                        var validEmail = _db.Users.Where(x => x.Email.ToLower().Trim() ==
                        model.Email.ToLower().Trim() && x.UserId != model.UserId).FirstOrDefault();
                        if (validEmail != null)
                        {
                            message = Resource.email_already_exists;
                        }
                        else
                        {
                            userData.FirstName = model.FirstName;
                            userData.LastName = model.LastName;
                            userData.Address = model.Address;
                            userData.Email = model.Email;
                            userData.UserType = model.UserType;
                            userData.PhoneNumber = model.PhoneNumber;
                            string salt = Common.FetchRandString(6);
                            string encryptedPswd = string.Empty;
                            Common.GeneratePassword(model.Password, "new", ref salt, ref encryptedPswd);

                            userData.Password = encryptedPswd;
                            userData.PasswordSalt = salt;

                            userData.ModifiedDate = DateTime.Now;
                            _db.SaveChanges();


                            var userServices = _db.UserSubCategories.Where(x => x.UserId == model.UserId).ToList();
                            foreach (var item in userServices)
                            {
                                _db.Entry(item).State = EntityState.Deleted;
                                _db.SaveChanges();
                            }
                            var subCatIdsArr = subCatIds.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            var subsubCatIdsArr = subsubCatIds.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < subCatIdsArr.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(subCatIdsArr[i]))
                                {
                                    var subCategoryId = Convert.ToInt64(subCatIdsArr[i].Trim());
                                    var subCatData = _db.SubCategories.Where(x => x.Id == subCategoryId).FirstOrDefault();
                                    if (subCatData != null)
                                    {
                                        if (subCatData.HasSubSubCategories == true)
                                        {
                                            if (subCatData.SubSubCategories.Count > 0)
                                            {
                                                foreach (var item in subCatData.SubSubCategories)
                                                {
                                                    if (subsubCatIdsArr.Contains(item.Id.ToString()))
                                                    {
                                                        UserSubCategory userSubCategory = new UserSubCategory();
                                                        userSubCategory.UserId = userData.UserId;
                                                        userSubCategory.SubCategoryId = subCategoryId;
                                                        userSubCategory.SubSubCategoryId = item.Id;
                                                        var serData = _db.UserSubCategories.Add(userSubCategory);
                                                    }
                                                    else
                                                    {
                                                        UserSubCategory userSubCategory = new UserSubCategory();
                                                        userSubCategory.UserId = userData.UserId;
                                                        userSubCategory.SubCategoryId = subCategoryId;
                                                        var serData = _db.UserSubCategories.Add(userSubCategory);
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                UserSubCategory userSubCategory = new UserSubCategory();
                                                userSubCategory.UserId = userData.UserId;
                                                userSubCategory.SubCategoryId = subCategoryId;
                                                var serData = _db.UserSubCategories.Add(userSubCategory);
                                            }
                                        }
                                        else
                                        {
                                            UserSubCategory userSubCategory = new UserSubCategory();
                                            userSubCategory.UserId = userData.UserId;
                                            userSubCategory.SubCategoryId = subCategoryId;
                                            var serData = _db.UserSubCategories.Add(userSubCategory);
                                        }
                                    }
                                }
                            }
                            _db.SaveChanges();
                            status = true;
                            message = Resource.user_detail_update_success;
                            string usertype = model.UserType == (int)Enums.UserTypeEnum.ServiceProvider ?
                           Enums.UserTypeEnum.ServiceProvider.ToString() :
                           Enums.UserTypeEnum.Worker.ToString();
                            Common.SendEmailUserCredentials(usertype, model.FirstName + " " + model.LastName, model.Email, model.Password);
                        }
                    }
                    else
                    {
                        message = Resource.user_not_exist;
                    }
                }
                else
                {
                    var validEmail = _db.Users.Where(x => x.Email.ToLower().Trim() ==
                    model.Email.ToLower().Trim() && x.UserId != model.UserId).FirstOrDefault();
                    if (validEmail != null)
                    {
                        message = Resource.email_already_exists;
                    }
                    else
                    {
                        User user = new User();
                        user.CreatedDate = DateTime.Now;
                        user.IsActive = true;
                        user.EmailVerified = true;
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        user.Email = model.Email;
                        user.Address = model.Address;
                        user.PhoneNumber = model.PhoneNumber;
                        user.UserType = model.UserType;
                        string salt = Common.FetchRandString(6);
                        string encryptedPswd = string.Empty;
                        Common.GeneratePassword(model.Password, "new", ref salt, ref encryptedPswd);

                        user.Password = encryptedPswd;
                        user.PasswordSalt = salt;
                        _db.Users.Add(user);
                        _db.SaveChanges();

                        var subCatIdsArr = subCatIds.Split(',');
                        var subsubCatIdsArr = subsubCatIds.Split(',');
                        for (int i = 0; i < subCatIdsArr.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(subCatIdsArr[i]))
                            {
                                var subCategoryId = Convert.ToInt64(subCatIdsArr[i]);
                                var subCatData = _db.SubCategories.Where(x => x.Id == subCategoryId).FirstOrDefault();
                                if (subCatData != null)
                                {
                                    if (subCatData.HasSubSubCategories == true)
                                    {
                                        foreach (var item in subCatData.SubSubCategories)
                                        {
                                            if (subsubCatIdsArr.Contains(item.Id.ToString()))
                                            {
                                                UserSubCategory userSubCategory = new UserSubCategory();
                                                userSubCategory.UserId = user.UserId;
                                                userSubCategory.SubCategoryId = subCategoryId;
                                                userSubCategory.SubSubCategoryId = item.Id;
                                                var serData = _db.UserSubCategories.Add(userSubCategory);
                                            }
                                            else
                                            {
                                                UserSubCategory userSubCategory = new UserSubCategory();
                                                userSubCategory.UserId = user.UserId;
                                                userSubCategory.SubCategoryId = subCategoryId;
                                                var serData = _db.UserSubCategories.Add(userSubCategory);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        UserSubCategory userSubCategory = new UserSubCategory();
                                        userSubCategory.UserId = user.UserId;
                                        userSubCategory.SubCategoryId = subCategoryId;
                                        var serData = _db.UserSubCategories.Add(userSubCategory);

                                    }
                                }
                            }
                        }
                        _db.SaveChanges();
                        status = true;
                        message = Resource.user_detail_add_success;
                        string usertype = model.UserType == (int)Enums.UserTypeEnum.ServiceProvider ?
                            Enums.UserTypeEnum.ServiceProvider.ToString() :
                            Enums.UserTypeEnum.Worker.ToString();
                        Common.SendEmailUserCredentials(usertype, model.FirstName + " " + model.LastName, model.Email, model.Password);
                    }

                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        #region App Methods


        /// <summary>
        /// Login Method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserModel GetLoginUserApp(UserModel model)
        {
            string _path = "https://" + System.Web.HttpContext.Current.Request.Url.Authority + "/Images/User/";

            UserModel obj = null;

            try
            {
                var usertypeA = Enums.UserTypeEnum.Admin.GetHashCode();
                var user = _db.Users.Where(i => i.Email == model.Email && !(i.UserType == usertypeA)).FirstOrDefault();

                if (user != null)
                {
                    user.DeviceId = model.DeviceId;
                    user.DeviceToken = model.DeviceToken;
                    _db.SaveChanges();

                    if (Common.EncryptString(model.Password, user.PasswordSalt) == user.Password)
                    {
                        if (user.IsActive == false)
                        {
                            obj = null;
                            message = user.FirstName + " " + user.LastName + " , "+Resource.account_deactivate_message;
                            return obj;
                        }
                        if (user.EmailVerified == false)
                        {
                            obj = null;
                            message = user.FirstName + " " + user.LastName + " , "+Resource.verify_email_before_signing;
                            return obj;
                        }
                        obj = new UserModel()
                        {
                            UserId = user.UserId,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            DeviceId = user.DeviceId,
                            DeviceToken = user.DeviceToken,
                            Email = user.Email,
                            Address = user.Address,
                            IsActive = user.IsActive,
                            PhoneNumber = user.PhoneNumber,
                            UserType = user.UserType,
                            PicturePath = user.PicturePath,
                            City = user.City,
                            Zipcode = user.Zipcode,
                            CountryId = user.CountryId,
                        };

                        message = Resource.user_logged_in_successfully;
                    }
                    else
                    {
                        obj = null;
                        message = Resource.please_enter_valid_password;
                    }
                }
                else
                {
                    obj = null;
                    message = Resource.please_enter_valid_email;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                obj = null;
            }
            return obj;
        }
        /// <summary>
        /// Logout Method.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool Logout(long userId)
        {
            bool status = false;
            try
            {
                var user = _db.Users.Where(i => i.UserId == userId).FirstOrDefault();
                if (user != null)
                {
                    user.DeviceToken = "";
                    _db.SaveChanges();
                    status = true;
                    message = Resource.success;
                }
                else
                {
                    message = Resource.user_not_exist;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }

        /// <summary>
        /// Sign Up Method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserModel SignUp(UserModel model)
        {
            UserModel obj = null;
            string _path = "https://" + System.Web.HttpContext.Current.Request.Url.Authority + "/Images/User/";
            try
            {
                // check if email already exist
                var getExistUserEmail = _db.Users.Where(x => x.Email == model.Email).FirstOrDefault();
                if (getExistUserEmail != null)
                {
                    obj = null;
                    message = Resource.email_already_exists;
                    return obj;
                }

                if (getExistUserEmail == null)
                {
                    User user = new User();
                    user.DeviceId = model.DeviceId;
                    user.DeviceToken = model.DeviceToken;
                    user.Email = model.Email;
                    user.IsActive = true;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.PicturePath = model.PicturePath;
                    user.CreatedDate = DateTime.Now;
                    user.UserType = (int)Enums.UserTypeEnum.Customer;
                    user.PaymentMethod = (int)Enums.PaymentMethod.ByCreditCard;
                    user.CountryId = model.CountryId;
                    user.EmailVerified = false;
                    user.City = model.City;
                    user.Zipcode = model.Zipcode;
                    user.Address = model.Address;
                    user.CountryCode = model.CountryCode;
                    user.PhoneNumber = model.PhoneNumber;

                    // Generating encrypted password
                    string salt = string.Empty;
                    string encryptedPswd = string.Empty;
                    Common.GeneratePassword(model.Password, "new", ref salt, ref encryptedPswd);

                    user.Password = encryptedPswd;
                    user.PasswordSalt = salt;
                    _db.Users.Add(user);
                    _db.SaveChanges();

                    obj = new UserModel();
                    obj.DeviceId = user.DeviceId;
                    obj.DeviceToken = user.DeviceToken;
                    obj.FirstName = user.FirstName;
                    obj.LastName = user.LastName;
                    obj.UserId = user.UserId;
                    obj.Email = user.Email;
                    obj.Address = user.Address;
                    obj.City = user.City;
                    obj.Zipcode = user.Zipcode;
                    obj.CountryCode = user.CountryCode;
                    obj.CountryId = user.CountryId;
                    obj.IsActive = user.IsActive;
                    obj.PhoneNumber = user.PhoneNumber;
                    obj.PicturePath = user.PicturePath;
                    obj.UserId = user.UserId;
                    obj.UserType = user.UserType;
                    message = Resource.confirmation_email_sent; //"You have successfully registered.";
                    Common.SendSignupConfirmationEmail(user.FirstName + " " + user.LastName, user.Email, user.UserId.ToString());
                }
            }
            catch (Exception ex)
            {

                message = ex.Message;
                obj = null;
            }

            return obj;
        }
        public bool UpdateDeviceInfo(long userId, int deviceId, string deviceToken)
        {
            bool status = false;
            try
            {
                var user = _db.Users.Where(i => i.UserId == userId).FirstOrDefault();
                if (user != null)
                {
                    user.DeviceId = deviceId;
                    user.DeviceToken = deviceToken;
                    _db.SaveChanges();
                    status = true;
                    message = Resource.success;
                }
                else
                {
                    message = Resource.user_not_exist;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        public bool EditProfile(UserModel userModel)
        {
            bool status = false;
            try
            {
                // check if name already exist 

                var uData = _db.Users.Where(a => a.UserId == userModel.UserId).FirstOrDefault();
                if (uData != null)
                {
                    var validEmail = _db.Users.Where(x => x.Email.ToLower().Trim() ==
                          userModel.Email.ToLower().Trim() && x.UserId != userModel.UserId).FirstOrDefault();
                    if (validEmail != null)
                    {
                        message = Resource.email_already_exists;
                    }
                    else
                    {
                        uData.FirstName = userModel.FirstName;
                        uData.LastName = userModel.LastName;
                        uData.Address = userModel.Address;
                        uData.City = userModel.City;
                        uData.CountryCode = userModel.CountryCode;
                        uData.CountryId = userModel.CountryId;
                        uData.PicturePath = !string.IsNullOrEmpty(userModel.PicturePath) ?
                            userModel.PicturePath : uData.PicturePath;
                        uData.ModifiedDate = DateTime.Now;
                        uData.DeviceId = userModel.DeviceId;
                        uData.DeviceToken = userModel.DeviceToken;
                        uData.Email = userModel.Email;
                        uData.PhoneNumber = userModel.PhoneNumber;
                        uData.Zipcode = userModel.Zipcode;
                        _db.SaveChanges();
                        status = true;
                        message = Resource.profile_update_success;
                    }
                }
                else
                {
                    message = Resource.user_not_exist;
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        public AboutU GetAboutus()
        {
            AboutU data = null;
            try
            {
                data = _db.AboutUs.FirstOrDefault();
            }
            catch (Exception ex)
            {
            }
            return data;
        }
        public TermsCondition GetTermsConditions()
        {
            TermsCondition data = null;
            try
            {
                data = _db.TermsConditions.FirstOrDefault();
            }
            catch (Exception ex)
            {
            }
            return data;
        }
        public PrivacyPolicy GetPrivacyPolicy()
        {
            PrivacyPolicy data = null;
            try
            {
                data = _db.PrivacyPolicies.FirstOrDefault();
            }
            catch (Exception ex)
            {
            }
            return data;
        }
        public bool ContactUs(ContactUsViewModel model)
        {
            bool status = false;
            try
            {
                var result = _db.Users.Where(x => x.UserId == model.UserId && x.IsActive == true).FirstOrDefault();
                if (result != null)
                {
                    ContactU contact = new ContactU()
                    {
                        Email = model.Email,
                        Message = model.Message,
                        UserId = model.UserId,
                        Name = model.Name
                    };
                    _db.ContactUs.Add(contact);
                    _db.SaveChanges();
                    message = Resource.info_update_success;
                    status = true;
                }
                else
                {
                    message = Resource.user_not_exist;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }

        public UserModel GetProfile(long userId)
        {
            UserModel userModel = new UserModel();
            try
            {
                var user = _db.Users.Where(x => x.UserId == userId).FirstOrDefault();
                if (user != null)
                {
                    userModel = new UserModel()
                    {
                        UserId = user.UserId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        Address = user.Address,
                        UserType = user.UserType,
                        City = user.City,
                        CountryCode = user.CountryCode,
                        CountryId = user.CountryId,
                        DeviceId = user.DeviceId,
                        DeviceToken = user.DeviceToken,
                        PicturePath = user.PicturePath,
                        Zipcode = user.Zipcode,

                    };
                    message = Resource.success;
                }
                else
                {
                    message = Resource.user_not_exist;
                }
            }
            catch (Exception ex)
            {
                userModel = new UserModel();
            }
            return userModel;
        }
        #endregion

        #region New Methods

        public DashboardViewModel GetDashboardData()
        {
            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            var getCustomerType = Enums.UserTypeEnum.Customer.GetHashCode();
            var getProviderType = Enums.UserTypeEnum.ServiceProvider.GetHashCode();
            var getWorkerType = Enums.UserTypeEnum.Worker.GetHashCode();
            dashboardViewModel.CustomerNo = _db.Users.Where(a => a.UserType == getCustomerType).Count().ToString();
            dashboardViewModel.ProviderNo = _db.Users.Where(a => a.UserType == getProviderType).Count().ToString();
            dashboardViewModel.WorkerNo = _db.Users.Where(a => a.UserType == getWorkerType).Count().ToString();
            return dashboardViewModel;
        }
        public SettingModel GetSettingData()
        {
            var result = _db.Settings.FirstOrDefault();
            SettingModel model = new SettingModel();
            if (result != null)
            {
                model.AdminPricePer = result.AdminPricePer ?? 0;
            }
            return model;
        }
        public bool AddUpdateSettingData(SettingModel model)
        {
            bool status = false;
            try
            {
                var result = _db.Settings.FirstOrDefault();
                if (result == null)
                {
                    Setting obj = new Setting();
                    obj.AdminPricePer = model.AdminPricePer;
                    _db.Settings.Add(obj);
                    _db.SaveChanges();
                    status = true;
                }
                else
                {
                    result.AdminPricePer = model.AdminPricePer;
                    _db.SaveChanges();
                    status = true;
                }
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }
        public AboutUsViewModel GetAboutUsData()
        {
            var result = _db.AboutUs.FirstOrDefault();
            AboutUsViewModel aboutUsModel = new AboutUsViewModel();
            if (result != null)
            {
                aboutUsModel.AboutUsId = result.AboutUsId;
                aboutUsModel.Text = result.Text;
            }
            return aboutUsModel;
        }

        public bool AddUpdateAboutUsData(AboutUsViewModel aboutUsModel)
        {
            bool status = false;
            try
            {
                var result = _db.AboutUs.FirstOrDefault();
                if (result == null)
                {
                    AboutU obj = new AboutU();
                    obj.Text = aboutUsModel.Text;
                    _db.AboutUs.Add(obj);
                    _db.SaveChanges();
                    status = true;
                }
                else
                {
                    result.Text = aboutUsModel.Text;
                    _db.SaveChanges();
                    status = true;
                }
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }


        public bool AddUpdateTermsConditionsData(TermsCondition model)
        {
            bool status = false;
            try
            {
                var result = _db.TermsConditions.FirstOrDefault();
                if (result == null)
                {
                    TermsCondition obj = new TermsCondition();
                    obj.TermsConditionText = model.TermsConditionText;
                    _db.TermsConditions.Add(obj);
                    _db.SaveChanges();
                    status = true;
                }
                else
                {
                    result.TermsConditionText = model.TermsConditionText;
                    _db.SaveChanges();
                    status = true;
                }
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }
        public bool AddUpdatePrivacyPolicyData(PrivacyPolicy model)
        {
            bool status = false;
            try
            {
                var result = _db.PrivacyPolicies.FirstOrDefault();
                if (result == null)
                {
                    PrivacyPolicy obj = new PrivacyPolicy();
                    obj.PrivacyPolicyText = model.PrivacyPolicyText;
                    _db.PrivacyPolicies.Add(obj);
                    _db.SaveChanges();
                    status = true;
                }
                else
                {
                    result.PrivacyPolicyText = model.PrivacyPolicyText;
                    _db.SaveChanges();
                    status = true;
                }
            }
            catch (Exception)
            {
                status = false;
            }
            return status;
        }
        public List<Testimonial> GetTestimonials(bool isActive = false)
        {
            List<Testimonial> testimonials = new List<Testimonial>();
            try
            {
                testimonials = _db.Testimonials.ToList();
                if (isActive)
                {
                    testimonials = testimonials.Where(x => x.IsActive == true).ToList();
                }
            }
            catch (Exception ex)
            {
            }
            return testimonials;
        }

        public Testimonial GetTestimonialById(long id)
        {
            Testimonial testimonial = new Testimonial();
            try
            {
                testimonial = _db.Testimonials.Where(x => x.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }
            return testimonial;
        }

        public bool AddUpdateTestimonial(Testimonial model)
        {
            bool status = false;
            try
            {
                var testimonial = _db.Testimonials.Where(x => x.Id == model.Id).FirstOrDefault();
                if (testimonial != null)
                {
                    testimonial.Image = model.Image;
                    testimonial.Title = model.Title;
                    testimonial.Name = model.Name;
                    testimonial.Description = model.Description;
                    _db.SaveChanges();
                    status = true;
                    message = Resource.testimonial_update_success;
                }
                else
                {
                    model.IsActive = true;
                    model.CreatedDate = DateTime.Now;
                    _db.Testimonials.Add(model);
                    _db.SaveChanges();

                    status = true;
                    message = Resource.testimonial_add_success;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }

        public bool UpdateTestimonialStatus(long id)
        {
            bool status = false;
            try
            {
                var testimonialData = _db.Testimonials.Where(x => x.Id == id).FirstOrDefault();
                if (testimonialData != null)
                {
                    if (testimonialData.IsActive == true)
                    {
                        testimonialData.IsActive = false;
                    }
                    else
                    {
                        testimonialData.IsActive = true;
                    }
                    _db.SaveChanges();
                    message = testimonialData.Name + Resource.status_update_sucess;
                    status = true;
                }
                else
                {
                    message = Resource.testimonial_not_exist;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        public bool DeleteTestimonial(long id)
        {
            bool status = false;
            try
            {
                var testimonialData = _db.Testimonials.Where(x => x.Id == id).FirstOrDefault();
                if (testimonialData != null)
                {
                    _db.Entry(testimonialData).State = EntityState.Deleted;
                    _db.SaveChanges();
                    message = Resource.testimonial_delete_success;
                    status = true;
                }
                else
                {
                    message = Resource.testimonial_not_exist;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return status;
        }
        #endregion

        #region Web Methods
        public UserModel GetLoginUserWeb(UserModel model)
        {
            string _path = "https://" + System.Web.HttpContext.Current.Request.Url.Authority + "/Images/User/";

            UserModel obj = null;

            try
            {
                var usertypeA = Enums.UserTypeEnum.Admin.GetHashCode();
                var user = _db.Users.Where(i => i.Email == model.Email && !(i.UserType == usertypeA)).FirstOrDefault();

                if (user != null)
                {
                    user.DeviceId = model.DeviceId;
                    user.DeviceToken = model.DeviceToken;
                    _db.SaveChanges();

                    if (Common.EncryptString(model.Password, user.PasswordSalt) == user.Password)
                    {
                        if (user.IsActive == false)
                        {
                            obj = null;
                            message = user.FirstName + " " + user.LastName + " , "+Resource.account_deactivate_message;
                            return obj;
                        }
                        if (user.EmailVerified == false)
                        {
                            obj = null;
                            message = user.FirstName + " " + user.LastName + " , "+Resource.verify_email_before_signing;
                            return obj;
                        }
                        obj = new UserModel()
                        {
                            UserId = user.UserId,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            DeviceId = user.DeviceId,
                            DeviceToken = user.DeviceToken,
                            Email = user.Email,
                            Address = user.Address,
                            IsActive = user.IsActive,
                            PhoneNumber = user.PhoneNumber,
                            UserType = user.UserType,
                            PicturePath = user.PicturePath,
                            City = user.City,
                            Zipcode = user.Zipcode,
                            CountryId = user.CountryId,
                            Image = user.PicturePath
                        };

                        message = Resource.user_logged_in_successfully;
                    }
                    else
                    {
                        obj = null;
                        message = Resource.please_enter_valid_password;
                    }
                }
                else
                {
                    obj = null;
                    message = Resource.please_enter_valid_email;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                obj = null;
            }
            return obj;
        }
        public List<SelectListItem> GetCountryCodeSelect()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems = _db.Countries.Where(x => x.IsActive == true)
                .Select(x => new SelectListItem
                {
                    Text = x.CountryCode,
                    Value = x.CountryId.ToString(),
                }).ToList();
            return listItems;
        }
        public List<SelectListItem> GetCountriesSelect()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems = _db.Countries.Where(x => x.IsActive == true)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.CountryId.ToString(),
                }).ToList();
            return listItems;
        }



        #endregion
    }
}
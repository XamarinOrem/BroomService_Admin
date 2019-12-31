using BroomServiceWeb.Helpers;
using BroomServiceWeb.Models;
using BroomServiceWeb.Resources;
using BroomServiceWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BroomServiceWeb.Controllers.Web
{
    public class PropertyController : MyController
    {
        PropertyService propertyService;

        public PropertyController()
        {
            propertyService = new PropertyService();
        }

        #region Add/Update Property

        [WebAuthAttribute]
        // GET: Property
        public ActionResult AddProperty(int id)
        {
            var userId = Convert.ToInt64(Session["UserId"]);
            if (userId != 0)
            {
                PropertyModel getPropertyData = null;
                if (id != 0)
                {
                    ViewBag.TitleText = Resource.edit_property;
                    getPropertyData = propertyService.GetPropertiesById(id);
                }
                else
                {
                    ViewBag.TitleText = Resource.add_property;
                }
                return View(getPropertyData);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        /// <summary>
        /// Add Update Property
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [WebAuthAttribute]
        public ActionResult AddProperty(PropertyModel model)
        {
            if (ModelState.IsValid)
            {
                var getLatLng = Common.GetLatLongFromAddress(model.Address);
                model.Latitude = Convert.ToDecimal(getLatLng.lat);
                model.Longitude = Convert.ToDecimal(getLatLng.lng);
                model.CreatedBy = Convert.ToInt64(Session["UserId"]);
                model.NoOfBathrooms = model.NoOfBathrooms == null ? 0 : model.NoOfBathrooms;
                model.NoOfQueenBeds = model.NoOfQueenBeds == null ? 0 : model.NoOfQueenBeds;
                model.NoOfDoubleBeds = model.NoOfDoubleBeds == null ? 0 : model.NoOfDoubleBeds;
                model.NoOfSingleBeds = model.NoOfSingleBeds == null ? 0 : model.NoOfSingleBeds;
                model.NoOfSingleSofaBeds = model.NoOfSingleSofaBeds == null ? 0 : model.NoOfSingleSofaBeds;
                model.NoOfDoubleSofaBeds = model.NoOfDoubleSofaBeds == null ? 0 : model.NoOfDoubleSofaBeds;
                model.NoOfDuvet = model.NoOfDuvet == null ? 0 : model.NoOfDuvet;
                model.NoOfPillows = model.NoOfPillows == null ? 0 : model.NoOfPillows;
                model.NoOfBedRooms = model.NoOfBedRooms == null ? 0 : model.NoOfBedRooms;
                model.NoOfToilets = model.NoOfToilets == null ? 0 : model.NoOfToilets;

                var status = propertyService.AddUpdateProperty(model);
                if (model.Id != null && model.Id != 0)
                {
                    ViewBag.TitleText = Resource.edit_property;
                }
                else
                {
                    ViewBag.TitleText = Resource.add_property;
                }
                if (status)
                {
                    ModelState.Clear();
                    TempData["SuccessMsg"] = propertyService.message;
                    return RedirectToAction("MyProperties", "Property");
                }
                else
                {
                    TempData["ErrorMsg"] = Resource.some_error_occured;
                }
            }
            return View();
        }

        #endregion

        #region My Properties

        [WebAuthAttribute]
        public ActionResult MyProperties()
        {
            var getUserId = Convert.ToInt64(Session["UserId"]);
            if (getUserId != 0)
            {
                var result = propertyService.GetPropertiesByUserId(getUserId);
                result = result.OrderByDescending(a => a.CreatedDate).ToList();
                return View(result);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        #endregion

        #region Property Details
        //[WebAuthAttribute]
        public ActionResult PropertyDetails(int id)
        {
            //var userId = Convert.ToInt64(Session["UserId"]);
            //if (userId != 0)
            //{
                var result = propertyService.GetPropertiesById(id);
                return PartialView("PropertyDetails", result);
            //}
            //else
            //{
              //  return RedirectToAction("Login", "Account");
            //}
        }
        #endregion
    }
}
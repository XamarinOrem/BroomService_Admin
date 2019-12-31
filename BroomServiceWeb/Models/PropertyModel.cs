using BroomServiceWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BroomServiceWeb.Models
{
    public class PropertyModel
    {
        public PropertyModel()
        { 
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool? ShortTermApartment { get; set; }
        public int? FloorNumber { get; set; }
        public int? ApartmentNumber { get; set; }
        public string BuildingCode { get; set; }
        public string AccessToProperty { get; set; }
        public int? NoOfBathrooms { get; set; }
        public int? NoOfQueenBeds { get; set; }
        public int? NoOfDoubleBeds { get; set; }
        public int? NoOfSingleBeds { get; set; }
        public int? NoOfSingleSofaBeds { get; set; }
        public int? NoOfDoubleSofaBeds { get; set; }
        public string DuvetSize { get; set; }
        public int? NoOfDuvet { get; set; }
        public int? NoOfPillows { get; set; }
        public bool? Doorman { get; set; }
        public bool? Parking { get; set; }
        public bool? Balcony { get; set; }
        public bool? Dishwasher { get; set; }
        public bool? Pool { get; set; }
        public bool? Garden { get; set; }
        public bool? Elevator { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool? IsActive { get; set; }
        public int? NoOfToilets { get; set; }
        public int? NoOfBedRooms { get; set; }
        public string Size { get; set; }
    } 
}
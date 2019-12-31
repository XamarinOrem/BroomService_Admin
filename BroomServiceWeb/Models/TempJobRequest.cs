//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BroomServiceWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TempJobRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TempJobRequest()
        {
            this.TempJobRequestCheckLists = new HashSet<TempJobRequestCheckList>();
            this.TempJobRequestRefImages = new HashSet<TempJobRequestRefImage>();
        }
    
        public long Id { get; set; }
        public Nullable<long> PropertyId { get; set; }
        public Nullable<long> CategoryId { get; set; }
        public Nullable<long> SubCategoryId { get; set; }
        public Nullable<System.DateTime> JobStartDatetime { get; set; }
        public Nullable<System.DateTime> JobEndDatetime { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<long> ServiceProviderId { get; set; }
        public Nullable<int> JobStatus { get; set; }
        public Nullable<decimal> QuotePrice { get; set; }
        public Nullable<bool> IsQuoteApproved { get; set; }
        public Nullable<System.TimeSpan> TimerStartTime { get; set; }
        public Nullable<System.TimeSpan> TimerEndTime { get; set; }
        public Nullable<int> PaymentMethod { get; set; }
        public Nullable<bool> IsPaymentDone { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual Property Property { get; set; }
        public virtual Property Property1 { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public virtual SubCategory SubCategory1 { get; set; }
        public virtual SubCategory SubCategory2 { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TempJobRequestCheckList> TempJobRequestCheckLists { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TempJobRequestRefImage> TempJobRequestRefImages { get; set; }
    }
}

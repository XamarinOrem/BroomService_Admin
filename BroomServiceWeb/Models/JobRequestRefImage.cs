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
    
    public partial class JobRequestRefImage
    {
        public long Id { get; set; }
        public string PicturePath { get; set; }
        public Nullable<long> JobRequestId { get; set; }
    
        public virtual JobRequest JobRequest { get; set; }
    }
}

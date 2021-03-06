//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace US_PrimarcyApp.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Medicine
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Medicine()
        {
            this.Medicine_To_Tags = new HashSet<Medicine_To_Tags>();
            this.Orders = new HashSet<Orders>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<short> Amount { get; set; }
        public Nullable<bool> isResipt { get; set; }
        public Nullable<System.DateTime> Prodate { get; set; }
        public Nullable<System.DateTime> Expdate { get; set; }
        public Nullable<int> Barcode { get; set; }
        public string Description { get; set; }
        public Nullable<int> FirmsId { get; set; }
    
        public virtual Firms Firms { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Medicine_To_Tags> Medicine_To_Tags { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Orders> Orders { get; set; }
    }
}

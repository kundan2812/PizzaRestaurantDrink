//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace dblayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class StockMenuCategoryTable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StockMenuCategoryTable()
        {
            this.StockMenuitemTables = new HashSet<StockMenuitemTable>();
        }
    
        public int StockMenuCategoryID { get; set; }
        public string StockMenuCategory { get; set; }
        public int CreatedBy_UserID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockMenuitemTable> StockMenuitemTables { get; set; }
    }
}

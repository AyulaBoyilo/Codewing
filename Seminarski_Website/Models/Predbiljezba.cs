//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Seminarski_Website.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Predbiljezba
    {
        public int IdPredbiljezbe { get; set; }
        public int IdSeminar { get; set; }
        public int IdKorisnik { get; set; }
        public System.DateTime Datum { get; set; }
        public Nullable<bool> Stanje { get; set; }
    
        public virtual Korisnik Korisnik { get; set; }
        public virtual Seminar Seminar { get; set; }
    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RemedioApi.EF
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RemedioEntities : DbContext
    {
        public RemedioEntities()
            : base("name=RemedioEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tb_FcmToken> tb_FcmToken { get; set; }
        public virtual DbSet<tb_Leitura> tb_Leitura { get; set; }
        public virtual DbSet<tb_CaixaRemedio> tb_CaixaRemedio { get; set; }
        public virtual DbSet<tb_Paciente> tb_Paciente { get; set; }
    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DrieUnityGarage.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DrieUnityGarageEntities : DbContext
    {
        public DrieUnityGarageEntities()
            : base("name=DrieUnityGarageEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CT_HOADON> CT_HOADON { get; set; }
        public virtual DbSet<HANGHOA> HANGHOAs { get; set; }
        public virtual DbSet<HOADON> HOADONs { get; set; }
        public virtual DbSet<KHACHHANG> KHACHHANGs { get; set; }
        public virtual DbSet<NHACUNGCAP> NHACUNGCAPs { get; set; }
        public virtual DbSet<NHANVIEN> NHANVIENs { get; set; }
        public virtual DbSet<PHUONGTIEN> PHUONGTIENs { get; set; }
        public virtual DbSet<THONGTINTHANHTOAN> THONGTINTHANHTOANs { get; set; }
        public virtual DbSet<THONGTINTIEPNHAN> THONGTINTIEPNHANs { get; set; }
    }
}

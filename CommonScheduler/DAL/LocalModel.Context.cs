﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CommonScheduler.DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class localDBEntities : DbContext
    {
        public localDBEntities()
            : base("name=localDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Classes_L> Classes_L { get; set; }
        public virtual DbSet<Deparment_L> Deparment_L { get; set; }
        public virtual DbSet<DepartmentTeacher_L> DepartmentTeacher_L { get; set; }
        public virtual DbSet<Dictionary_L> Dictionary_L { get; set; }
        public virtual DbSet<DictionaryValue_L> DictionaryValue_L { get; set; }
        public virtual DbSet<Group_L> Group_L { get; set; }
        public virtual DbSet<Holiday_L> Holiday_L { get; set; }
        public virtual DbSet<LocalUser_L> LocalUser_L { get; set; }
        public virtual DbSet<Location_L> Location_L { get; set; }
        public virtual DbSet<Major_L> Major_L { get; set; }
        public virtual DbSet<Role_L> Role_L { get; set; }
        public virtual DbSet<Room_L> Room_L { get; set; }
        public virtual DbSet<Semester_L> Semester_L { get; set; }
        public virtual DbSet<SpecialLocation_L> SpecialLocation_L { get; set; }
        public virtual DbSet<Subgroup_L> Subgroup_L { get; set; }
        public virtual DbSet<Teacher_L> Teacher_L { get; set; }
        public virtual DbSet<Week_L> Week_L { get; set; }
    }
}
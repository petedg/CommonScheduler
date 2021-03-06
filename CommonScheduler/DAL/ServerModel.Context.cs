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
    
    public partial class serverDBEntities : DbContext
    {
        public serverDBEntities()
            : base("name=serverDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Classes> Classes { get; set; }
        public virtual DbSet<ClassesGroup> ClassesGroup { get; set; }
        public virtual DbSet<ClassesWeek> ClassesWeek { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<DepartmentTeacher> DepartmentTeacher { get; set; }
        public virtual DbSet<Dictionary> Dictionary { get; set; }
        public virtual DbSet<DictionaryValue> DictionaryValue { get; set; }
        public virtual DbSet<ExternalTeacher> ExternalTeacher { get; set; }
        public virtual DbSet<GlobalUser> GlobalUser { get; set; }
        public virtual DbSet<Group> Group { get; set; }
        public virtual DbSet<Holiday> Holiday { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<Major> Major { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Room> Room { get; set; }
        public virtual DbSet<Semester> Semester { get; set; }
        public virtual DbSet<SpecialLocation> SpecialLocation { get; set; }
        public virtual DbSet<Subgroup> Subgroup { get; set; }
        public virtual DbSet<Teacher> Teacher { get; set; }
        public virtual DbSet<UserDepartment> UserDepartment { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Week> Week { get; set; }
        public virtual DbSet<SubjectDefinition> SubjectDefinition { get; set; }
        public virtual DbSet<CurrentSchedule> CurrentSchedule { get; set; }
    }
}

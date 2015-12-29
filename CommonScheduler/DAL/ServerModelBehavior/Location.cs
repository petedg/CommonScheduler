using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class Location
    {
        private serverDBEntities context;
        private Room roomBehavior;

        public Location(serverDBEntities context)
        {
            this.context = context;
            roomBehavior = new Room(context);
        }

        public dynamic GetLocationsWithDepartments()
        {
            var locationsJoinDepartments = from location in context.Location
                                           join department in context.Department on location.Department_ID equals department.ID
                                           select new { DEP_DATE_CREATED = department.DATE_CREATED, DEP_DATE_MODIFIED = department.DATE_MODIFIED, DEP_ID = department.ID, DEP_ID_CREATED = department.ID_CREATED, 
                                               DEP_ID_MODIFIED = department.ID_MODIFIED, DEP_NAME = department.NAME, DEP_WWW_HOME_PAGE = department.WWW_HOME_PAGE, LOC_CITY = location.CITY, LOC_DATE_CREATED = location.DATE_CREATED,
                                           LOC_DATEMODIFIED = location.DATE_MODIFIED, LOC_DEPARTMENT_ID = location.Department_ID, LOC_ID = location.ID, LOC_ID_CREATED = location.ID_CREATED, LOC_ID_MODIFIED = location.ID_MODIFIED, 
                                           LOC_NAME = location.NAME, LOC_POSTAL_CODE = location.POSTAL_CODE, LOC_STREET = location.STREET, LOC_STREET_NUMBER = location.STREET_NUMBER};

            return locationsJoinDepartments.ToArray();
        }

        public List<Location> GetLocationsForDepartment(Department department)
        {
            var locations = from location in context.Location
                              where location.Department_ID == department.ID
                              select location;

            return locations.ToList();
        }

        public Location AddLocation(Location location)
        {
            return context.Location.Add(location);
        }

        public Location UpdateLocation(Location location)
        {
            context.Location.Attach(location);
            context.Entry(location).State = EntityState.Modified;
            return location;
        }

        public Location DeleteLocation(Location location)
        {
            context.Entry(location).State = EntityState.Deleted;
            return location;
        }

        public void RemoveLocationsForDepartment(Department department)
        {
            var locations = from location in context.Location
                            where location.Department_ID == department.ID
                            select location;

            foreach (Location l in locations)
            {
                roomBehavior.RemoveRoomsForLocation(l);
                DeleteLocation(l);
            }
        }

                                           
    }
}

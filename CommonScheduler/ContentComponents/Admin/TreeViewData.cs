using CommonScheduler.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    class TreeViewData
    {
        private serverDBEntities context;
        private Major majorBehavior;
        private Subgroup subgroupBehavior;
        private Group groupBehavior;
        private Department departmentBehavior;
        private Location locationBehavior;
        private Room roomBehavior;

        public List<Major> MajorList { get; set; }
        public List<Department> DepartmentList { get; set; }

        public TreeViewData(serverDBEntities context, TreeViewType treeViewType)
        {
            this.context = context;

            majorBehavior = new Major(context);
            subgroupBehavior = new Subgroup(context);
            groupBehavior = new Group(context);
            departmentBehavior = new Department(context);
            locationBehavior = new Location(context);
            roomBehavior = new Room(context);

            if (treeViewType == TreeViewType.MAJOR_LIST)
            {
                initializeMajorList();
            }
            else if (treeViewType == TreeViewType.GROUP_LIST)
            {
                initializeGroupList();
            }
            else if (treeViewType == TreeViewType.ROOM_LIST)
            {
                initializeRoomList();
            }   
        }

        private void initializeMajorList()
        {
            MajorList = majorBehavior.GetMajorsForDepartment(CurrentUser.Instance.AdminCurrentDepartment);
            if (MajorList.Count == 0)
                MajorList = new List<Major>();

            foreach (Major m in MajorList)
            {
                m.SubgroupsList = subgroupBehavior.GetSubgroupsForMajor(m);
                if (m.SubgroupsList.Count == 0)
                    m.SubgroupsList = new List<object>();

                foreach (Subgroup s in m.SubgroupsList)
                {
                    s.NestedSubgroupsList = subgroupBehavior.GetSubgroupsForParentSubgroup(s);
                    if (s.NestedSubgroupsList.Count == 0)
                        s.NestedSubgroupsList = new List<object>();
                }
            }
        }

        private void initializeGroupList()
        {
            MajorList = majorBehavior.GetMajorsForDepartment(CurrentUser.Instance.AdminCurrentDepartment);

            foreach (Major m in MajorList)
            {
                m.CompositeSubgroupsList = new List<CompositeCollectionSubgroupsAndGroups>();
                
                foreach(Subgroup sub in subgroupBehavior.GetSubgroupsForMajor(m).Cast<Subgroup>().ToList())
                {
                    List<Group> groupsForParentSubgroup = groupBehavior.GetGroupsForParentSubgroup(sub).Cast<Group>().ToList();
                    m.CompositeSubgroupsList.Add(new CompositeCollectionSubgroupsAndGroups
                    {
                        Name = sub.NAME,
                        Subgroup = sub,
                        Groups = groupsForParentSubgroup,
                        CompositeSubgroupsList = new List<CompositeCollectionSubgroupsAndGroups>()
                    });
                }

                foreach (CompositeCollectionSubgroupsAndGroups s in m.CompositeSubgroupsList)
                {
                    List<Subgroup> subgroupsForParentSubgroup = subgroupBehavior.GetSubgroupsForParentSubgroup(s.Subgroup).Cast<Subgroup>().ToList();                    

                    foreach (Subgroup sub in subgroupBehavior.GetSubgroupsForParentSubgroup(s.Subgroup).Cast<Subgroup>().ToList())
                    {
                        List<Group> groupsForParentSubgroup = groupBehavior.GetGroupsForParentSubgroup(sub).Cast<Group>().ToList();
                        s.CompositeSubgroupsList.Add(new CompositeCollectionSubgroupsAndGroups
                        {
                            Name = sub.NAME,
                            Subgroup = sub,
                            Groups = groupsForParentSubgroup,
                            CompositeSubgroupsList = new List<CompositeCollectionSubgroupsAndGroups>()
                        });                        
                    }                   
                }
            }
        }

        private void initializeRoomList()
        {
            DepartmentList = departmentBehavior.GetList();

            foreach (Department d in DepartmentList)
            {
                d.LocationsList = locationBehavior.GetLocationsForDepartment(d);

                foreach (Location loc in d.LocationsList)
                {
                    loc.RoomsList = new List<RoomWithDescriptionDataType>();

                    List<Room> rooms = roomBehavior.GetListForLocation(loc);

                    var roomsWithDescription = from r in rooms
                                               select new { Room = r, Description = r.NUMBER + " (" + r.NUMBER_OF_PLACES + ")"};                    

                    foreach (dynamic dyn in roomsWithDescription.ToList())
                    {
                        loc.RoomsList.Add(new RoomWithDescriptionDataType(dyn.Room, dyn.Description));
                    }
                }                
            }
        }

        //public List<TreeViewItem> ItemsList { get; set; }

        //public TreeViewData(serverDBEntities context)
        //{
        //    this.context = context;

        //    majorBehavior = new Major(context);
        //    subgroupBehavior = new Subgroup(context);

        //    ItemsList = new List<TreeViewItem>();
        //    List<Major> majorList = majorBehavior.GetMajorsForDepartment(CurrentUser.Instance.AdminCurrentDepartment);

        //    foreach (Major m in majorList)
        //    {
        //        ItemsList.Add(new TreeViewItem { Header = m.NAME, Focusable = false });
        //        List<Subgroup> subgrouspList = subgroupBehavior.GetSubgroupsForMajor(m);

        //        foreach (Subgroup s in subgrouspList)
        //        {
        //            List<Subgroup> nestedSubgroups = subgroupBehavior.GetSubgroupsForParentSubgroup(s);
        //            ItemsList.Last().Items.Add(new TreeViewItem { Header = s.NAME });

        //            foreach (Subgroup n in nestedSubgroups)
        //            {
        //                ItemCollection itemCollection = ItemsList.Last().Items;
        //                ((TreeViewItem)itemCollection.GetItemAt(itemCollection.Count - 1)).Items.Add(new TreeViewItem { Header = n.NAME });
        //            }
        //        }
        //    }
        //}
    }
}

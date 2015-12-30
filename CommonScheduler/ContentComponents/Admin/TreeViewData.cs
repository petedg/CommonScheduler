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

        public List<Major> MajorList { get; set; }

        public TreeViewData(serverDBEntities context)
        {
            this.context = context;

            majorBehavior = new Major(context);
            subgroupBehavior = new Subgroup(context);

            MajorList = majorBehavior.GetMajorsForDepartment(CurrentUser.Instance.AdminCurrentDepartment);
            if (MajorList.Count == 0)
                MajorList = new List<Major>();

            foreach(Major m in MajorList)
            {
                m.SubgroupsList = subgroupBehavior.GetSubgroupsForMajor(m);
                if (m.SubgroupsList.Count == 0)
                    m.SubgroupsList = new List<Subgroup>();

                foreach (Subgroup s in m.SubgroupsList)
                {
                    s.NestedSubgroupsList = subgroupBehavior.GetSubgroupsForParentSubgroup(s);
                    if (s.NestedSubgroupsList.Count == 0)
                        s.NestedSubgroupsList = new List<Subgroup>();
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

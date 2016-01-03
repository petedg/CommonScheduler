using CommonScheduler.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CommonScheduler.DAL
{
    public class CompositeCollectionSubgroupsAndGroups
    {
        public String Name { get; set; }
        public Subgroup Subgroup { get; set; }
        public List<CompositeCollectionSubgroupsAndGroups> CompositeSubgroupsList { get; set; }
        public List<Group> Groups { get; set; }

        public IList Children
        {
            get
            {
                return new CompositeCollection()
            {
                new CollectionContainer() { Collection = CompositeSubgroupsList },
                new CollectionContainer() { Collection = Groups }
            };
            }
        }
    }
}

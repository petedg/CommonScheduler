﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class SpecialLocation
    {
        private serverDBEntities context;

        public SpecialLocation(serverDBEntities context)
        {
            this.context = context;
        }

        public SpecialLocation GetSpecialLocationById(int specialLocationId)
        {
            var specialLocations = from specialLocation in context.SpecialLocation
                                   where specialLocation.ID == specialLocationId
                                   select specialLocation;

            return specialLocations.FirstOrDefault();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public class RoomWithDescriptionClass
    {
        public Room Room { get; set; }
        public string Description { get; set; }

        public RoomWithDescriptionClass(Room room, string description)
        {
            this.Room = room;
            this.Description = description;
        }
    }
}

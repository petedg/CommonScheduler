using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class Room
    {
        private serverDBEntities context;

        public Room(serverDBEntities context)
        {
            this.context = context;
        }

        public List<Room> GetListForLocation(Location location)
        {
            var rooms = from room in context.Room
                        where room.Location_ID == location.ID && room.ID != 4   // != specjalna lokalizacja
                        select room;

            return rooms.ToList();
        }

        public Room GetRoomById(int roomId)
        {
            var rooms = from room in context.Room
                        where room.ID == roomId
                        select room;

            return rooms.FirstOrDefault();
        }

        public Room AddRoom(Room room)
        {
            return context.Room.Add(room);
        }

        public Room UpdateRoom(Room room)
        {
            context.Room.Attach(room);
            context.Entry(room).State = EntityState.Modified;
            return room;
        }

        public Room DeleteRoom(Room room)
        {
            context.Entry(room).State = EntityState.Deleted;
            return room;
        }

        public void RemoveRoomsForLocation(Location location)
        {
            var rooms = from room in context.Room
                        where room.Location_ID == location.ID
                        select room;

            foreach (Room r in rooms)
            {
                DeleteRoom(r);
            }
        }
    }
}

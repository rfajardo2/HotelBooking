using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int RoomId { get; set; }
        public int ClientId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string Status { get; set; } = "Confirmed";
        public Room? Room { get; set; } = null!;
        public Client? Client { get; set; } = null!;
    }
}

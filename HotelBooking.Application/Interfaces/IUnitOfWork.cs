using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HotelBooking.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Hotel> Hotels { get; }
        IRepository<Room> Rooms { get; }
        IRepository<Reservation> Reservations { get; }
        IRepository<Client> Clients { get; }

        Task<int> CompleteAsync();
    }
}

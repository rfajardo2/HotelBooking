using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Data;
using HotelBooking.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HotelBookingDbContext _context;

        public IRepository<Hotel> Hotels { get; }
        public IRepository<Room> Rooms { get; }
        public IRepository<Reservation> Reservations { get; }
        public IRepository<Client> Clients { get; }

        public UnitOfWork(HotelBookingDbContext context)
        {
            _context = context;
            Hotels = new Repository<Hotel>(context);
            Rooms = new Repository<Room>(context);
            Reservations = new Repository<Reservation>(context);
            Clients = new Repository<Client>(context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

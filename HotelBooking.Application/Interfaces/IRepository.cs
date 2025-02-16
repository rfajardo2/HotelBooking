using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();  // Obtener todos los registros
        Task<T?> GetByIdAsync(int id);       // Obtener un registro por ID
        Task AddAsync(T entity);             // Agregar un registro
        Task UpdateAsync(T entity);          // Actualizar un registro
        Task DeleteAsync(int id);            // Eliminar un registro

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate); // 🔹 Asegurar que AnyAsync esté aquí
        Task<List<T>> WhereAsync(Expression<Func<T, bool>> predicate);


    }
}


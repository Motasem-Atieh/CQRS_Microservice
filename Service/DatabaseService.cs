using CQRS_Microservice.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CQRS_Microservice.Services
{
    public class DatabaseService<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseService<T>> _logger;

        public DatabaseService(ApplicationDbContext context, ILogger<DatabaseService<T>> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Method to get all entities from the database
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        // Method to get an entity by ID from the database
        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        // Method to update an entity in the database
        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Entity of type {typeof(T).Name} updated in the database.");
        }

        // Method to delete an entity from the database
        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Entity of type {typeof(T).Name} deleted from the database.");
        }
    }
}

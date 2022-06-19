using Library.Data;
using Library.Entities;
using Library.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Repositories
{
    public class StoreRepository: IStoreRepository
    {
        private readonly LibraryContext context;

        public StoreRepository(LibraryContext context)
        {
            this.context = context;
        }

        public async Task<List<Store>> GetAll()
        {
            return await context.Stores.ToListAsync();
        }
        public async Task<Store> GetStoreById(int id)
        {
            var store = await context.Stores
              .Include(s => s.StoreBooks)
              .ThenInclude(b => b.Book)
              .FirstOrDefaultAsync(m => m.Id == id);
            return store;
        }
        public async Task AddStore(Store store)
        {
            context.Add(store);
            await context.SaveChangesAsync();
        }
        public async Task<bool> DeleteStore(Store store)
        {
            try
            {
                context.Stores.Remove(store);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

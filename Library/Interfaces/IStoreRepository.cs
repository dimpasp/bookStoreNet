using Library.Entities;

namespace Library.Interfaces
{
    public interface IStoreRepository
    {
        Task<List<Store>> GetAll();
        Task<Store> GetStoreById(int id);
        Task AddStore(Store store);
        Task<bool> DeleteStore(Store store);
    }
}


using Library.Entities;
using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class ContactController : Controller
    {
        private readonly IStoreRepository _storeRepository;

        public ContactController(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.AllStore=await GetAllStores();
            return View();
        }
        public IActionResult CreateEmail()
        {
            return RedirectToAction("Index","Home");
        }
        
        public async Task<List<Store>> GetAllStores()
        {
            var stores = await _storeRepository.GetAll();
            return stores;
        }
    }
}

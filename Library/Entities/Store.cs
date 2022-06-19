using System.ComponentModel.DataAnnotations;

namespace Library.Entities
{
    public class Store
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? PostalCode { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public ICollection<StoreBook>? StoreBooks { get; set; }
    }
}

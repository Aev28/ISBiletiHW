using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Domain.Models
{
    public class Ticket : BaseEntity
    {
        [Required(ErrorMessage = "Please enter a title")]
        [StringLength(255)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please enter a price")]
        [Range(0, Double.PositiveInfinity)]
        public double Price { get; set; }
        [Required(ErrorMessage = "Please enter a date")]
        public DateTime DateValid { get; set; }

        public List<String> genres;

        public String selectedGenre { get; set; }
        public virtual ICollection<TicketInShoppingCart> TicketInCart { get; set; }
        public virtual ICollection<TicketInOrder> TicketInOrder { get; set; }

        public Ticket()
        {
            genres = new List<String>() {
                "Action",
                "Adventure",
                "Comedy",
                "Drama",
                "Fantasy",
                "Horror",
                "Musical",
                "Mystery",
                "Romance",
                "Sci-Fi",
                "Western",
                "Thriller"
           };
            DateValid = DateTime.Now;
            DateValid = DateValid.AddMilliseconds(-DateValid.Millisecond);
        }
    }
}

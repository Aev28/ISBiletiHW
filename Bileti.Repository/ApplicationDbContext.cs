using Bileti.Domain.DTO;
using Bileti.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace Bileti.Repository
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ticket> Ticket { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCart { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<TicketInShoppingCart> TicketInShoppingCart { get; set; }
        public virtual DbSet<TicketInOrder> TicketInOrder { get; set; }
        public virtual DbSet<EmailMessage> EmailMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Ticket>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<ShoppingCart>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<TicketInShoppingCart>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<TicketInShoppingCart>()
                .HasOne(z => z.Ticket)
                .WithMany(z => z.TicketInCart)
                .HasForeignKey(z => z.TicketId);

            builder.Entity<TicketInShoppingCart>()
                .HasOne(z => z.Cart)
                .WithMany(z => z.TicketInCart)
                .HasForeignKey(z => z.CartId);

            builder.Entity<TicketInOrder>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<TicketInOrder>()
                .HasOne(z => z.Ticket)
                .WithMany(z => z.TicketInOrder)
                .HasForeignKey(z => z.TicketId);

            builder.Entity<TicketInOrder>()
                .HasOne(z => z.Order)
                .WithMany(z => z.TicketInOrder)
                .HasForeignKey(z => z.OrderId);
        }
    }
}
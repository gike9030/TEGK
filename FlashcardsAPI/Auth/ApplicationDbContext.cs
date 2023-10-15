using System.Reflection.Emit;
using FlashcardsAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthentication.NET6._0.Auth
{
    public class ApplicationDbContext : IdentityDbContext<FlashcardsAppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Flashcards>()
            .HasOne(e => e.FlashcardCollection)
            .WithMany(e => e.Flashcards)
            .HasForeignKey(e => e.FlashcardCollectionId)
            .IsRequired();

            builder.Entity<FlashcardCollection<Flashcards>>()
            .HasOne(e => e.FlashcardsAppUser)
            .WithMany(e => e.FlashcardCollections)
            .HasForeignKey(e => e.FlashcardsAppUserId)
            .IsRequired();
        }

        public DbSet<FlashcardCollection<Flashcards>> FlashcardCollection { get; set; }
        public DbSet<Flashcards> Flashcards { get; set; }
        public DbSet<Reaction<Flashcards>> Reactions { get; set; }

    }
}
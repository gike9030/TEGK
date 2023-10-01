using System.Reflection.Emit;
using FlashcardsApp.Areas.Identity.Data;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FlashcardsApp.Data;

public class FlashcardsAppContext : IdentityDbContext<FlashcardsAppUser>
{
    public FlashcardsAppContext(DbContextOptions<FlashcardsAppContext> options)
        : base(options)
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

        builder.Entity<FlashcardCollection>()
        .HasOne(e => e.FlashcardsAppUser)
        .WithMany(e => e.FlashcardCollections)
        .HasForeignKey(e => e.FlashcardsAppUserId)
        .IsRequired();

        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    public DbSet<FlashcardCollection> FlashcardCollection { get; set; }
    public DbSet<Flashcards> Flashcards { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
}

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace FlashcardsAPI.Models;

public class FlashcardsAppUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string? FirstName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string? LastName { get; set; }
    public ICollection<FlashcardCollection<Flashcards>> FlashcardCollections { get; set; } = new List<FlashcardCollection<Flashcards>>();

}


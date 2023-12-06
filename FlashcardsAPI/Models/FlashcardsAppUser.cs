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

    public string? ProfilePhotoPath { get; internal set; }
    public string? Description { get; internal set; }

    public ICollection<Following> Followings { get; set;} = new List<Following>();
}


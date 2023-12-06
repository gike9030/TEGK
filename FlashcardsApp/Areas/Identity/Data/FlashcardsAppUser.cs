using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Identity;

namespace FlashcardsApp.Areas.Identity.Data;
public class FlashcardsAppUser : IdentityUser
{

    [PersonalData]
    [Column(TypeName = "nvarchar(500)")]
     public string? FirstName { get; set; }

    public string? ProfilePhotoPath { get; internal set; }
    public string? Description { get; internal set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string? LastName { get; set; }
    public ICollection<FlashcardCollection<Flashcards>> FlashcardCollections { get; set; } = new List<FlashcardCollection<Flashcards>>();
	public ICollection<Following> Followings { get; set; } = new List<Following>();
}


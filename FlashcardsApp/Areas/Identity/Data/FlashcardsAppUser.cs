using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Identity;

namespace FlashcardsApp.Areas.Identity.Data;

// Add profile data for application users by adding properties to the FlashcardsAppUser class
public class FlashcardsAppUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string FirstName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; }

    public ICollection<FlashcardCollection> flashcardCollections { get; set; } = new List<FlashcardCollection>();
}


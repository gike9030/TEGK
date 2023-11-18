
using FlashcardsAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsAPI.Services
{
    public interface IProfileService
    {
        Task<Profile> GetProfileAsync(string userId);
        Task<bool> UpdateDescriptionAsync(string userId, string newDescription);
        Task<bool> UpdateProfilePhotoPathAsync(string userId, string profilePhotoPath);
    }
}
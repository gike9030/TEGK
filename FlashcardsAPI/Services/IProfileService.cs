
using FlashcardsAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsAPI.Services
{
    public interface IProfileService
    {
        public Task<Profile> GetProfileAsync(string userId);
        public  Task<bool> UpdateDescriptionAsync(string userId, string newDescription);
        public Task<bool> UpdateProfilePhotoAsync(string userId, IFormFile profilePhoto);
    }
}
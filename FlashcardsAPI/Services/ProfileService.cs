using FlashcardsAPI.Models;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsAPI.Services
{
    public class ProfileService : IProfileService
    {
        private readonly ApplicationDbContext _context;

        public ProfileService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Profile> GetProfileAsync(string userId)
        {
            var user = await _context.Users
                                     .Where(u => u.Id == userId)
                                     .Select(u => new Profile
                                     {
                                         FirstName = u.FirstName,
                                         LastName = u.LastName,
                                         ProfilePhoto = u.ProfilePhotoPath,
                                         Description = u.Description,
                                         FlashcardCollections = _context.FlashcardCollection
                                               .Where(c => c.FlashcardsAppUserId == userId)
                                               .ToList()
                                     })
                                     .FirstOrDefaultAsync();

            return user;
        }

        public async Task<bool> UpdateDescriptionAsync(string userId, string newDescription)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.Description = newDescription;

            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateProfilePhotoPathAsync(string userId, string profilePhotoPath)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.ProfilePhotoPath = profilePhotoPath;

            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

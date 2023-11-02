using FlashcardsApp.Models;
using System.Text.RegularExpressions;

namespace FlashcardsApp.Services
{
        public class SearchService
        {
            public List<FlashcardCollection<Flashcards>> FilterBySearchTerm(List<FlashcardCollection<Flashcards>> collections, string searchTerm)
            {
                var pattern = Regex.Escape(searchTerm);
                var regex = new Regex(pattern, RegexOptions.IgnoreCase);
                return collections.Where(collection => regex.IsMatch(collection.CollectionName)).ToList();
            }
        }

}

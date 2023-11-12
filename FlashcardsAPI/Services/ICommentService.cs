using FlashcardsAPI.Models;

namespace FlashcardsAPI.Services
{
	public interface ICommentService
	{
		Task<Comment?> GetComment(int id);
		Task<Comment?> AddComment(Comment comment);
	    Task<Comment?> UpdateComment(int id, Comment comment);
		Task<bool> DeleteComment(int id);
		Task<IEnumerable<Comment>?> GetAllComments();
	}
}

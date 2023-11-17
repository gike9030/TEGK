using FlashcardsAPI.Models;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsAPI.Services
{
	public class CommentService : ICommentService
	{
		private readonly ApplicationDbContext _context;
		public CommentService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Comment?> AddComment(Comment comment)
		{
			if (_context.Comments == null)
			{
				return null;
			}

			try
			{
				_context.Comments.Add(comment);
				await _context.SaveChangesAsync();
			}
			catch (Exception)
			{
				return null;
			}

			return comment;
		}

		public async Task<bool> DeleteComment(int id)
		{
			try
			{
				Comment? comment = await _context.Comments.FindAsync(id);

				_context.Comments.Remove(comment);
				await _context.SaveChangesAsync();

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<Comment?> GetComment(int id)
		{
			if (_context.Comments == null)
			{
				return null;
			}

			var comment = await _context.Comments.FindAsync(id);

			return comment;
		}

		public async Task<Comment?> UpdateComment(int id, Comment comment)
		{
			if (_context.Comments == null || id != comment.Id)
			{
				return null;
			}

			_context.Entry(comment).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				return null;
			}

			return comment;
		}

	}
}

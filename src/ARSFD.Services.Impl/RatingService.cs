using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DATABASE = ARSFD.Database;

namespace ARSFD.Services.Impl
{
	public class RatingService: IRatingService
	{
		private DATABASE.ApplicationDbContext _context;

		public RatingService(
			DATABASE.ApplicationDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		#region IRatingService members

		public async Task SetRating(int userId, int value, int byUserId, CancellationToken cancellationToken = default)
		{
			try
			{
				var rating = new DATABASE.Rating
				{
					UserId = userId,
					Value = value,
					ByUserId = byUserId,
				};

				await _context.Ratings.AddAsync(rating, cancellationToken);
				await _context.SaveChangesAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to set rating for user `{userId}`.", ex);
			}
		}

		public async Task<Rating[]> Get(int userId, CancellationToken cancellationToken = default)
		{
			try
			{
				DATABASE.Rating[] ratings = await _context.Ratings
					.Where(x => x.UserId == userId)
					.ToArrayAsync(cancellationToken);

				return ratings
					.Select(x => new Rating
					{
						Id = x.Id,
						ByUserId = x.ByUserId,
						UserId = x.UserId,
						Value = x.Value,
					})
					.ToArray();
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to get ratings for user `{userId}`.", ex);
			}
		}

		#endregion
	}
}

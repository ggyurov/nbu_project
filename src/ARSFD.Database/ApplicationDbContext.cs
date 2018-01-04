using Microsoft.EntityFrameworkCore;

namespace ARSFD.Database
{
	public class ApplicationDbContext: DbContext
	{
		public DbSet<Appointment> Appointments { get; set; }

		public DbSet<Rating> Ratings { get; set; }

		public DbSet<Comment> Comments { get; set; }

		public DbSet<Event> Events { get; set; }

		public DbSet<ApplicationUser> Users { get; set; }

		public DbSet<WorkingHour> WorkingHours { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<ApplicationUser>(b =>
			{
				b.Property(x => x.Id);

				b.Property(x => x.UserName);

				b.Property(x => x.PasswordHash);

				b.Property(x => x.Email);

				b.Property(x => x.EmailConfirmed);

				b.Property(x => x.Role);

				b.Property(x => x.City);

				b.Property(x => x.Name);

				b.Property(x => x.Type);

				b.HasKey(x => x.Id);

				b.ToTable("Users");
			});

			modelBuilder.Entity<Appointment>(b =>
			{
				b.Property(x => x.Id);

				b.Property(x => x.UserId);

				b.Property(x => x.Date);

				b.Property(x => x.DoctorId);

				b.Property(x => x.CanceledById);

				b.Property(x => x.CanceledOn);

				b.HasKey(x => x.Id);

				b.ToTable("Appointments");
			});

			modelBuilder.Entity<Rating>(b =>
			{
				b.Property(x => x.Id);

				b.Property(x => x.UserId);

				b.Property(x => x.ByUserId);

				b.Property(x => x.Value);

				b.HasKey(x => x.Id);

				b.ToTable("Ratings");
			});

			modelBuilder.Entity<Comment>(b =>
			{
				b.Property(x => x.Id);

				b.Property(x => x.Text);

				b.Property(x => x.ByUserId);

				b.Property(x => x.UserId);

				b.Property(x => x.EventId);

				b.HasKey(x => x.Id);

				b.ToTable("Comments");
			});

			modelBuilder.Entity<Event>(b =>
			{
				b.Property(x => x.Id);

				b.Property(x => x.UserId);

				b.Property(x => x.Title);

				b.Property(x => x.Text);

				b.Property(x => x.StartDate);

				b.Property(x => x.EndDate);

				b.HasKey(x => x.Id);

				b.ToTable("Events");
			});

			modelBuilder.Entity<WorkingHour>(b =>
			{
				b.Property(x => x.Id);

				b.Property(x => x.UserId);

				b.Property(x => x.DayOfWeek);

				b.Property(x => x.StartTime);

				b.Property(x => x.EndTime);

				b.HasKey(x => x.Id);

				b.ToTable("WorkingHours");
			});
		}
	}
}

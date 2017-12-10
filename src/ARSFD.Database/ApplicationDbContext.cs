using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ARSFD.Database
{
	public class ApplicationDbContext: IdentityDbContext<ApplicationUser, ApplicationRole, string>
	{
		public DbSet<Appointment> Appointments { get; set; }

		public DbSet<Rating> Ratings { get; set; }

		public DbSet<Comment> Comments { get; set; }

		public DbSet<Event> Events { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			#region Default ASP.NET Identity models

			modelBuilder.Entity<ApplicationRole>(b =>
			{
				b.Property<string>("Id");

				b.Property<string>("ConcurrencyStamp")
					.IsConcurrencyToken();

				b.Property<string>("Name")
					.HasAnnotation("MaxLength", 256);

				b.Property<string>("NormalizedName")
					.HasAnnotation("MaxLength", 256);
				b.HasKey("Id");

				b.HasIndex("NormalizedName")
					.HasName("RoleNameIndex");

				b.ToTable("Roles");
			});

			modelBuilder.Entity<IdentityRoleClaim<string>>(b =>
			{
				b.Property<int>("Id")
					.ValueGeneratedOnAdd();

				b.Property<string>("ClaimType");

				b.Property<string>("ClaimValue");

				b.Property<string>("RoleId")
					.IsRequired();

				b.HasKey("Id");

				b.HasIndex("RoleId");

				b.ToTable("RoleClaims");

				b.HasOne<ApplicationRole>()
					.WithMany("Claims")
					.HasForeignKey("RoleId")
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<IdentityUserClaim<string>>(b =>
			{
				b.Property<int>("Id")
					.ValueGeneratedOnAdd();

				b.Property<string>("ClaimType");

				b.Property<string>("ClaimValue");

				b.Property<string>("UserId")
					.IsRequired();

				b.HasKey("Id");

				b.HasIndex("UserId");

				b.ToTable("UserClaims");

				b.HasOne<ApplicationUser>()
					.WithMany("Claims")
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<IdentityUserLogin<string>>(b =>
			{
				b.Property<string>("LoginProvider");

				b.Property<string>("ProviderKey");

				b.Property<string>("ProviderDisplayName");

				b.Property<string>("UserId")
					.IsRequired();

				b.HasKey("LoginProvider", "ProviderKey");

				b.HasIndex("UserId");

				b.ToTable("UserLogins");

				b.HasOne<ApplicationUser>()
					.WithMany("Logins")
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<IdentityUserRole<string>>(b =>
			{
				b.Property<string>("UserId");

				b.Property<string>("RoleId");

				b.HasKey("UserId", "RoleId");

				b.HasIndex("RoleId");

				b.HasIndex("UserId");

				b.ToTable("UserRoles");

				b.HasOne<ApplicationRole>()
					.WithMany("Users")
					.HasForeignKey("RoleId")
					.OnDelete(DeleteBehavior.Cascade);

				b.HasOne<ApplicationUser>()
					.WithMany("Roles")
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<IdentityUserToken<string>>(b =>
			{
				b.Property<string>("UserId");

				b.Property<string>("LoginProvider");

				b.Property<string>("Name");

				b.Property<string>("Value");

				b.HasKey("UserId", "LoginProvider", "Name");

				b.ToTable("UserTokens");
			});

			modelBuilder.Entity<ApplicationUser>(b =>
			{
				b.Property<string>("Id");

				b.Property<int>("AccessFailedCount");

				b.Property<string>("ConcurrencyStamp")
					.IsConcurrencyToken();

				b.Property<string>("Email")
					.HasAnnotation("MaxLength", 256);

				b.Property<bool>("EmailConfirmed");

				b.Property<bool>("LockoutEnabled");

				b.Property<DateTimeOffset?>("LockoutEnd");

				b.Property<string>("NormalizedEmail")
					.HasAnnotation("MaxLength", 256);

				b.Property<string>("NormalizedUserName")
					.HasAnnotation("MaxLength", 256);

				b.Property<string>("PasswordHash");

				b.Property<string>("PhoneNumber");

				b.Property<bool>("PhoneNumberConfirmed");

				b.Property<string>("SecurityStamp");

				b.Property<bool>("TwoFactorEnabled");

				b.Property<string>("UserName")
					.HasAnnotation("MaxLength", 256);

				b.HasKey("Id");

				b.HasIndex("NormalizedEmail")
					.HasName("EmailIndex");

				b.HasIndex("NormalizedUserName")
					.IsUnique()
					.HasName("UserNameIndex");

				b.ToTable("Users");
			});

			#endregion

			#region ARSFD models

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

			#endregion
		}
	}
}

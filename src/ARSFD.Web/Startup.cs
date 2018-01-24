﻿using System;
using ARSFD.Services.Impl;
using ARSFD.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using DATABASE = ARSFD.Database;
using SERVICES = ARSFD.Services;

namespace ARSFD.Web
{
	public class Startup
	{
		private IConfigurationRoot Configuration { get; }

		private IHostingEnvironment HostingEnvironment { get; }

		public Startup(IHostingEnvironment env)
		{
			IConfigurationBuilder builder = new ConfigurationBuilder()
				.SetBasePath(basePath: env.ContentRootPath)
				.AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile(path: $"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();

			HostingEnvironment = env;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddDbContext<DATABASE.ApplicationDbContext>(options =>
				{
					string connectionString = Configuration.GetConnectionString("DefaultConnection");
					options.UseSqlServer(connectionString);
				});

			services
				.AddTransient<IEmailSender, EmailSender>();

			services
				.AddIdentity<SERVICES.ApplicationUser, ApplicationRole>(options =>
				{
					options.Password.RequireDigit = false;
					options.Password.RequiredLength = 5;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
					options.Password.RequireLowercase = false;
					options.Password.RequiredUniqueChars = 2;
				})
				.AddUserStore<ApplicationUserStore>()
				.AddRoleStore<ApplicationRoleStore>()
				.AddDefaultTokenProviders();

			services
				.AddScoped<SERVICES.IAppointmentService, AppointmentService>()
				.AddScoped<SERVICES.ICommentService, CommentService>()
				.AddScoped<SERVICES.IUserService, UserService>()
				.AddScoped<SERVICES.IEventService, EventService>()
				.AddScoped<SERVICES.IRatingService, RatingService>();

			services.AddAuthentication();

			services.AddAuthorization(options =>
			{
				options.AddPolicy(nameof(SERVICES.RoleType.Doctor), config =>
				{
					string roleName = Enum.GetName(typeof(SERVICES.RoleType), SERVICES.RoleType.Doctor);

					config.RequireClaim(ApplicationRole.ClaimType, roleName);
				});

				options.AddPolicy(nameof(SERVICES.RoleType.Patient), config =>
				{
					string roleName = Enum.GetName(typeof(SERVICES.RoleType), SERVICES.RoleType.Patient);

					config.RequireClaim(ApplicationRole.ClaimType, roleName);
				});
			});

			// add in-memory caching
			services.AddMemoryCache();

			// add response compression middleware
			services.AddResponseCompression();

			// add response caching
			services.AddResponseCaching();

			services.AddMvc()
				.AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
				.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
				.AddDataAnnotationsLocalization();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();

			app.UseAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}

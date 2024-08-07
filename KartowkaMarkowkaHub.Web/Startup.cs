﻿using FluentValidation;
using FluentValidation.AspNetCore;
using KartowkaMarkowkaHub.Core.Abstractions.Repositories;
using KartowkaMarkowkaHub.Core.Domain;
using KartowkaMarkowkaHub.Data;
using KartowkaMarkowkaHub.Data.Repositories;
using KartowkaMarkowkaHub.Services.Account;
using KartowkaMarkowkaHub.Services.Identity;
using KartowkaMarkowkaHub.Web.Validators;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;

namespace KartowkaMarkowkaHub.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Метод для добавления служб в контейнер DI (Default name)
        // Add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            #region Авторизация
            //// Добавление и настройка служб авторизации
            //services.AddAuthorization(options =>
            //{
            //    // Пример политики авторизации
            //    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            //});

            //// Добавление аутентификации (например, с использованием cookie)
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //.AddCookie(options =>
            //{
            //    options.LoginPath = "/Account/Login";
            //    options.AccessDeniedPath = "/Account/AccessDenied";
            //});
            #endregion
        }

        // Метод для добавления служб в контейнер DI
        // Add services to the container.
        public void DependencyInjectionRegistration(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //https://github.com/FluentValidation/FluentValidation.AspNetCore
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining(typeof(UserViewModelValidator));



            if (DbConfiguration.CurrentDbType == DbType.SqlLite)
            {
                services.AddDbContext<HubContext>(o => o.UseSqlite());
            }

            //services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "KartowkaMarkowkaHub ", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IUserService, UserService>();

            //Авторизация
            services.AddAuthorization();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // указывает, будет ли валидироваться издатель при валидации токена
                        ValidateIssuer = true,
                        // строка, представляющая издателя
                        ValidIssuer = AuthOptions.ISSUER,
                        // будет ли валидироваться потребитель токена
                        ValidateAudience = true,
                        // установка потребителя токена
                        ValidAudience = AuthOptions.AUDIENCE,
                        // будет ли валидироваться время существования
                        ValidateLifetime = true,
                        // установка ключа безопасности
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        // валидация ключа безопасности
                        ValidateIssuerSigningKey = true,
                    };
                });
        }
        // Метод для настройки конвейера HTTP-запросов
        // Configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<HubContext>();
                DbHelper.Initialize(dbContext);

                //dbContext.Database.EnsureDeleted();
                //dbContext.Database.EnsureCreated();
                //dbContext.Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } 
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Наиболее гибкий вариант роутинга.
                endpoints.MapControllers();

                #region Как могло бы быть
                // Основной маршрут для контроллеров без области
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Маршрут для области Admin
                endpoints.MapAreaControllerRoute(
                    name: "admin",
                    areaName: "Admin",
                    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

                // Маршрут для области Фермер
                endpoints.MapAreaControllerRoute(
                    name: "farmer",
                    areaName: "Farmer",
                    pattern: "Farmer/{controller=Home}/{action=Index}/{id?}");

                // Маршрут для области Клиент
                endpoints.MapAreaControllerRoute(
                    name: "client",
                    areaName: "Client",
                    pattern: "Client/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "Account",
                    areaName: "Account",
                    pattern: "Account/{controller=Home}/{action=Index}/{id?}");
                #endregion
            });

            // Включаем middleware для генерации Swagger JSON и Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = "swagger"; // Чтобы Swagger UI был доступен по корню приложения
            });
        }
    }
}

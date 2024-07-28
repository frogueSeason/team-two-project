﻿using Microsoft.AspNetCore.Authentication.Cookies;

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
        }

        // Метод для добавления служб в контейнер DI
        // Add services to the container.
        public void DependencyInjectionRegistration(IServiceCollection services)
        {
            services.AddControllersWithViews();

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
        }

        // Метод для настройки конвейера HTTP-запросов
        // Configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Наиболее гибкий вариант роутинга.
                endpoints.MapControllers();


                //// Основной маршрут для контроллеров без области
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");

                //// Маршрут для области Admin
                //endpoints.MapAreaControllerRoute(
                //    name: "admin",
                //    areaName: "Admin",
                //    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

                //endpoints.MapAreaControllerRoute(
                //    name: "User",
                //    areaName: "User",
                //    pattern: "User/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

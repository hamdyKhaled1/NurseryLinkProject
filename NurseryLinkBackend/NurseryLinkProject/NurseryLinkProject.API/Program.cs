using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NurseryLinkProject.Domain.Data;
using System.Reflection;

namespace NurseryLinkProject.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // add swagger configuration
            builder.Services.AddSwaggerGen(builder =>
            {
                builder.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "NurseryLink API",
                    Version = "v1",
                    Description = "API for NurseryLink Project"
                });
            });

            // Add connection to the database
            builder.Services.AddDbContext<AppDbContext>(ops =>
            {
                ops.UseSqlServer(builder.Configuration.GetConnectionString("NurseryLinkConn"));
            });

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
            });
            mapperConfig.AssertConfigurationIsValid();

            IMapper mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);

            builder.Services.AddCors(ops =>
            {
                ops.AddPolicy("nursryLink", p =>
                {
                    p.AllowAnyOrigin();
                    p.AllowAnyMethod();
                    p.AllowAnyHeader();
                });
            });

            var app = builder.Build();

            app.UseCors("nursryLink");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NurseryLink API V1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
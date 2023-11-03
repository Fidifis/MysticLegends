using Microsoft.EntityFrameworkCore;
using MysticLegendsServer.Models;
using System.Text.Json.Serialization;

namespace MysticLegendsServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

#if DEBUG
            var connectionString = builder.Configuration.GetConnectionString("GameDB");
#else
            var connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRING") ?? throw new Exception("No CONNECTIONSTRING env variable defined");
#endif

            // Add services to the container.

            builder.Services.AddDbContext<Xdigf001Context>(options => options
                .UseNpgsql(connectionString,
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            // Add logging configuration
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
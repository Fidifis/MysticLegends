namespace MysticLegendsServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            var connectionString = "Host=db.kii.pef.czu.cz;Username=xdigf001;Password=nbs0e2;Database=xdigf001;Pooling=true;MaxPoolSize=3;";
#else
            var connectionString = Environment.GetEnvironmentVariable("CONSTRING") ?? throw new Exception("No CONSTRING env variable defined");
#endif

            DB.OpenConnection(connectionString);

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Add logging configuration
            //builder.Logging.AddConsole();

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
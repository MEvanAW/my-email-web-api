using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyEmailWebApi;
using MyEmailWebApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string defaultConnection = "DefaultConnection";
builder.Services.AddDbContext<EmailContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(defaultConnection)));
builder.Services.AddDbContext<UserContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(defaultConnection)));
builder.Services.AddDbContext<KpiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(defaultConnection)));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(
    options => options.SignIn.RequireConfirmedAccount = false)
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<UserContext>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<Seeder>();

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

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetService<Seeder>();
    if (seeder is not null)
    {
        var rolesTask = seeder.SeedRoles();
        await rolesTask;
        try
        {
            await seeder.SeedKpis();
        }
        catch (Exception e)
        {
            var logger = scope.ServiceProvider.GetService<ILogger>();
            if (logger is not null)
            {
                logger.LogError(e.ToString());
            }
            else
            {
                Console.WriteLine(e);
            }
        }
    }
}

app.Run();

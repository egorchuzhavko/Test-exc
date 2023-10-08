using Microsoft.EntityFrameworkCore;
using VebTechTest;
using VebTechTest.EFCore;
using VebTechTest.Interfaces;
using VebTechTest.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<EFDataContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionStr"))
    );
builder.Services.AddControllers();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<RoleFiller>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

/*
To fullfill roles in db 
In Terminal:
1) cd VebTechTest
2) dotnet run filldata
 */
if (args.Length == 1 && args[0].ToLower() == "filldata")
    SeedData(app);
   

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

void SeedData(IHost app) {
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using(var scope = scopedFactory.CreateScope()) {
        var service = scope.ServiceProvider.GetService<RoleFiller>();
        service.FillRoles();
    }
}
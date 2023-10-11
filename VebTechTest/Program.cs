using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using VebTechTest;
using VebTechTest.EFCore;
using Microsoft.IdentityModel.Tokens;
using VebTechTest.Interfaces;
using VebTechTest.Repository;
using System.Text;
using Serilog;
using Microsoft.CodeAnalysis.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<EFDataContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionStr"))
    );
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<RoleFiller>();
builder.Services.AddSwaggerGen();
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.AddSerilog(logger);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c => {
        c.SwaggerDoc("v1", new OpenApiInfo {
            Title = "API users VebTech Test",
            Version = "v1",
            Description = "Entity framework + Postgresql (connection string in appsettings.json in project)\n\n" +
            "Before first use api you should make few steps:\n1) Open project in VS;\n" +
            "2) Open package manager console;\n3) Write 'Update-Database'\n" +
            "4) Then open terminal;\n5) Write in terminal 'cd vebTechTest';\n" +
            "6) Then write in terminal 'dotnet run filldata';\n" +
            "7) Press the keyboard shortcut ctrl+c.\n\n" +
            "This is necessary to populate the database with data.\n\n"+
            "To Get JWT token you have to make post request 'LoginJwt' where you have to write " + "" +
            "In name 'Log' and in password 'Pas'. Then you have to get your token in write him in authorization panel."
        });
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }});
    });
builder.Services.AddAuthentication(x => {
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.TokenValidationParameters = new TokenValidationParameters {
        ValidIssuer = "UserTest",
        ValidAudience = "http://localhost:51398",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("shYCEfQDeKhAkLKmnigpPDDAkD__FdsFbDg")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization();

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
    app.UseSwaggerUI(c=> {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API users VebTech Test");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
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
using EventScheduler;
using EventScheduler.Config;
using EventScheduler.Data;
using EventScheduler.Data.Model;
using EventScheduler.Helpers;
using EventScheduler.Filetrs;
using EventScheduler.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using EventScheduler.Services.Exceptions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers(op =>
{
    op.Filters.Add<ExternalDependencyExceptionFiletrAttribute>();
})
.ConfigureApiBehaviorOptions(op =>
{
    op.ClientErrorMapping[StatusCodes.Status500InternalServerError] = new Microsoft.AspNetCore.Mvc.ClientErrorData
    {
        Link = "https://errorcodes/status500",
        Title = "Something went wrong , Please contact suppot team."
    };
});

builder.Services.AddLogging();

builder.Services.AddTransient<ProblemDetailsFactory, AppProblemDetailsFactory>();

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("EventSchedulerDB")));
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("EventSchedulerApp"));// Use in memory db

builder.Services.RegisterEventSchedulerServices();

// For Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.Authority = "https://localhost:7130";
    options.Audience = configuration["JWT:ValidAudience"];
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});

builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Event Scheduler API",
        Description = "This is Scheduler App API.",
        Version = "v1",
        TermsOfService = new Uri("https://google/terms"),
        Contact = new OpenApiContact
        {
            Name = "Customer connect",
            Url = new Uri("https://google/customer/contactus")
        },
        License = new OpenApiLicense
        {
            Name = "API Licence",
            Url = new Uri("https://google/eventscheduler/licence")
        }
    });
    var xmlPath = Path.Combine(AppContext.BaseDirectory
        , $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    o.IncludeXmlComments(xmlPath);

    o.AddSecurityDefinition("ApiBearer", new OpenApiSecurityScheme
    {
        Description = "Please eneter a valid token",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference =  new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiBearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI();

   app.UseExceptionHandler("/errorhandler/error-development");
}
else
{
    //app.UseExceptionHandler("/errorhandler/error");
    //app.UseCustomExceptionHandler(app.Services.GetService<ILogger<IEventServiceException>>());
    app.UseCustomException();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.SeedUsers();
app.MapControllers();

app.Run();

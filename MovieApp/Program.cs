using GOSBackend.DI_Intallers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x => {
    x.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GOS SOftware Website",
        Version = "V1",
        Description = "An API to perform business automated operations",
        TermsOfService = new Uri("http://www.godp.co.uk/"),
        Contact = new OpenApiContact
        {
            Name = "Joshua Chukwu",
            Email = "joshua.chukwu@godp.com.uk",
            Url = new Uri("https://github.com/JoshuaTChukwu"),
        },
        License = new OpenApiLicense
        {
            Name = "GODP API LICX",
            Url = new Uri("http://www.godp.co.uk/"),
        },

    });

    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //x.IncludeXmlComments(xmlPath);

    var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer ", new string[0] }
                };
    x.AddSecurityDefinition("Bearer ", new OpenApiSecurityScheme
    {
        Description = "GODP Cloud Authorization header using bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    });
    x.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {new OpenApiSecurityScheme {Reference = new OpenApiReference
                    {
                        Id = "Bearer ",
                        Type = ReferenceType.SecurityScheme
                    } }, new List<string>() }
                });
}
);
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddFile("logs/myapp-{Date}.txt");
});
var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

builder.Services.InstallServicesInAssembly(config);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();

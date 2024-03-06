
using Chat.Filters;
using Chat.Models;
using Chat.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;


#region SERVICES

var builder = WebApplication.CreateBuilder(args);

string connectionString;
string secret;
// Add services to the container.


bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
//if (!isDevelopment)
//{
//    connectionString = builder.Configuration["CONNECTION_STRING"];
//    secret = builder.Configuration["ClaveJWT"];
//}
//else
//{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    secret = builder.Configuration["ClaveJWT"];
//}

builder.Services.AddControllers(options =>
{
    options.Filters.Add<FiltroDeExcepcion>();
}).AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChatContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);

}
);
builder.Services.AddTransient<HistorialChat_Service>();
builder.Services.AddTransient<HashService>();
builder.Services.AddTransient<TokenService>();
builder.Services.AddTransient<ArchivosService>();
builder.Services.AddTransient<EmailService>();
builder.Services.AddHttpContextAccessor();


// Para utilizar SignalR debemos agregar el servicio SignalR mediante un método especial para ello
builder.Services.AddSignalR();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(
                                        //Encoding.UTF8.GetBytes(secret ?? "ratones"))
                                        Encoding.UTF8.GetBytes(secret))

               });



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
});

//+ CORS versipon original
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        //builder.WithOrigins("https://www.apirequest.io").AllowAnyMethod().AllowAnyHeader();
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        //builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials();

    });
});

#endregion

#region APP
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseCors();
//¡-------------------------------------------------------
//+CORS MODIFICADO
//app.UseCors("PoliticasCorsChat");



//app.UseMiddleware<LogFileIPMiddleware>();


//iniciar nuestro servidor con index.html
app.UseFileServer(); //PARA QUE TE LLEVE AL INDEX.HTML DEL WWWROOT
app.UseStaticFiles();

// Para que la relación cliente/servidor en tiempo real sea posible, debemos hacer esta configuración aquí
// app.UseRouting() especifa que va a haber una ruta a la que un cliente se conecte a nuestro servidor en tiempo real
app.UseRouting();
// app.UseEndpoints describe cual va a ser esa ruta
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chatHub"); // El acceso al hub sería vía https://localhost:puerto/chatHub

});

app.UseAuthorization();

app.MapControllers();

app.Run();
#endregion
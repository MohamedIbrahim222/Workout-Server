   using workOut.Data;
   using Microsoft.AspNetCore.Authentication.JwtBearer;
   using Microsoft.EntityFrameworkCore;
   using Microsoft.IdentityModel.Tokens;
   using System.Text;
   using Microsoft.OpenApi.Models;
   using workOut.Services;
using workOut.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

   builder.Services.AddControllers();

   builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// builder.Services.AddIdentity<User, IdentityRole>()
    // .AddEntityFrameworkStores<AppDbContext>();
    
   builder.Services.AddEndpointsApiExplorer();
   builder.Services.AddSwaggerGen(c =>
   {
       c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
       {
           Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
           Name = "Authorization",
           In = ParameterLocation.Header,
           Type = SecuritySchemeType.ApiKey,
           Scheme = "Bearer"
       });

       c.AddSecurityRequirement(new OpenApiSecurityRequirement()
       {
           {
               new OpenApiSecurityScheme
               {
                   Reference = new OpenApiReference
                   {
                       Type = ReferenceType.SecurityScheme,
                       Id = "Bearer"
                   },
                   Scheme = "oauth2",
                   Name = "Bearer",
                   In = ParameterLocation.Header,
               },
               new List<string>()
           }
       });
   });

   var jwtSettings = builder.Configuration.GetSection("JwtSettings");
   var secretKey = jwtSettings.GetValue<string>("SecretKey");

   builder.Services.AddAuthentication(options =>
   {
       options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
       options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
   })
   .AddJwtBearer(options =>
   {
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
           ValidAudience = jwtSettings.GetValue<string>("Audience"),
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
       };
   });

   builder.Services.AddScoped<IUserService, UserService>();

   builder.Services.AddCors(options =>
   {
       options.AddPolicy("AllowSpecificOrigin",
           builder => builder.WithOrigins("http://localhost:4200") // Adjust for frontend URL
                             .AllowAnyHeader()
                             .AllowAnyMethod());
   });

   builder.Logging.ClearProviders();
   builder.Logging.AddConsole();

   var app = builder.Build();

   if (app.Environment.IsDevelopment())
   {
       app.UseSwagger();
       app.UseSwaggerUI();
   }

   app.UseHttpsRedirection();

   app.UseAuthentication();
   app.UseAuthorization();

   app.MapControllers();

   app.UseCors("AllowSpecificOrigin");

   app.UseExceptionHandler("/Error");

   app.Run();
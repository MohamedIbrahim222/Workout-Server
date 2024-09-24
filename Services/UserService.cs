     // Services/UserService.cs
     using workOut.Data;
     using workOut.DTOs;
     using workOut.Models;
     using Microsoft.EntityFrameworkCore;
     using Microsoft.Extensions.Configuration;
     using Microsoft.IdentityModel.Tokens;
     using System;
     using System.IdentityModel.Tokens.Jwt;
     using System.Security.Claims;
     using System.Text;
     using System.Threading.Tasks;

     namespace workOut.Services
     {
         public class UserService : IUserService
         {
             private readonly AppDbContext _context;
             private readonly IConfiguration _configuration;

             public UserService(AppDbContext context, IConfiguration configuration)
             {
                 _context = context;
                 _configuration = configuration;
             }

             public async Task<User> RegisterAsync(UserRegisterDto registerDto)
             {
                 if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                     throw new ApplicationException("Email already exists.");

                 var user = new User
                 {
                     Email = registerDto.Email,
                     PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
                 };

                 _context.Users.Add(user);
                 await _context.SaveChangesAsync();

                 return user;
             }

             public async Task<string> LoginAsync(UserLoginDto loginDto)
             {
                 var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                 if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                     throw new ApplicationException("Invalid credentials.");

                 return GenerateJwtToken(user);
             }

             public async Task<User> GetByIdAsync(int id)
             {
                 return await _context.Users.FindAsync(id);
             }

             private string GenerateJwtToken(User user)
             {
                 var jwtSettings = _configuration.GetSection("JwtSettings");
                 var secretKey = jwtSettings.GetValue<string>("SecretKey");
                 var issuer = jwtSettings.GetValue<string>("Issuer");
                 var audience = jwtSettings.GetValue<string>("Audience");
                 var expiryInMinutes = jwtSettings.GetValue<int>("ExpiryInMinutes");

                 var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                 var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                 var claims = new[]
                 {
                     new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                     new Claim("id", user.Id.ToString())
                 };

                 var token = new JwtSecurityToken(
                     issuer,
                     audience,
                     claims,
                     expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                     signingCredentials: credentials
                 );

                 return new JwtSecurityTokenHandler().WriteToken(token);
             }
         }
     }
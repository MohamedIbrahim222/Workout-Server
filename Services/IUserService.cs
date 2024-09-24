     // Services/IUserService.cs
     using workOut.DTOs;
     using workOut.Models;
     using System.Threading.Tasks;

     namespace workOut.Services
     {
         public interface IUserService
         {
             Task<User> RegisterAsync(UserRegisterDto registerDto);
             Task<string> LoginAsync(UserLoginDto loginDto);
             Task<User> GetByIdAsync(int id);
         }
     }
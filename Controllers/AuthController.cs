   // Controllers/AuthController.cs
   using workOut.DTOs;
   using workOut.Services;
   using Microsoft.AspNetCore.Mvc;
   using System.Threading.Tasks;

   namespace workOut.Controllers
   {
       [ApiController]
       [Route("api/[controller]")]
       public class AuthController : ControllerBase
       {
           private readonly IUserService _userService;

           public AuthController(IUserService userService)
           {
               _userService = userService;
           }

           
           [HttpPost("register")]
           public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
           {
               if (!ModelState.IsValid)
                   return BadRequest(ModelState);

               try
               {
                   await _userService.RegisterAsync(registerDto);
                   return Ok(new { message = "User registered successfully." });
               }
               catch (ApplicationException ex)
               {
                   return BadRequest(new { message = ex.Message });
               }
           }

           [HttpPost("login")]
           public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
           {
               if (!ModelState.IsValid)
                   return BadRequest(ModelState);

               try
               {
                   var token = await _userService.LoginAsync(loginDto);
                   return Ok(new { Token = token });
               }
               catch (ApplicationException ex)
               {
                   return Unauthorized(new { message = ex.Message });
               }
           }
       }
   }
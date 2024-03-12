using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace API.Controllers{

    public class AccountController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        
        public AccountController(UserManager<ApplicationUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }  

        [HttpPost("register")]  // POST api/account/register
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if(await UserExists(registerDTO.UserName))
                return BadRequest("Username is taken");
            
            var user = _mapper.Map<ApplicationUser>(registerDTO);

            user.UserName = registerDTO.UserName.ToLower();

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if(!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if(!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            return new UserDTO 
            { 
                UserName = user.UserName, 
                Token = await _tokenService.CreateToken(user), 
                KnownAs = user.KnownAs,
                Gender = user.Gender 
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.Users
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(u => u.UserName == loginDTO.UserName.ToLower());

            if(user == null) return Unauthorized("Invalid username!");

            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if(!result) return Unauthorized("Invalid password!");

            return new UserDTO 
            { 
                UserName = user.UserName, 
                Token = await _tokenService.CreateToken(user), 
                ImageUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.ImageUrl,
                KnownAs = user.KnownAs,
                Gender = user.Gender 
            };
        }

        //Check if user exist, cannot have same username
        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName == username.ToLower());
        }
    }
}

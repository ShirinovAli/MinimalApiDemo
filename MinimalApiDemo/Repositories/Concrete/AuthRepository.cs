using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalApiDemo.Data;
using MinimalApiDemo.DTOs;
using MinimalApiDemo.Models;
using MinimalApiDemo.Repositories.Abstract;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalApiDemo.Repositories.Concrete
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private string _secretKey;

        public AuthRepository(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _secretKey = _configuration.GetValue<string>("APISettings:Secret");
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = await _context.LocalUsers.SingleOrDefaultAsync(x => x.Username.ToLower() == loginRequestDto.Username.ToLower()
                                                                      && x.Password == loginRequestDto.Password);

            if (user == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,user.Username),
                    new Claim(ClaimTypes.Role,user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDto loginResponseDto = new()
            {
                User = _mapper.Map<UserDto>(user),
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };

            return loginResponseDto;
        }

        public bool IsUniqueUser(string username)
        {
            var user = _context.LocalUsers.SingleOrDefault(x => x.Username.ToLower() == username.ToLower());
            if (user == null)
                return true;
            return false;
        }

        public async Task<UserDto> Register(RegistrationRequestDto registrationRequestDto)
        {
            LocalUser user = new()
            {
                Username = registrationRequestDto.Username,
                Password = registrationRequestDto.Password,
                Name = registrationRequestDto.Name,
                Role = "admin"
            };

            await _context.LocalUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }
    }
}

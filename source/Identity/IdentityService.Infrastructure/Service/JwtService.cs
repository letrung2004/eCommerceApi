using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces.Security;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace IdentityService.Infrastructure.Service
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }


        public TokenResponse GenerateToken(User user)
        {
            // Tạo access token
            var accessToken = GenerateAccessToken(user);

            // Tạo refresh token
            var refreshToken = GenerateRefreshToken();

            // Trả về response
            return new TokenResponse
            {
                AccessToken = accessToken.Token,
                AccessTokenExpiration = accessToken.Expiration,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.Expiration
            };
        }

        private (string Token, DateTime Expiration) GenerateAccessToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey); // lấy secret key để tạo token
            
            // lấy user permission để add vào scope dể protect các api
            var permissions = user.Role.RolePermissions
                .Select(rp=>rp.Permission.Name).ToList();

            // gộp các permission lại 
            var scopeValue = string.Join(" ", permissions);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("role", user.Role.Name), // lấy user role để lúc decode trả về role để protect các api
                new("scope", scopeValue) // add các permission vào token
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return (
                tokenHandler.WriteToken(token),
                tokenDescriptor.Expires!.Value
            );
        }

        private (string Token, DateTime Expiration) GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            string token = Convert.ToBase64String(randomNumber);
            DateTime expiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

            return (token, expiration);
        }
    }
}

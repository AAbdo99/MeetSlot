// Denne mappen inneholder kode for a generere og validere JWT-tokens, og inneholder forretningslogikk for autentisering og autorisasjon.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MeetSlot.Models;
using Microsoft.IdentityModel.Tokens;

namespace MeetSlot.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(AppUser user)
        {
            // Claims er informasjon vi legger inn i tokenet.
            // Slik at API-et vet hvem brukeren er og hvilken rolle de har.
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            // Henter den hemmelige nøkkelen fra appsettings.json.
            // Og bruker den til a signere tokenet.
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            // Oppretter en signeringsmetode ved hjelp av den hemmelige nøkkelen.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Oppretter et token med claims, utløpsdato og signeringsmetode.
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            // Genererer tokenet som en streng.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
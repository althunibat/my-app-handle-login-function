using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godwit.Common.Data.Model;
using JsonWebToken;
using Microsoft.Extensions.Configuration;

namespace Godwit.HandleLoginAction.Services {
    public class JwtService : IJwtService {
        private readonly IConfiguration _configuration;
        private readonly AsymmetricJwk _key;

        public JwtService(X509Certificate2 certificate, IConfiguration configuration) {
            _configuration = configuration;
            _key = Jwk.FromX509Certificate(certificate, true);
        }

        public string GenerateToken(User user) {
            var descriptor = new JwsDescriptor {
                SigningKey = _key,
                IssuedAt = DateTime.UtcNow,
                ExpirationTime = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["ISSUER"],
                Audience = _configuration["AUDIENCE"],
                Subject = user.Id,
                Algorithm = SignatureAlgorithm.RsaSha256
            };
            descriptor.AddClaim("https://hasura.io/jwt/claims", JsonSerializer.Serialize(new HasuraClaim {
                UserId = user.Id,
                DefaultRole = "user",
                Roles = new[] {"user"}
            }));
            var writer = new JwtWriter();
            return writer.WriteTokenString(descriptor);
        }
    }

    public class HasuraClaim {
        [JsonPropertyName("x-hasura-user-id")] public string UserId { get; set; }

        [JsonPropertyName("x-hasura-default-role")]
        public string DefaultRole { get; set; }

        [JsonPropertyName("x-hasura-allowed-roles")]
        public string[] Roles { get; set; }
    }
}
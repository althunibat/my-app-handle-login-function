using System.Text.Json.Serialization;

namespace Godwit.HandleLoginAction.Model {
    public class LoginViewModel {
        [JsonPropertyName("username")] public string UserName { get; set; }

        [JsonPropertyName("password")] public string Password { get; set; }
    }
}
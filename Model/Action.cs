using System.Text.Json.Serialization;

namespace Godwit.HandleLoginAction.Model {
    public class Action {
        [JsonPropertyName("name")] public string Name { get; set; }
    }
}
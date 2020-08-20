using Godwit.Common.Data.Model;

namespace Godwit.HandleLoginAction.Services {
    public interface IJwtService {
        string GenerateToken(User user);
    }
}
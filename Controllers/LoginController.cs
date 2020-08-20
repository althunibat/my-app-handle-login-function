using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Godwit.Common.Data.Model;
using Godwit.HandleLoginAction.Model;
using Godwit.HandleLoginAction.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Godwit.HandleLoginAction.Controllers {
    [ApiController]
    [Route("")]
    public class LoginController : ControllerBase {
        private readonly IJwtService _jwtTokenHandler;
        private readonly IValidator<ActionData> _validator;
        private readonly UserManager<User> _manager;

        public LoginController(IValidator<ActionData> validator, IJwtService jwtTokenHandler,
            UserManager<User> manager) {
            _validator = validator;
            _jwtTokenHandler = jwtTokenHandler;
            _manager = manager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ActionData model) {
            var validation = _validator.Validate(model);
            if (!validation.IsValid)
                return Ok(new {
                    Errors = validation.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            var user = await _manager.FindByNameAsync(model.Input.UserName)
                .ConfigureAwait(false);
            if (user == null ||
                _manager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Input.Password) !=
                PasswordVerificationResult.Success)
                return Ok(new {
                    Errors = new[] {"Invalid Credentials"}
                });
            return Ok(new {AccessToken = _jwtTokenHandler.GenerateToken(user), user.Id});
        }
    }
}
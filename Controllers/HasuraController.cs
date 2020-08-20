using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Godwit.Common.Data.Model;
using Godwit.HandleLoginAction.Model;
using Godwit.HandleLoginAction.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Godwit.HandleLoginAction.Controllers {
    [ApiController]
    [Route("")]
    public class HasuraController : ControllerBase {
        private readonly IJwtService _jwtTokenHandler;
        private readonly IValidator<ActionData> _validator;
        private readonly UserManager<User> _manager;
        private readonly ILogger<HasuraController> _logger;
        public HasuraController(IValidator<ActionData> validator, IJwtService jwtTokenHandler,
            UserManager<User> manager, ILogger<HasuraController> logger) {
            _validator = validator;
            _jwtTokenHandler = jwtTokenHandler;
            _manager = manager;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ActionData model) {
            _logger.LogInformation($"Call Started by {model.Session.UserId} having role {model.Session.Role}");
            var validation = _validator.Validate(model);
            if (!validation.IsValid)
            {
                _logger.LogWarning("request validation failed!");
                return Ok(new
                {
                    Errors = validation.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            var user = await _manager.FindByNameAsync(model.Input.UserName)
                .ConfigureAwait(false);
            if (user != null &&
                _manager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Input.Password) ==
                PasswordVerificationResult.Success)
                return Ok(new {AccessToken = _jwtTokenHandler.GenerateToken(user), user.Id});
            _logger.LogWarning("Invalid Credentials");
            return Ok(new
            {
                Errors = new[] {"Invalid Credentials"}
            });

        }
    }
}
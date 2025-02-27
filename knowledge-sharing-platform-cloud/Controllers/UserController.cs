using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace knowledge_sharing_platform_cloud.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserRepo _userRepo;
        private readonly ILogger<UserController> _logger;

        public UserController(UserRepo userRepo, ILogger<UserController> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            try
            {
                var newUser = await _userRepo.CreateUserAsync(user);
                return CreatedAtAction(nameof(CreateUser), newUser);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.InnerException?.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException?.Message);
            }
        }
    }
}

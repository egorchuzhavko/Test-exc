using Microsoft.AspNetCore.Mvc;
using VebTechTest.Interfaces;
using VebTechTest.Models;

namespace VebTechTest.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository user) {
            this._userRepository = user;
        }

        //Get all users
        [HttpGet]
        [ProducesResponseType(200,Type = typeof(IEnumerable<User>))]
        public IActionResult GetUsers() {
            var users = _userRepository.GetUsers();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(users);
        }

        //Get user by id
        [HttpGet("{UserId}")]
        [ProducesResponseType(200,Type=typeof(User))]
        [ProducesResponseType(400)]
        public IActionResult GetUser(int id) {
            if (!_userRepository.UserExists(id))
                return NotFound();

            var user = _userRepository.GetUser(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(user);
        }

        //Create user
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateUser([FromBody] User user) {
            if (user == null)
                return BadRequest(ModelState);

            var checkuser = _userRepository.GetUsers().Where(u => u.Email == user.Email).FirstOrDefault();
            if (checkuser != null) {
                ModelState.AddModelError("", "User with that email already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_userRepository.CreateUser(user)) {
                ModelState.AddModelError("", "Something went wrong while we saved new user");
                return StatusCode(500, ModelState);
            }

            return Ok("User was created");
        }

        [HttpPut("{UserId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateUser(int userId, [FromBody] User user) {
            if (user == null)
                return BadRequest(ModelState);

            if (userId != user.Id)
                return BadRequest(ModelState);

            if (!_userRepository.UserExists(userId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_userRepository.UpdateUser(user)) {
                ModelState.AddModelError("", "Something went wrong while we updated user");
                return StatusCode(500, ModelState);
            }

            return Ok("User was updated");
        }

        [HttpDelete("userId")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteUser(int userId) {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var user = _userRepository.GetUser(userId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_userRepository.DeleteUser(user)) {
                ModelState.AddModelError("", "Something went wrong while we deleted user");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
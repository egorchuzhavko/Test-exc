using Microsoft.AspNetCore.Mvc;
using VebTechTest.Interfaces;
using VebTechTest.Models;
using VebTechTest.DTO;
using Microsoft.AspNetCore.Authorization;

namespace VebTechTest.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userroleRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository user, IRoleRepository role, IUserRoleRepository userrole, ILogger<UserController> logger) {
            this._userRepository = user;
            this._roleRepository = role;
            this._userroleRepository = userrole;
            this._logger = logger;
        }

        /// <summary>
        /// Get list users with pagination, filter and sort
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="sortby"></param>
        /// <param name="sorttype"></param>
        /// <param name="filterby"></param>
        /// <param name="filtertext"></param>
        /// <returns></returns>

        [HttpGet]
        [ProducesResponseType(200,Type = typeof(IEnumerable<GetUserDTO>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetUsers(int page = 1, int pagesize = 10, string? sortby =null, string? sorttype = null, 
            string? filterby = null, string? filtertext = null) {
            _logger.LogInformation("Get users executing..");

            var users = _userRepository.GetUsersDto();

            if (users.Count == 0) {
                _logger.LogInformation("Get users not found");
                return NotFound();
            }

            if (!ModelState.IsValid) {
                _logger.LogInformation("Get users bad request");
                return BadRequest(ModelState);
            }

            if ((sorttype != "asc" & sorttype != "desc" & sorttype != null)
                || (sortby != "id" & sortby != "name" & sortby != "age" & sortby != "email" & sortby != "role" & sortby != null)
                || (filterby != "id" & filterby != "name" & filterby != "age" & filterby != "email" & filterby != "role" & filterby != null)) {
                _logger.LogInformation("Get users bad request");
                return BadRequest();
            }

            if (sortby!=null)
                switch (sortby) {
                    case "id":
                        users = sorttype == "asc" ? users.OrderBy(x => x.Id).ToList() : users.OrderByDescending(x => x.Id).ToList();
                        break;
                    case "name":
                        users = sorttype == "asc" ? users.OrderBy(x => x.Name).ToList() : users.OrderByDescending(x => x.Name).ToList();
                        break;
                    case "age":
                        users = sorttype == "asc" ? users.OrderBy(x => x.Age).ToList() : users.OrderByDescending(x => x.Age).ToList();
                        break;
                    case "email":
                        users = sorttype == "asc" ? users.OrderBy(x => x.Email).ToList() : users.OrderByDescending(x => x.Email).ToList();
                        break;
                    case "role":
                        users = sorttype == "asc" ? users.OrderBy(x => x.Roles.Count).ToList() : users.OrderByDescending(x => x.Roles.Count).ToList();
                        break;
                    default:
                        break;
                }

            if(filterby!= null)
                switch (filterby) {
                    case "id":
                        users = users.Where(x => x.Id==Convert.ToInt32(filtertext)).ToList();
                        break;
                    case "name":
                        users = users.Where(x => x.Name.Contains(filtertext)).ToList();
                        break;
                    case "age":
                        users = users.Where(x => x.Age == Convert.ToInt32(filtertext)).ToList();
                        break;
                    case "email":
                        users = users.Where(x => x.Email.Contains(filtertext)).ToList();
                        break;
                    case "role":
                        users = users.Where(x => x.Roles.Contains(filtertext)).ToList();
                        break;
                    default:
                        break;
                }

            var query = users.AsQueryable();
            var usersresult = query.Skip((page - 1) * pagesize).Take(pagesize).ToList();

            _logger.LogInformation("Get users Successfull");
            return Ok(usersresult);
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        
        [HttpGet("{UserId}")]
        [ProducesResponseType(200,Type=typeof(GetUserDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUser(int UserId) {
            _logger.LogInformation("Get user by id executing..");
            if (!_userRepository.UserExists(UserId)) {
                _logger.LogInformation("Get user by id not found");
                return NotFound();
            }

            var user = _userRepository.GetUserDto(UserId);

            if (!ModelState.IsValid) {
                _logger.LogInformation("Get user by id bad request");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Get user Successfull");
            return Ok(user);
        }

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO user) {
            _logger.LogInformation("Create user executing..");
            if (user == null) {
                _logger.LogInformation("Create user bad request");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var checkuser = _userRepository.GetUsers().Where(u => u.Email == user.Email).FirstOrDefault();
            if (user.Name == "") {
                _logger.LogInformation("Create user empty name");
                ModelState.AddModelError("", "Empty name");
                return StatusCode(422, ModelState);
            }

            if (user.Age <= 0) {
                _logger.LogInformation("Create user wrong age");
                ModelState.AddModelError("", "Wrong age");
                return StatusCode(422, ModelState);
            }

            if (user.Email == "") {
                _logger.LogInformation("Create user empty email");
                ModelState.AddModelError("", "Empty email");
                return StatusCode(422, ModelState);
            }

            if (checkuser != null) {
                _logger.LogInformation("Create user user exists");
                ModelState.AddModelError("", "User with that email already exists");
                return StatusCode(422, ModelState);
            }

            var newuser = new User {
                Name = user.Name,
                Email = user.Email,
                Age = user.Age,
                UserRoles = new List<UserRole>()
            };

            if (!_userRepository.CreateUser(newuser)) {
                _logger.LogInformation("Create user create user error");
                ModelState.AddModelError("", "Something went wrong while we saved new user");
                return StatusCode(500, ModelState);
            }

            var createduser = _userRepository.GetUser(user.Email);

            if (!_userroleRepository.AddRole(new UserRole { UserId = createduser.Id, RoleId = 1 })) {
                _logger.LogInformation("Create user create userrole error server error");
                ModelState.AddModelError("", "Something went wrong while we added new role to created user");
                return StatusCode(500, ModelState);
            }

            _logger.LogInformation("Create user Successfull");
            return Ok("User was created");
        }

        /// <summary>
        /// update user info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <returns></returns>

        [HttpPut("{UserId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserDTO user) {
            _logger.LogInformation("Update user executing...");
            if (userId! > 0) {
                _logger.LogInformation("Update user not found");
                ModelState.AddModelError("", "User not found");
                return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid) {
                _logger.LogInformation("Update user bad request");
                return BadRequest(ModelState);
            }

            if (user == null) {
                _logger.LogInformation("Update user bad request");
                return BadRequest(ModelState);
            }

            if (user.Name == "") {
                _logger.LogInformation("Update user empty name");
                ModelState.AddModelError("", "Empty name");
                return StatusCode(422, ModelState);
            }

            if (user.Age <= 0) {
                _logger.LogInformation("Update user wrong age");
                ModelState.AddModelError("", "Wrong age");
                return StatusCode(422, ModelState);
            }

            if (user.Email == "") {
                _logger.LogInformation("Update user empty email");
                ModelState.AddModelError("", "Empty email");
                return StatusCode(422, ModelState);
            }

            var baseuser = _userRepository.GetUser(userId);
            if(baseuser != null) {
                _logger.LogInformation("Update user bad request");
                return BadRequest(ModelState);
            }

            if(user.Email != baseuser.Email) {
                if(_userRepository.GetUsers().Where(u => u.Email == user.Email).FirstOrDefault()!=null) {
                    _logger.LogInformation("Update user email already exists");
                    ModelState.AddModelError("", "User with that email already exists");
                    return StatusCode(422, ModelState);
                }
            }

            baseuser.Email = user.Email;
            baseuser.Age = user.Age;
            baseuser.Name = user.Name;
            if (!_userRepository.UpdateUser(baseuser)) {
                _logger.LogInformation("Update user server error");
                ModelState.AddModelError("", "Something went wrong while we updated user");
                return StatusCode(500, ModelState);
            }

            _logger.LogInformation("Update user Successfull");
            return Ok("User was updated");
        }

        /// <summary>
        /// Add user role
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>

        [HttpPost("{UserId}/roles")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AddUserRole(int UserId, int roleId) {
            _logger.LogInformation("Add user role executing..");
            if (!_userRepository.UserExists(UserId)) {
                _logger.LogInformation("Add user role user not found");
                return NotFound();
            }

            if (!_roleRepository.RoleExists(roleId)) {
                _logger.LogInformation("Add user role role not found");
                return NotFound();
            }

            var checkuserrole = _userroleRepository.GetUsersRoles().Where(ur => ur.UserId == UserId & ur.RoleId == roleId).FirstOrDefault();

            if (checkuserrole != null) {
                _logger.LogInformation("Add user role user already has this role");
                ModelState.AddModelError("", "User already has this role");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) {
                _logger.LogInformation("Add user role bad request");
                return BadRequest(ModelState);
            }

            if (!_userroleRepository.AddRole(new UserRole { UserId = UserId, RoleId = roleId })) {
                _logger.LogInformation("Add user role server error");
                ModelState.AddModelError("", "Something went wrong while we added new role to user");
                return StatusCode(500, ModelState);
            }

            _logger.LogInformation("Add user role Successfull");
            return Ok("User got new role");
        }

        /// <summary>
        /// Delete user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        
        [HttpDelete("userId")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteUser(int userId) {
            _logger.LogInformation("Delete user executing..");
            if (!_userRepository.UserExists(userId)) {
                _logger.LogInformation("Delete user user not found");
                return NotFound();
            }

            var user = _userRepository.GetUser(userId);

            if (!ModelState.IsValid) {
                _logger.LogInformation("Delete user bad request");
                return BadRequest(ModelState);
            }

            if (!_userRepository.DeleteUser(user)) {
                _logger.LogInformation("Delete user server error");
                ModelState.AddModelError("", "Something went wrong while we deleted user");
                return StatusCode(500, ModelState);
            }

            _logger.LogInformation("Delete user Successfull");
            return NoContent();
        }
    }
}
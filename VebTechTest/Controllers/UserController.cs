﻿using Microsoft.AspNetCore.Mvc;
using VebTechTest.Interfaces;
using VebTechTest.Models;
using VebTechTest.DTO;

namespace VebTechTest.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userroleRepository;

        public UserController(IUserRepository user, IRoleRepository role, IUserRoleRepository userrole) {
            this._userRepository = user;
            this._roleRepository = role;
            this._userroleRepository = userrole;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200,Type = typeof(IEnumerable<GetUserDTO>))]
        public IActionResult GetUsers(int page = 1, int pagesize = 10, string? sortby =null, string? sorttype = null, 
            string? filterby = null, string? filtertext = null) {
            var users = _userRepository.GetUsersDto();

            if (users.Count == 0)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if ((sorttype != "asc" & sorttype != "desc" & sorttype != null)
                || (sortby != "id" & sortby != "name" & sortby != "age" & sortby != "email" & sortby != "role" & sortby != null)
                || (filterby != "id" & filterby != "name" & filterby != "age" & filterby != "email" & filterby != "role" & filterby != null))
                return BadRequest();

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

            return Ok(usersresult);
        }

        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{UserId}")]
        [ProducesResponseType(200,Type=typeof(GetUserDTO))]
        [ProducesResponseType(400)]
        public IActionResult GetUser(int UserId) {
            if (!_userRepository.UserExists(UserId))
                return NotFound();

            var user = _userRepository.GetUserDto(UserId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(user);
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateUser([FromBody] UserDTO user) {
            if (user == null)
                return BadRequest(ModelState);

            var checkuser = _userRepository.GetUsers().Where(u => u.Email == user.Email).FirstOrDefault();
            if (user.Name == "") {
                ModelState.AddModelError("", "Empty name");
                return StatusCode(422, ModelState);
            }

            if (user.Age <= 0) {
                ModelState.AddModelError("", "Wrong age");
                return StatusCode(422, ModelState);
            }

            if (user.Email == "") {
                ModelState.AddModelError("", "Empty email");
                return StatusCode(422, ModelState);
            }

            if (checkuser != null) {
                ModelState.AddModelError("", "User with that email already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newuser = new User {
                Name = user.Name,
                Email = user.Email,
                Age = user.Age,
                UserRoles = new List<UserRole>()
            };

            if (!_userRepository.CreateUser(newuser)) {
                ModelState.AddModelError("", "Something went wrong while we saved new user");
                return StatusCode(500, ModelState);
            }

            var createduser = _userRepository.GetUser(user.Email);

            if (!_userroleRepository.AddRole(new UserRole { UserId = createduser.Id, RoleId = 1 })) {
                ModelState.AddModelError("", "Something went wrong while we added new role to created user");
                return StatusCode(500, ModelState);
            }

            return Ok("User was created");
        }

        /// <summary>
        /// Update user by Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("{UserId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateUser(int userId, [FromBody] UserDTO user) {
            if (userId! > 0) {
                ModelState.AddModelError("", "Wrong user id");
                return StatusCode(400, ModelState);
            }

            if (user == null)
                return BadRequest(ModelState);

            if (user.Name == "") {
                ModelState.AddModelError("", "Empty name");
                return StatusCode(422, ModelState);
            }

            if (user.Age <= 0) {
                ModelState.AddModelError("", "Wrong age");
                return StatusCode(422, ModelState);
            }

            if (user.Email == "") {
                ModelState.AddModelError("", "Empty email");
                return StatusCode(422, ModelState);
            }

            var baseuser = _userRepository.GetUser(userId);
            if(baseuser != null) {
                return BadRequest(ModelState);
            }

            if(user.Email != baseuser.Email) {
                if(_userRepository.GetUsers().Where(u => u.Email == user.Email).FirstOrDefault()!=null) {
                    ModelState.AddModelError("", "User with that email already exists");
                    return StatusCode(422, ModelState);
                }
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            baseuser.Email = user.Email;
            baseuser.Age = user.Age;
            baseuser.Name = user.Name;
            if (!_userRepository.UpdateUser(baseuser)) {
                ModelState.AddModelError("", "Something went wrong while we updated user");
                return StatusCode(500, ModelState);
            }

            return Ok("User was updated");
        }

        /// <summary>
        /// Add new role to user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost("{UserId}/roles")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult AddUserRole(int UserId, int roleId) {
            if (!_userRepository.UserExists(UserId))
                return NotFound();

            if (!_roleRepository.RoleExists(roleId))
                return NotFound();

            var checkuserrole = _userroleRepository.GetUsersRoles().Where(ur => ur.UserId == UserId & ur.RoleId == roleId).FirstOrDefault();

            if (checkuserrole != null) {
                ModelState.AddModelError("", "User already has this role");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_userroleRepository.AddRole(new UserRole { UserId = UserId, RoleId = roleId })) {
                ModelState.AddModelError("", "Something went wrong while we added new role to user");
                return StatusCode(500, ModelState);
            }

            return Ok("User got new role");
        }

        /// <summary>
        /// Delete user by Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
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
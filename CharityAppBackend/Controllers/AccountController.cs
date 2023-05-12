using CharityAppBL.Login;
using CharityAppBL.Users;
using CharityAppBO.Users;
using Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Controllers
{
    [Route("user")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {

        private readonly IBLAccount _bLUser;


        public AccountController(IBLAccount bLUser)
        {
            _bLUser = bLUser;
        }


        /// <summary>
        /// Get a specific User with ID.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get http://localhost:8080/user/current-user
        ///     {
        ///        
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns user with specific id</response>
        /// <response code="403">User does not have permission</response>
        /// <response code="400">If the user is null</response>
        [HttpGet("current-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetCurrentUser()
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = _bLUser.GetUser(id);
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// Get all users(only Admin role).
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get http://localhost:8080/user/all
        ///     {
        ///        
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns all users</response>
        /// <response code="400">If no user</response>
        /// <response code="403">User does not have permission</response>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetAllUser()
        {
            var result = _bLUser.GetAllUser();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// Change status User (lock or not).
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     Post http://localhost:8080/user/change-status
        ///     {
        ///        id: 1,
        ///        status: true
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns 1</response>
        /// <response code="400">If can not update</response>
        /// <response code="403">User does not have permission</response>
        [HttpPost("change-status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult ChangeStatus(UpdateStatusUser updateStatusUser)
        {
            var result = _bLUser.ChangeStatus(updateStatusUser);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            //using IDbConnection db = new MySqlConnection(_connectionString);

            //var existingUser = await db.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = id });

            //if (existingUser == null)
            //{
            //    return NotFound();
            //}

            //var sql = @"UPDATE Users SET Name = @Name, Email = @Email, Password = @Password WHERE Id = @Id;";

            //await db.ExecuteAsync(sql, new { Id = id, Name = user.Name, Email = user.Email, Password = user.Password });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            //using IDbConnection db = new MySqlConnection(_connectionString);

            //var existingUser = await db.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = id });

            //if (existingUser == null)
            //{
            //    return NotFound();
            //}

            //var sql = @"DELETE FROM Users WHERE Id = @Id;";

            //await db.ExecuteAsync(sql, new { Id = id });

            return NoContent();
        }


        private User? GetCurrentUser1()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;
                return new User
                {
                    UserName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    Id = int.Parse(userClaims.FirstOrDefault(o => o.Type == "id")?.Value),
                    RoleName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value
                };
            }
            return null;
        }
    }
}

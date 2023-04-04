using CharityAppBL.Login;
using Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {

        private readonly IBLLogin _iBLLogin;

        public UserController(IBLLogin bLLogin)
        {
            _iBLLogin = bLLogin;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            //using IDbConnection db = new MySqlConnection(_connectionString);
            //var user = await db.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Id = @Id", new { Id = id });

            //if (user == null)
            //{
            //    return NotFound();
            //}
            //var user = GetCurrentUser();
            return Ok("Aioiqwf");
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            //using IDbConnection db = new MySqlConnection(_connectionString);
            //var sql = @"INSERT INTO Users (Name, Email, Password) VALUES (@Name, @Email, @Password);
            //            SELECT LAST_INSERT_ID();";

            //var id = await db.ExecuteScalarAsync<int>(sql, user);

            //user.Id = id;

            return CreatedAtAction("oke", 1);
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


        private User? GetCurrentUser()
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

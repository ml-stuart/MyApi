using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet("sql-connection")]
        public IActionResult GetSqlConnection()
        {
            var connString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                using var cmd = new NpgsqlCommand("SELECT NOW()", conn);
                var result = cmd.ExecuteScalar();

                return Ok(new { DatabaseTime = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }

        }
        [HttpGet("outer-connection")]
        public async Task<IActionResult> GetOuterConnection() 
        {
            var url = "https://httpbin.org/get";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            string result = await response.Content.ReadAsStringAsync();

            return Ok(result);
        }
    }
}

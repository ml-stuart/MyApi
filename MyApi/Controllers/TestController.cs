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

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("請選擇檔案");

            var allowedExtensions = new[] { ".xlsx", ".xls", ".docx", ".doc" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension)) return BadRequest("不支援的格式");

            // ---------------------------------------------------------
            // 關鍵修改：從設定檔讀取路徑
            // 如果設定檔沒寫，預設還是放在 App 目錄下的 Uploads (方便本機開發)
            // ---------------------------------------------------------
            var configuredPath = _configuration["FileStoragePath"];
            var uploadPath = string.IsNullOrEmpty(configuredPath)
                ? Path.Combine(Directory.GetCurrentDirectory(), "Uploads")
                : configuredPath;

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var trustedFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadPath, trustedFileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"存檔失敗: {ex.Message}");
            }

            return Ok(new { Message = "上傳成功", Path = filePath });
        }
    }
}

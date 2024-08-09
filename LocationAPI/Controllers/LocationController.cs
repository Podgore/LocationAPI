using LocationAPI.BLL.Interface;
using LocationAPI.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace LocationAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly IDataService _dataService;
        public LocationController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ReadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var result = await _dataService.ReadCSVFileAsync(file);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetDataByRange(DateTime dateStart, DateTime dateEnd)
        {
            var result = await _dataService.GetTransactionBetweenTwoDatesAsync(dateStart, dateEnd);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetDataByRangeInCurrentOffset(DateTime dateStart, DateTime dateEnd)
        {
            var result = await _dataService.GetTransactionBetweenTwoDatesInUsersOffsetAsync(dateStart, dateEnd);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetJanuaryData()
        {
            var result = await _dataService.GetTransactionForJanuaryAsync();
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateExcel()
        {
            await _dataService.ExportAllDataToExcelAsync();
            return Ok();
        }

    }
}

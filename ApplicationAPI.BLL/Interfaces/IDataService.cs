using LocationAPI.DAL.Models;
using Microsoft.AspNetCore.Http;

namespace LocationAPI.BLL.Interface
{
    public interface IDataService
    {
        public Task<List<Transaction>> ReadCSVFileAsync(IFormFile file);
        public Task<List<Transaction>> GetTransactionBetweenTwoDatesAsync(DateTime dateStart, DateTime dateEnd);
        public Task<List<Transaction>> GetTransactionBetweenTwoDatesInUsersOffsetAsync(DateTime dateStart, DateTime dateEnd);
        public Task<List<Transaction>> GetTransactionForJanuaryAsync();
        public Task ExportAllDataToExcelAsync();
    }
}

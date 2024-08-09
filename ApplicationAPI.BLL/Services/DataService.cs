using GeoTimeZone;
using LocationAPI.BLL.Interface;
using LocationAPI.DAL.DBContext;
using LocationAPI.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TimeZoneConverter;
using Microsoft.AspNetCore.Http;
using System.Text;
using OfficeOpenXml;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;


namespace LocationAPI.BLL.Services
{
    public class DataService : IDataService
    {
        private readonly ApplicationDbContext _context;

        public DataService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> ReadCSVFileAsync(IFormFile file)
        {
            using (var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            {
                List<Transaction> transactions = new List<Transaction>();

                var isFirstRow = true;

                while (!reader.EndOfStream)
                {
                    var content = reader.ReadLine();
                    if (isFirstRow)
                    {
                        isFirstRow = false;
                        continue;
                    }
                    var props = content.Split(',').ToList();

                    var tempLatitude = props[5].Replace("\"", "").Split(',')[0].Trim();
                    var tempLonger = props[6].Replace("\"", "").Split(',')[0].Trim();

                    var culture = new CultureInfo("en-US");

                    double latitude = double.Parse(tempLatitude, culture);

                    double longitude = double.Parse(tempLonger, culture);

                    var date = DateTime.ParseExact(props[4], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                    string timeZoneId = TimeZoneLookup.GetTimeZone(latitude, longitude).Result;

                    var timeZone = TZConvert.GetTimeZoneInfo(timeZoneId);

                    var dateTimeOffset = new DateTimeOffset(date, timeZone.BaseUtcOffset);

                    var amount = decimal.Parse(props[3].Replace("$", "").Trim(), culture);

                    var entity = new Transaction
                    {
                        Id = props[0],
                        FullName = props[1],
                        Email = props[2],
                        Amount = amount,
                        Date = dateTimeOffset,
                    };

                    var existingEntity = transactions.FirstOrDefault(t => t.Id == entity.Id);

                    if (existingEntity == null)
                    {
                        transactions.Add(entity);
                        continue;
                    }
                    else
                    {
                        existingEntity.FullName = entity.FullName;
                        existingEntity.Email = entity.Email;
                        existingEntity.Amount = entity.Amount;
                        existingEntity.Date = entity.Date;
                    }
                }
                using (IDbConnection db = new SqlConnection(_context.Database.GetConnectionString()))
                {
                    var sql = @"INSERT INTO Transactions (Id, FullName, Email, Amount, Date) 
                                VALUES (@Id, @FullName, @Email, @Amount, @Date)";
                    await db.ExecuteAsync(sql, transactions);
                }
                return transactions;
            }
        }

        public async Task<List<Transaction>> GetTransactionBetweenTwoDatesAsync(DateTime dateStart, DateTime dateEnd)
        {

            var dateStartOffSet = new DateTimeOffset(DateTime.SpecifyKind(dateStart, DateTimeKind.Unspecified));

            var dateEndOffSet = new DateTimeOffset(DateTime.SpecifyKind(dateEnd, DateTimeKind.Unspecified));

            using (IDbConnection db = new SqlConnection(_context.Database.GetConnectionString()))
            {
                var sql = @"SELECT * FROM Transactions WHERE Date >= @dateStartOffSet AND Date <= @dateEndOffSet";
                var transactions = await db.QueryAsync<Transaction>(sql, new { dateStartOffSet, dateEndOffSet });
                return transactions.ToList();
            }
           
        }

        public async Task<List<Transaction>> GetTransactionBetweenTwoDatesInUsersOffsetAsync(DateTime dateStart, DateTime dateEnd)
        {
            var transactions = await GetTransactionBetweenTwoDatesAsync(dateStart, dateEnd);

            var filteredTransaction = transactions.Where(t => t.Date.Offset == DateTimeOffset.Now.Offset).ToList();

            return filteredTransaction;
        }

        public async Task<List<Transaction>> GetTransactionForJanuaryAsync()
        {
            var beginningOfJanuary = new DateTime(2024, 1, 1, 0, 0, 0);

            var endOfJanuary = new DateTime(2024, 1, 31, 23, 59, 59, 999);

            var transactions = await GetTransactionBetweenTwoDatesAsync(beginningOfJanuary, endOfJanuary);

            return transactions;
        }

        public async Task ExportAllDataToExcelAsync()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (IDbConnection db = new SqlConnection(_context.Database.GetConnectionString()))
            {
                var transactions = await db.QueryAsync<Transaction>("SELECT * FROM Transactions");

                var projectDirectory = Directory.GetCurrentDirectory();
                var filePath = Path.Combine(projectDirectory, "Transactions.xlsx");

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Transactions");

                    worksheet.Cells[1, 1].Value = "FullName";
                    worksheet.Cells[1, 2].Value = "Email";
                    worksheet.Cells[1, 3].Value = "Date";
                    worksheet.Cells[1, 4].Value = "Amount";

                    var transactionList = transactions.ToList();
                    for (int i = 0; i < transactionList.Count; i++)
                    {
                        var transaction = transactionList[i];
                        worksheet.Cells[i + 2, 1].Value = transaction.FullName;
                        worksheet.Cells[i + 2, 2].Value = transaction.Email;
                        worksheet.Cells[i + 2, 3].Value = transaction.Date;
                        worksheet.Cells[i + 2, 4].Value = transaction.Amount;
                    }

                    var fileInfo = new FileInfo(filePath);
                    await package.SaveAsAsync(fileInfo);
                }
            }
        }
    }
}

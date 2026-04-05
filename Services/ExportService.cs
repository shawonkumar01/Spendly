using CsvHelper;
using CsvHelper.Configuration;
using Spendly.Models;
using System.Globalization;

namespace Spendly.Services
{
    public class ExportService
    {
        public byte[] ExportExpensesToCsv(IEnumerable<Expense> expenses)
        {
            var records = expenses.Select(e => new
            {
                Title = e.Title,
                Amount = e.Amount,
                Category = e.Category.Name,
                Date = e.Date.ToString("dd MMM yyyy"),
                Notes = e.Notes ?? ""
            });

            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms);
            using var csv = new CsvWriter(writer, new CsvConfiguration(
                CultureInfo.InvariantCulture));

            csv.WriteRecords(records);
            writer.Flush();
            return ms.ToArray();
        }
    }
}
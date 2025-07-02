using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using RazorPagesMovie.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Movies
{
    public class ExportModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public ExportModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(
            string? SearchString,
            string? MovieGenre,
            int PageIndex = 1,
            string exportMode = "all", // "all" or "page"
            int PageSize = 5,
            string? SortField = "Title",
            string? SortOrder = "asc"
        )
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Movies");

            var query = _context.Movie.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(SearchString))
                query = query.Where(m => EF.Functions.Like(m.Title, $"%{SearchString}%"));

            if (!string.IsNullOrEmpty(MovieGenre))
                query = query.Where(m => m.Genre == MovieGenre);

            // Apply sorting
            query = SortField switch
            {
                "Title" => SortOrder == "asc" ? query.OrderBy(m => m.Title) : query.OrderByDescending(m => m.Title),
                "Genre" => SortOrder == "asc" ? query.OrderBy(m => m.Genre) : query.OrderByDescending(m => m.Genre),
                "ReleaseDate" => SortOrder == "asc" ? query.OrderBy(m => m.ReleaseDate) : query.OrderByDescending(m => m.ReleaseDate),
                "Price" => SortOrder == "asc" ? query.OrderBy(m => (double)m.Price) : query.OrderByDescending(m => (double)m.Price),
                "Rating" => SortOrder == "asc" ? query.OrderBy(m => m.Rating) : query.OrderByDescending(m => m.Rating),
                _ => query.OrderBy(m => m.Title)
            };

            // Apply pagination if mode is 'page'
            if (exportMode == "page")
            {
                query = query
                    .Skip((PageIndex - 1) * PageSize)
                    .Take(PageSize);
            }

            // Execute query
            var movies = await query.ToListAsync();

            // Header row
            worksheet.Cells[1, 1].Value = "Title";
            worksheet.Cells[1, 2].Value = "Release Date";
            worksheet.Cells[1, 3].Value = "Genre";
            worksheet.Cells[1, 4].Value = "Price";
            worksheet.Cells[1, 5].Value = "Rating";

            // Data rows
            for (int i = 0; i < movies.Count; i++)
            {
                var movie = movies[i];
                worksheet.Cells[i + 2, 1].Value = movie.Title;
                worksheet.Cells[i + 2, 2].Value = movie.ReleaseDate.ToString("yyyy-MM-dd");
                worksheet.Cells[i + 2, 3].Value = movie.Genre;
                worksheet.Cells[i + 2, 4].Value = (double)movie.Price;
                worksheet.Cells[i + 2, 5].Value = movie.Rating;
            }

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Stream back file
            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Movies.xlsx");
        }
    }
}

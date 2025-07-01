using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using RazorPagesMovie.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;

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
            int PageSize = 5
        )
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Movies");
            
            var query = _context.Movie.AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
                query = query.Where(m => m.Title.Contains(SearchString));

            if (!string.IsNullOrEmpty(MovieGenre))
                query = query.Where(m => m.Genre == MovieGenre);

            // Calculate total filtered count
            var totalCount = query.Count();

            // Calculate total pages based on PageSize
            var totalPages = (int)System.Math.Ceiling(totalCount / (double)PageSize);

            if (exportMode == "page")
            {
                // Export only the current page
                query = query
                    .OrderBy(m => m.Title)
                    .Skip((PageIndex - 1) * PageSize)
                    .Take(PageSize);
            }
            else if (exportMode == "all")
            {
                // Export all filtered movies up to total pages (i.e. all pages currently available)
                // So take totalPages * PageSize but not more than totalCount
                int takeCount = totalPages * PageSize;
                if (takeCount > totalCount)
                    takeCount = totalCount;

                query = query
                    .OrderBy(m => m.Title)
                    .Take(takeCount);
            }

            var movies = query.ToList();


            worksheet.Cells[1, 1].Value = "Title";
            worksheet.Cells[1, 2].Value = "Release Date";
            worksheet.Cells[1, 3].Value = "Genre";
            worksheet.Cells[1, 4].Value = "Price";
            worksheet.Cells[1, 5].Value = "Rating";

            for (int i = 0; i < movies.Count; i++)
            {
                var movie = movies[i];
                worksheet.Cells[i + 2, 1].Value = movie.Title;
                worksheet.Cells[i + 2, 2].Value = movie.ReleaseDate.ToShortDateString();
                worksheet.Cells[i + 2, 3].Value = movie.Genre;
                worksheet.Cells[i + 2, 4].Value = movie.Price;
                worksheet.Cells[i + 2, 5].Value = movie.Rating;
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream, 
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                        "Movies.xlsx");
        }
    }
}

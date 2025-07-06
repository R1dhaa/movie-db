using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;


namespace RazorPagesMovie.Pages_Movies
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public IndexModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IList<Movie> Movie { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public SelectList? Genres { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? MovieGenre { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;  // current page number, default to 1
        
        [BindProperty(SupportsGet = true)]
        public string? SortField { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortOrder { get; set; } = "asc"; // Default to ascending

        public int TotalPages { get; set; }
        private const int PageSize = 5;  // items per page

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        public async Task OnGetAsync()
        {
            IQueryable<string> genreQuery = from m in _context.Movie
                                            orderby m.Genre
                                            select m.Genre;

            var movies = from m in _context.Movie
            select m;
            movies = SortField switch
            {
                "Title" => SortOrder == "asc" ? movies.OrderBy(m => m.Title) : movies.OrderByDescending(m => m.Title),
                "Genre" => SortOrder == "asc" ? movies.OrderBy(m => m.Genre) : movies.OrderByDescending(m => m.Genre),
                "ReleaseDate" => SortOrder == "asc" ? movies.OrderBy(m => m.ReleaseDate) : movies.OrderByDescending(m => m.ReleaseDate),
                "Price" => SortOrder == "asc"
                    ? movies.OrderBy(m => (double)m.Price)
                    : movies.OrderByDescending(m => (double)m.Price),
                "Rating" => SortOrder == "asc" ? movies.OrderBy(m => m.Rating) : movies.OrderByDescending(m => m.Rating),
                _ => movies.OrderBy(m => m.Title) // Default sort
            };

            if (!string.IsNullOrEmpty(SearchString))
            {
                movies = movies.Where(s => EF.Functions.Like(s.Title, $"%{SearchString}%"));
            }

            if (!string.IsNullOrEmpty(MovieGenre))
            {
                movies = movies.Where(x => x.Genre == MovieGenre);
            }
            Genres = new SelectList(await genreQuery.Distinct().ToListAsync());
            Movie = await movies.ToListAsync();
            int count = await movies.CountAsync();

            TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            if (PageIndex < 1) PageIndex = 1;
            if (PageIndex > TotalPages) PageIndex = TotalPages > 0 ? TotalPages : 1;

            Movie = await movies
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}



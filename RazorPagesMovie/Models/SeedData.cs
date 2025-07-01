using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;

namespace RazorPagesMovie.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
       using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<RazorPagesMovieContext>();

        {
            if (context == null || context.Movie == null)
            {
                throw new ArgumentNullException("Null RazorPagesMovieContext");
            }

            // Look for any movies.
            if (context.Movie.Any())
            {
                return;   // DB has been seeded
            }

            context.Movie.AddRange(
                new Movie
                {
                    Title = "When Harry Met Sally",
                    ReleaseDate = DateTime.Parse("1989-2-12"),
                    Genre = "Romantic Comedy",
                    Price = 7.99M,
                    Rating = "7.5"
                },

                new Movie
                {
                    Title = "Ghostbusters ",
                    ReleaseDate = DateTime.Parse("1984-3-13"),
                    Genre = "Comedy",
                    Price = 8.99M,
                    Rating = "7.8"
                },

                new Movie
                {
                    Title = "Ghostbusters 2",
                    ReleaseDate = DateTime.Parse("1986-2-23"),
                    Genre = "Comedy",
                    Price = 9.99M,
                    Rating = "6.5"
                },

                new Movie
                {
                    Title = "Rio Bravo",
                    ReleaseDate = DateTime.Parse("1959-4-15"),
                    Genre = "Western",
                    Price = 3.99M,
                    Rating = "7.3"
                },
            
                new Movie { Title = "La La Land", ReleaseDate = DateTime.Parse("2016-12-09"), Genre = "Musical", Price = 10.00M, Rating = "8.0" },
                new Movie { Title = "Inception", ReleaseDate = DateTime.Parse("2010-07-16"), Genre = "Sci-Fi", Price = 13.00M, Rating = "8.8" },
                new Movie { Title = "The Godfather", ReleaseDate = DateTime.Parse("1972-03-24"), Genre = "Crime", Price = 11.00M, Rating = "9.2" },
                new Movie { Title = "Toy Story", ReleaseDate = DateTime.Parse("1995-11-22"), Genre = "Animation", Price = 9.00M, Rating = "8.3" },
                new Movie { Title = "Avengers: Endgame", ReleaseDate = DateTime.Parse("2019-04-26"), Genre = "Superhero", Price = 14.00M, Rating = "8.4" },
                new Movie { Title = "Parasite", ReleaseDate = DateTime.Parse("2019-05-30"), Genre = "Thriller", Price = 10.50M, Rating = "8.6" },
                new Movie { Title = "The Notebook", ReleaseDate = DateTime.Parse("2004-06-25"), Genre = "Romance", Price = 8.00M, Rating = "7.9" },
                new Movie { Title = "Spirited Away", ReleaseDate = DateTime.Parse("2001-07-20"), Genre = "Fantasy", Price = 9.50M, Rating = "8.6" }
            );

            context.SaveChanges();
        }
    }
}
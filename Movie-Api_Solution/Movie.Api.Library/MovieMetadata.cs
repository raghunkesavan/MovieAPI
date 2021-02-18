using System.Collections.Generic;

namespace MovieApiLibrary
{
    public class MovieMetadata : IMovieMetadata
    {
       private List<Movie> lstdatabase = new List<Movie>();
        public List<Movie> AddMovieMetaData(Movie movie)
        {
            lstdatabase.Add(movie);
            return lstdatabase;
        }

    }
}

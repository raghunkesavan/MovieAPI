using System.Collections.Generic;

namespace MovieApiLibrary
{
    public interface IMovieMetadata
    {
        List<Movie> AddMovieMetaData(Movie movie);
    }
}
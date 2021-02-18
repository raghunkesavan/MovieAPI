using System.Collections.Generic;

namespace MovieApiLibrary
{
    public interface ICsvMetaDataFileRead
    {
        List<Movie> CsvMovieMetadataRead();
        List<Movie> LookupMoviesById(int movieid);
    }
}
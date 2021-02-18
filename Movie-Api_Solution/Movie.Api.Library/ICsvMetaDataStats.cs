using System.Collections.Generic;

namespace MovieApiLibrary
{
    public interface ICsvMetaDataStats
    {
        List<MovieStatistics> CsvMovieStatsRead();
        
    }
}
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;

namespace MovieApiLibrary
{
    public class CsvMetaDataStats : ICsvMetaDataStats
    {
        private readonly ICsvMetaDataFileRead _CsvFileRead = null;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CsvMetaDataStats> _logger;
        public CsvMetaDataStats(ICsvMetaDataFileRead csvFileRead, IMemoryCache memoryCache, ILogger<CsvMetaDataStats> logger)
        {
            _CsvFileRead = csvFileRead;
            _cache = memoryCache;
            _logger = logger;
        }

        public List<MovieStatistics> CsvMovieStatsRead()
        {
            try
            {

                if (!_cache.TryGetValue("stats", out List<MovieStatistics> Moviesstatistics))
                {
                    Moviesstatistics = LookupMoviesStats();
                    _cache.Set("stats", Moviesstatistics, TimeSpan.FromSeconds(100));
                }
                else
                {
                    Console.WriteLine("Cache hit");
                }

                return Moviesstatistics;
            }

            catch (WebException wex)
            {
                _logger.LogError(wex.Message, wex);

                throw;
            }
            catch (Exception wex)
            {
                WebException webSourceException = new WebException("An error occurred while fetching movie stats.", wex);

                _logger.LogError(webSourceException.Message, wex);

                throw webSourceException;
            }


        }

        private List<MovieStatistics> LookupMoviesStats()
        {
            List<MovieWatchDuration> lstmovieWatchDration = new List<MovieWatchDuration>();

            var metadatastatfilename = Directory.GetCurrentDirectory() + @"\stats.csv";

            using (TextFieldParser csvReader = new TextFieldParser(metadatastatfilename))
            {
                DataTable csvData = new DataTable();
                csvReader.SetDelimiters(new string[] { "," });
                csvReader.HasFieldsEnclosedInQuotes = true;
                string[] colFields = csvReader.ReadFields();
                foreach (string column in colFields)
                {
                    DataColumn datecolumn = new DataColumn(column);
                    datecolumn.AllowDBNull = true;
                    csvData.Columns.Add(datecolumn);
                }

                while (!csvReader.EndOfData)
                {
                    string[] fieldData = csvReader.ReadFields();
                    for (int i = 0; i < fieldData.Length; i++)
                    {
                        if (fieldData[i] == "")
                        {
                            fieldData[i] = null;
                        }
                    }
                    lstmovieWatchDration.Add(new MovieWatchDuration
                    {
                        MovieId = Convert.ToInt32(fieldData[0]),
                        watchDurationMs = Convert.ToInt64(fieldData[1])
                    });

                }
            }

            var result = lstmovieWatchDration
                .GroupBy(x => x.MovieId)
                .Select(g => new
                {
                    Movieid = g.Key,
                    watches = g.Sum(x => x.watchDurationMs),
                    averageWatchDurationS = g.Average(x => x.watchDurationMs)
                });

            List<MovieStatistics> lstmovieStatistics = new List<MovieStatistics>();
            List<Movie> movies  = _CsvFileRead.CsvMovieMetadataRead();

            foreach (var value in result)
            {
                lstmovieStatistics.Add(new MovieStatistics
                {
                    MovieId = value.Movieid,
                    Title = movies.Where(h => h.MovieId == value.Movieid).Select(j => j.Title).FirstOrDefault(),
                    averageWatchDurationS = Convert.ToInt64(value.averageWatchDurationS),
                    watches = value.watches,
                    ReleaseYear = movies.Where(m => m.MovieId == value.Movieid).Select(r => r.ReleaseYear).FirstOrDefault()
                });
            }
            lstmovieStatistics.OrderByDescending(o => o.watches).ThenByDescending(r => r.ReleaseYear);
            return lstmovieStatistics;
        }

    }
}

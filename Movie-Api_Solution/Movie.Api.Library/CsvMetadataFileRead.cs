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
    public class CsvMetadataFileRead : ICsvMetaDataFileRead
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CsvMetadataFileRead> _logger;
        public CsvMetadataFileRead(IMemoryCache memoryCache, ILogger<CsvMetadataFileRead> logger)
        {
            _cache = memoryCache;
            _logger = logger;
        }

        public List<Movie> LookupMoviesById(int movieid)
        {
            try
            {
                if (!_cache.TryGetValue(movieid, out List<Movie> Movies))
                {
                    Movies = MoviesFilter(CsvMovieMetadataRead(), movieid);
                    _cache.Set(movieid, Movies, TimeSpan.FromSeconds(100));
                }
                else
                {
                    Console.WriteLine("Cache hit");
                }

                return Movies;
            }
            catch (WebException wex)
            {
                _logger.LogError(wex.Message, wex);

                throw;
            }
            catch (Exception wex)
            {
                WebException webSourceException = new WebException("An error occurred while fetching moviemetadata.", wex);

                _logger.LogError(webSourceException.Message, wex);

                throw webSourceException;
            }

            
        }

        public List<Movie> CsvMovieMetadataRead()
        {
            var metadatafilename = Directory.GetCurrentDirectory() + @"\metadata.csv";
            List<Movie> lstmovies = new List<Movie>();
            using (TextFieldParser csvReader = new TextFieldParser(metadatafilename))
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

                    lstmovies.Add(new Movie
                    {
                        Id = Convert.ToInt32(fieldData[0]),
                        MovieId = Convert.ToInt32(fieldData[1]),
                        Title = fieldData[2],
                        Language = fieldData[3],
                        Duration = fieldData[4],
                        ReleaseYear =  Convert.ToInt32(fieldData[5])
                    });
                }
            }

            return lstmovies;
        }

        private List<Movie> MoviesFilter (List<Movie> lstmovies,int movieid)
        {

            var lstresult = from n in lstmovies
                             where n.MovieId.Equals(movieid)
                             group n by n.Language into g
                             select new Movie
                             {
                                 Id = g.Max(t => t.Id),
                                 MovieId = (from t2 in g select t2.MovieId).Max(),
                                 Title = (from t2 in g select t2.Title).Max(),
                                 ReleaseYear = (from t2 in g select t2.ReleaseYear).Max(),
                                 Language = (from t2 in g select t2.Language).Max(),
                                 Duration = (from t2 in g select t2.Duration).Max()
                             };

            var lists = lstresult.AsEnumerable().ToList();
            return lists;

        }

    }
}

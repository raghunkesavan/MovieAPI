using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using MovieApiLibrary;


namespace Movie_Api.Controllers
{
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly ILogger<MovieController> _logger;
        private readonly IMovieMetadata _moviemetadata;
        private readonly ICsvMetaDataStats _csvmetadatastats;
        private readonly ICsvMetaDataFileRead _csvfileread;

        public MovieController(ILogger<MovieController> logger,  
            IMovieMetadata movieMetadata, ICsvMetaDataStats csvMetaDataStats, 
            ICsvMetaDataFileRead csvFileRead)
        {
            _logger = logger;
            _moviemetadata = movieMetadata;
            _csvmetadatastats = csvMetaDataStats;
            _csvfileread = csvFileRead;
        }

        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [HttpGet]
        [Route("GET/metadata/{movieId}")]
        public ActionResult GetMoviemetaData(int movieId)
        {
            try
            {
                var result = _csvfileread.LookupMoviesById(movieId);
                if (result.Count == 0)
                {
                    return NotFound();
                }
                else
                {
                    return Accepted(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed To get MovieMetadata", ex);
                throw;
            }
        }

        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [HttpGet]
        [Route("GET/metadata/stats")]
        public ActionResult GetMovieMetaDataStats()
        {
            try
            {
                var result  = _csvmetadatastats.CsvMovieStatsRead();

                if (result.Count == 0)
                {
                    return NotFound();
                }
                else
                {
                    return Accepted(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed To get Metadata Stats", ex);
                throw;
            }
        }

        [HttpPost]
        [Route("POST/metadata")]
        public ActionResult PostMetaData([FromBody] Movie movie)
        {
            try
            {
                if (movie == null)
                {
                    return BadRequest("Failed");
                }
                else
                {
                    var lstmovies = _moviemetadata.AddMovieMetaData(movie);
                    return Accepted("Success");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed To post movie", ex);
                throw;
            }
        }

    }
}

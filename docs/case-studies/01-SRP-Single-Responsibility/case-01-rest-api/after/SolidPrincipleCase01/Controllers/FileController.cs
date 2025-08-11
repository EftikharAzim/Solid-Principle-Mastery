using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolidPrincipleCase01.Models;
using SolidPrincipleCase01.Services;

namespace SolidPrincipleCase01.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IFileValidationService _fileValidator;
        private readonly ISearchService _searchService;
        private readonly ISearchResultRepository _searchResultRepository;
        private readonly ILogger<FilesController> _logger;

        public FilesController(
            IFileValidationService fileValidator,
            ISearchService searchService,
            ISearchResultRepository searchResultRepository,
            ILogger<FilesController> logger
        )
        {
            _fileValidator = fileValidator;
            _searchService = searchService;
            _searchResultRepository = searchResultRepository;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<FileUploadResponse>> UploadCsv(IFormFile file)
        {
            _logger.LogInformation("File upload started. File: {FileName}", file?.FileName);

            if (file == null)
                return BadRequest(new { Message = "No file provided" });

            // Validate file format
            if (!_fileValidator.IsValidCsvFormat(file))
                return BadRequest(
                    new { Message = "Invalid CSV format. Please upload a .csv file." }
                );

            try
            {
                // Extract keywords
                var keywords = await _fileValidator.ExtractKeywordsAsync(file.OpenReadStream());
                _logger.LogInformation("Extracted {Count} keywords from file", keywords.Count);

                if (keywords.Count == 0)
                    return BadRequest(new { Message = "No valid keywords found in the file" });

                // Process each keyword with search service
                var allResults = await _searchService.SearchMultipleAsync(keywords);
                _logger.LogInformation(
                    "Search completed. Found {Count} total results",
                    allResults.Count
                );

                // Save results to repository
                await _searchResultRepository.SaveSearchResultsAsync(allResults);
                _logger.LogInformation("Search results saved to database");

                return Ok(
                    new FileUploadResponse
                    {
                        Message =
                            $"File processed successfully! Found {allResults.Count} search results.",
                        KeywordCount = keywords.Count,
                        ResultCount = allResults.Count,
                        ProcessedKeywords = keywords,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File processing failed");
                return StatusCode(500, new { Message = "Processing failed", Error = ex.Message });
            }
        }

        [HttpGet("results")]
        public async Task<ActionResult<List<SearchResult>>> GetAllResults()
        {
            var results = await _searchResultRepository.GetAllSearchResultsAsync();
            return Ok(results);
        }

        [HttpGet("results/{keyword}")]
        public async Task<ActionResult<List<SearchResult>>> GetResultsByKeyword(string keyword)
        {
            var results = await _searchResultRepository.GetSearchResultsByKeywordAsync(keyword);
            return Ok(results);
        }
    }
}

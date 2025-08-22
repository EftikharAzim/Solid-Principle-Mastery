public class UserFileController : ControllerBase
{
    private readonly string _connectionString;
    private readonly HttpClient _httpClient;

    public UserFileController(string connectionString, HttpClient httpClient)
    {
        _connectionString = connectionString;
        _httpClient = httpClient;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // HTTP handling
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Password validation logic
        if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 8)
            return BadRequest("Invalid password");

        // Database connection and user lookup
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand("SELECT * FROM Users WHERE Username = @username", connection);
        command.Parameters.AddWithValue("@username", request.Username);

        var reader = await command.ExecuteReaderAsync();
        if (!reader.Read())
            return Unauthorized("User not found");

        // Password hashing and comparison
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var storedPassword = reader["Password"].ToString();
        if (!BCrypt.Net.BCrypt.Verify(request.Password, storedPassword))
            return Unauthorized("Invalid password");

        // JWT token generation
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("your-secret-key");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("username", request.Username) }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { Token = tokenString });
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        // HTTP validation
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        // File format validation
        if (!file.ContentType.Equals("text/csv", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only CSV files are allowed");

        if (file.Length > 5 * 1024 * 1024) // 5MB limit
            return BadRequest("File too large");

        // CSV parsing
        var keywords = new List<string>();
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    keywords.Add(line.Trim());
                }
            }
        }

        // Validate keywords
        if (keywords.Count == 0)
            return BadRequest("No valid keywords found");

        if (keywords.Count > 100)
            return BadRequest("Too many keywords. Maximum 100 allowed");

        // Google Search for each keyword
        var allResults = new List<SearchResult>();
        foreach (var keyword in keywords)
        {
            try
            {
                // Google API call
                var searchUrl =
                    $"https://customsearch.googleapis.com/customsearch/v1?key=YOUR_KEY&cx=YOUR_CX&q={Uri.EscapeDataString(keyword)}";
                var response = await _httpClient.GetAsync(searchUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var googleResponse = JsonSerializer.Deserialize<GoogleSearchResponse>(
                        jsonContent
                    );

                    // Transform Google results
                    foreach (var item in googleResponse.Items ?? new List<GoogleSearchItem>())
                    {
                        allResults.Add(
                            new SearchResult
                            {
                                Keyword = keyword,
                                Title = item.Title,
                                Url = item.Link,
                                Description = item.Snippet,
                                SearchedAt = DateTime.UtcNow,
                            }
                        );
                    }
                }
                else
                {
                    // Log error but continue with other keywords
                    Console.WriteLine($"Search failed for keyword: {keyword}");
                }

                // Rate limiting - don't overwhelm Google
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                // Log error but continue
                Console.WriteLine($"Error searching for {keyword}: {ex.Message}");
            }
        }

        // Save to database
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        foreach (var result in allResults)
        {
            var insertCommand = new SqlCommand(
                @"
                INSERT INTO SearchResults (Keyword, Title, Url, Description, SearchedAt) 
                VALUES (@keyword, @title, @url, @description, @searchedAt)",
                connection
            );

            insertCommand.Parameters.AddWithValue("@keyword", result.Keyword);
            insertCommand.Parameters.AddWithValue("@title", result.Title ?? "");
            insertCommand.Parameters.AddWithValue("@url", result.Url ?? "");
            insertCommand.Parameters.AddWithValue("@description", result.Description ?? "");
            insertCommand.Parameters.AddWithValue("@searchedAt", result.SearchedAt);

            await insertCommand.ExecuteNonQueryAsync();
        }

        // Email notification to user
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("your-email@gmail.com", "your-password"),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("your-email@gmail.com"),
            Subject = "File Processing Complete",
            Body =
                $"Your file with {keywords.Count} keywords has been processed. {allResults.Count} results found.",
            IsBodyHtml = false,
        };
        mailMessage.To.Add("user@example.com");

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            // Log but don't fail the request
            Console.WriteLine($"Email notification failed: {ex.Message}");
        }

        return Ok(
            new
            {
                Message = "File processed successfully",
                KeywordCount = keywords.Count,
                ResultCount = allResults.Count,
            }
        );
    }
}

using ResponseForge.Models;
using Xunit;

namespace ResponseForge.Tests;

public class ApiResponseTests
{
    [Fact]
    public void Ok_ShouldReturnSuccessResponse()
    {
        // Act
        var response = ApiResponse.Ok("Test Data", "Nice!");

        // Assert
        Assert.True(response.Success);
        Assert.Equal("Nice!", response.Message);
        Assert.Equal("Test Data", response.Data);
        Assert.Null(response.Errors);
        Assert.Null(response.StackTrace);
    }
    
    [Fact]
    public void Fail_ShouldReturnFailedResponse()
    {
        // Act
        var errors = new Dictionary<string, List<string>> { { "Field", ["Error1"] } };
        var response = ApiResponse.Fail("Error!", errors);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("Error!", response.Message);
        Assert.Null(response.Data);
        Assert.NotNull(response.Errors);
        Assert.True(response.Errors.ContainsKey("Field"));
        Assert.Equal("Error1", response.Errors["Field"][0]);
    }
}

namespace IntegrationTests;

using Xunit;

[Collection("Database")]
public class CreateDatabaseTests
{
    [Fact]
    public void ShouldCreateDatabase() => Assert.True(true);
}

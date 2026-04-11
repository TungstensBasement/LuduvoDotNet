namespace LuduvoDotNet.Tests
{
    public class UserApiTests
    {
        [Fact]
        public async Task GetUserByIdAsync_Should_ReturnCorrectData()
        {
            var luduvo = new Luduvo();
            var result = await luduvo.GetUserByIdAsync(2);
            Assert.NotNull(result);
            Assert.Equal(2u, result.UserId);
            Assert.Equal("IgorAlexey", result.Username);
            Assert.Equal(1768055400ul, result.MemberSince);
        }
    }
}

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Tweedekamer.Controllers;
using Tweedekamer.Models;

namespace Tweedekamer.Tests
{
    public class BlogTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _factory;

        public BlogTests(DatabaseFixture factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetBlogPosts_ReturnsCorrectNumberOfBlogPosts()
        {
            // Arrange
            _factory._context.BlogPosts.Add(new BlogPost { Title = "Test Title 1", Content = "Test Content 1" });
            _factory._context.BlogPosts.Add(new BlogPost { Title = "Test Title 2", Content = "Test Content 2" });
            await _factory._context.SaveChangesAsync();
            var controller = new BlogController(_factory._context);

            // Act
            var result = await controller.GetBlogPosts();

            // Asserts
            var actionResult = result.Should().BeOfType<ActionResult<IEnumerable<BlogPost>>>().Subject;
            var blogPosts = actionResult.Value.Should().BeOfType<List<BlogPost>>().Subject;
            blogPosts.Count.Should().Be(2);
        }


        [Fact]
        public async Task GetBlogPost_ValidId_ReturnsCorrectBlogPost()
        {
            // Arrange
            var getIdBlogPost = new BlogPost { Id = 8, Title = "Old Title", Content = "Old Content" };
            _factory._context.BlogPosts.Add(getIdBlogPost);
            await _factory._context.SaveChangesAsync();
            var service = new BlogController(_factory._context);

            // Act
            var result = await service.GetBlogPost(8);

            // Assert
            var actionResult = result.Should().BeOfType<ActionResult<BlogPost>>().Subject;
            var returnedBlogPost = actionResult.Value.Should().BeOfType<BlogPost>().Subject;
            returnedBlogPost.Id.Should().Be(getIdBlogPost.Id);
            returnedBlogPost.Title.Should().Be(getIdBlogPost.Title);
            returnedBlogPost.Content.Should().Be(getIdBlogPost.Content);
        }

        [Fact]
        public async Task UpdateBlogPost_UpdatesCorrectly()
        {
            // Arrange
            var updateBlogPost = new BlogPost { Id = 5, Title = "Old Title", Content = "Old Content" };
            _factory._context.BlogPosts.Add(updateBlogPost);
            await _factory._context.SaveChangesAsync();

            var updatedTitle = "New Title";
            var updatedContent = "New Content";
            var service = new BlogController(_factory._context);
            updateBlogPost.Title = updatedTitle;
            updateBlogPost.Content = updatedContent;

            // Act
            await service.UpdateBlogPost(5, updateBlogPost);

            // Assert
            var blogPostInDb = await _factory._context.BlogPosts.FindAsync(5);
            blogPostInDb.Title.Should().Be("New Title");
            blogPostInDb.Content.Should().Be("New Content");
        }

        [Fact]
        public async Task CreateBlogPost_ValidBlogPost_CreatesBlogPost()
        {
            // Arrange
            var createBlogPost = new BlogPost { Id = 6, Title = "New Title", Content = "New Content" };
            var service = new BlogController(_factory._context);

            // Act
            var result = await service.CreateBlogPost(createBlogPost);

            // Assert
            var blogPostInDb = await _factory._context.BlogPosts.FindAsync(6);
            blogPostInDb.Should().NotBeNull();
            result.Should().NotBeNull();
            blogPostInDb.Id.Should().Be(createBlogPost.Id);
            blogPostInDb.Title.Should().Be(createBlogPost.Title);
            blogPostInDb.Content.Should().Be(createBlogPost.Content);
        }

        [Fact]
        public async Task DeleteBlogPost_ValidId_DeletesBlogPost()
        {
            // Arrange
            var deleteBlogPost = new BlogPost { Id = 9, Title = "Title", Content = "Content" };
            _factory._context.BlogPosts.Add(deleteBlogPost);
            await _factory._context.SaveChangesAsync();
            var service = new BlogController(_factory._context);

            // Act
            await service.DeleteBlogPost(9);

            // Assert
            var blogPostInDb = await _factory._context.BlogPosts.FindAsync(9);
            blogPostInDb.Should().BeNull();
        }
    }

}


//    [Theory]
//    [InlineData("api/Blog")]
//    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
//    {
//        // Arrange
//        var client = _factory.CreateClient();

//        // Act
//        var response = await client.GetAsync(url);

//        // Assert
//        response.EnsureSuccessStatusCode(); // Status Code 200-299
//        Assert.Equal("application/json; charset=utf-8",
//            response.Content.Headers.ContentType.ToString());
//    }

//    [Fact]
//    public async Task GetBlogPost_ReturnsNotFound_ForInvalidId()
//    {
//        // Arrange
//        var client = _factory.CreateClient();

//        // Act
//        var response = await client.GetAsync("api/Blog/9999");

//        // Assert
//        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//    }

//}

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
            var actionResult = Assert.IsType<ActionResult<IEnumerable<BlogPost>>>(result);
            var blogPosts = Assert.IsType<List<BlogPost>>(actionResult.Value);
            Assert.Equal(2, blogPosts.Count);
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
            var actionResult = Assert.IsType<ActionResult<BlogPost>>(result);
            var returnedBlogPost = Assert.IsType<BlogPost>(actionResult.Value);
            Assert.Equal(getIdBlogPost.Id, returnedBlogPost.Id);
            Assert.Equal(getIdBlogPost.Title, returnedBlogPost.Title);
            Assert.Equal(getIdBlogPost.Content, returnedBlogPost.Content);
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
            Assert.Equal("New Title", blogPostInDb.Title);
            Assert.Equal("New Content", blogPostInDb.Content);
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
            Assert.NotNull(blogPostInDb);
            Assert.NotNull(result);
            Assert.Equal(createBlogPost.Id, blogPostInDb.Id);
            Assert.Equal(createBlogPost.Title, blogPostInDb.Title);
            Assert.Equal(createBlogPost.Content, blogPostInDb.Content);
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
            Assert.Null(blogPostInDb);
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

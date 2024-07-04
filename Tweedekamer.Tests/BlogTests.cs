using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        public async Task GetBlogPostJson_ValidId_ReturnsCorrectBlogPost()
        {
            // Arrange
            using (var context = _factory.CreateContext())
            {
                // Clear the BlogPosts from the previous test
                context.BlogPosts.RemoveRange(context.BlogPosts);

                var id = 1;
                var json = File.ReadAllText("/Dev/Tweedekamer/Tweedekamer.Tests/Blog.json");
                var blogPostsFromJson = JsonConvert.DeserializeObject<List<BlogPost>>(json);

                foreach (var blogPost in blogPostsFromJson)
                {
                    context.BlogPosts.Add(blogPost);
                }
                await context.SaveChangesAsync();

                var service = new BlogController(context);

                // Act
                var result = await service.GetBlogPost(id);

                // Assert
                var actionResult = result.Should().BeOfType<ActionResult<BlogPost>>().Subject;
                var returnedBlogPost = actionResult.Value.Should().BeOfType<BlogPost>().Subject;
                var getIdBlogPost = await context.BlogPosts.FindAsync(id);
                returnedBlogPost.Id.Should().Be(getIdBlogPost.Id);
                returnedBlogPost.Title.Should().Be(getIdBlogPost.Title);
                returnedBlogPost.Content.Should().Be(getIdBlogPost.Content);
            }
        }


        [Fact]
        public async Task UpdateBlogPostJson_UpdatesCorrectly()
        {
            // Arrange
            using (var context = _factory.CreateContext())
            {
                // Clear the BlogPosts from the previous test
                context.BlogPosts.RemoveRange(context.BlogPosts);

                var id = 3;
                var json = File.ReadAllText("/Dev/Tweedekamer/Tweedekamer.Tests/Blog.json");
                var blogPostsFromJson = JsonConvert.DeserializeObject<List<BlogPost>>(json);

                foreach (var blogPost in blogPostsFromJson)
                {
                    context.BlogPosts.Add(blogPost);
                }
                await context.SaveChangesAsync();

                var updatedTitle = "New Title";
                var updatedContent = "New Content";
                var service = new BlogController(context);

                var updateBlogPost = await context.BlogPosts.FindAsync(id);
                updateBlogPost.Title = updatedTitle;
                updateBlogPost.Content = updatedContent;

                // Act
                await service.UpdateBlogPost(id, updateBlogPost);

                // Assert
                var blogPostInDb = await context.BlogPosts.FindAsync(id);
                blogPostInDb.Title.Should().Be("New Title");
                blogPostInDb.Content.Should().Be("New Content");
            }
        }

        [Fact]
        public async Task CreateBlogPostJson_ValidBlogPost_CreatesBlogPost()
        {
            // Arrange
            using (var context = _factory.CreateContext())
            {
                // Clear the BlogPosts from the previous test
                context.BlogPosts.RemoveRange(context.BlogPosts);

                var json = File.ReadAllText("/Dev/Tweedekamer/Tweedekamer.Tests/Blog.json");
                var blogPostsFromJson = JsonConvert.DeserializeObject<List<BlogPost>>(json);

                foreach (var blogPost in blogPostsFromJson)
                {
                    context.BlogPosts.Add(blogPost);
                }
                await context.SaveChangesAsync();

                var id = 998;
                var createBlogPost = new BlogPost { Id = id, Title = "New Title", Content = "New Content" };
                var service = new BlogController(context);

                // Act
                var result = await service.CreateBlogPost(createBlogPost);

                // Assert
                var blogPostInDb = await context.BlogPosts.FindAsync(id);
                blogPostInDb.Should().NotBeNull();
                result.Should().NotBeNull();
                blogPostInDb.Id.Should().Be(createBlogPost.Id);
                blogPostInDb.Title.Should().Be(createBlogPost.Title);
                blogPostInDb.Content.Should().Be(createBlogPost.Content);
            }
        }


        [Fact]
        public async Task DeleteBlogPostJson_ValidId_DeletesBlogPost()
        {
            // Arrange
            using (var context = _factory.CreateContext())
            {
                // Clear the BlogPosts from the previous test
                context.BlogPosts.RemoveRange(context.BlogPosts);

                var id = 1;
                var json = File.ReadAllText("/Dev/Tweedekamer/Tweedekamer.Tests/Blog.json");
                var blogPostsFromJson = JsonConvert.DeserializeObject<List<BlogPost>>(json);

                foreach (var blogPost in blogPostsFromJson)
                {
                    context.BlogPosts.Add(blogPost);
                }
                await context.SaveChangesAsync();

                var service = new BlogController(context);

                // Act
                await service.DeleteBlogPost(id);

                // Assert
                var blogPostInDb = await context.BlogPosts.FindAsync(id);
                blogPostInDb.Should().BeNull();
            }
        }


        [Fact]
        public async Task GetBlogPostsJson_ReturnsCorrectNumberOfBlogPosts()
        {
            // Arrange
            using (var context = _factory.CreateContext())
            {
                // Clear the BlogPosts from the previous test
                context.BlogPosts.RemoveRange(context.BlogPosts);

                var json = File.ReadAllText("/Dev/Tweedekamer/Tweedekamer.Tests/Blog.json");
                var blogPostsFromJson = JsonConvert.DeserializeObject<List<BlogPost>>(json);

                foreach (var blogPost in blogPostsFromJson)
                {
                    context.BlogPosts.Add(blogPost);
                }
                await context.SaveChangesAsync();
                var controller = new BlogController(context);

                // Act
                var result = await controller.GetBlogPosts();

                // Asserts
                var actionResult = result.Should().BeOfType<ActionResult<IEnumerable<BlogPost>>>().Subject;
                var blogPosts = actionResult.Value.Should().BeOfType<List<BlogPost>>().Subject;
                blogPosts.Count.Should().Be(blogPostsFromJson.Count);
                blogPosts.Should().BeEquivalentTo(blogPostsFromJson);

            }
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

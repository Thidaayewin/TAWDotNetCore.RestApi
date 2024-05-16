using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Dapper;
using TAWDotNetCore.RestApi.Models;

namespace TAWDotNetCore.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogDapperController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;

        public BlogDapperController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        [HttpGet]
        public IActionResult Read()
        {
            string query = "select * from Tbl_Blog";
            var lst = _dbConnection.Query<BlogModel>(query).ToList();
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult Edit(int id)
        {
            BlogModel blog = new BlogModel
            {
                BlogId = id
            };
            string query = "select * from Tbl_Blog where BlogId = @BlogId";
            var item = _dbConnection.Query<BlogModel>(query, blog).FirstOrDefault();
            if (item is null)
            {
                return NotFound("No data found.");
            }

            return Ok(item);
        }

        [HttpPost]
        public IActionResult Create(BlogModel blog)
        {
            string query = @"
                INSERT INTO [dbo].[Tbl_Blog]
                           ([BlogTitle]
                           ,[BlogAuthor]
                           ,[BlogContent])
                     VALUES
                           (@BlogTitle
                           ,@BlogAuthor
                           ,@BlogContent)
                ";
            var result = _dbConnection.Execute(query, blog);

            string message = result > 0 ? "Saving Successful." : "Saving Failed.";
            return Ok(message);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] BlogModel blog)
        {
            string querySelect = "select * from Tbl_Blog where BlogId = @BlogId";
            var item = _dbConnection.Query<BlogModel>(querySelect, new { BlogId = id }).FirstOrDefault();
            if (item is null)
            {
                return NotFound("No data found.");
            }

            string query = @"
                UPDATE [dbo].[Tbl_Blog]
                   SET [BlogTitle] = @BlogTitle
                      ,[BlogAuthor] = @BlogAuthor
                      ,[BlogContent] = @BlogContent
                 WHERE BlogId = @BlogId
                ";
            blog.BlogId = id;
            var result = _dbConnection.Execute(query, blog);

            string message = result > 0 ? "Updating Successful." : "Updating Failed.";
            return Ok(message);
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] BlogModel blog)
        {
            string querySelect = "select * from Tbl_Blog where BlogId = @BlogId";
            var item = _dbConnection.Query<BlogModel>(querySelect, new { BlogId = id }).FirstOrDefault();
            if (item is null)
            {
                return NotFound("No data found.");
            }
            string conditions = string.Empty;

            if (!string.IsNullOrEmpty(blog.BlogTitle))
            {
                item.BlogTitle = blog.BlogTitle;
                conditions += " [BlogTitle] = @BlogTitle, ";
            }
            if (!string.IsNullOrEmpty(blog.BlogAuthor))
            {
                item.BlogAuthor = blog.BlogAuthor;
                conditions += " [BlogAuthor] = @BlogAuthor, ";
            }
            if (!string.IsNullOrEmpty(blog.BlogContent))
            {
                item.BlogContent = blog.BlogContent;
                conditions += " [BlogContent] = @BlogContent, ";
            }
            if (conditions.Length == 0)
            {
                return NotFound("No data to update.");
            }

            conditions = conditions.Substring(0, conditions.Length - 2);// to remove , and space after combined all string
            string query = $@"
                UPDATE [dbo].[Tbl_Blog]
                   SET {conditions}
                 WHERE BlogId = @BlogId
                ";
            blog.BlogId = id;
            var result = _dbConnection.Execute(query, blog);

            string message = result > 0 ? "Updating Successful." : "Updating Failed.";
            return Ok(message);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string querySelect = "select * from Tbl_Blog where BlogId = @BlogId";
            var item = _dbConnection.Query<BlogModel>(querySelect, new { BlogId = id }).FirstOrDefault();
            if (item is null)
            {
                return NotFound("No data found.");
            }

            string query = @"Delete From [dbo].[Tbl_Blog] WHERE BlogId = @BlogId";
            var result = _dbConnection.Execute(query, new { BlogId = id });

            string message = result > 0 ? "Deleting Successful." : "Deleting Failed.";
            return Ok(message);
        }


    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TAWDotNetCore.RestApi.Models;

namespace TAWDotNetCore.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogAdoDotNetController : ControllerBase
    {
        private readonly SqlConnection _connection;

        public BlogAdoDotNetController(SqlConnection connection)
        {
            _connection = connection;
        }

        [HttpGet]
        public IActionResult Read()
        {
            _connection.Open();

            string query = "SELECT * FROM Tbl_Blog";
            SqlCommand cmd = new SqlCommand(query, _connection);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            _connection.Close();

            List<BlogModel> lst = dt.AsEnumerable().Select(dr => new BlogModel
            {
                BlogTitle = dr["BlogTitle"].ToString(),
                BlogAuthor = dr["BlogAuthor"].ToString(),
                BlogContent = dr["BlogContent"].ToString(),
            }).ToList();

            return Ok(lst);
        }

        [HttpGet("{id}")]
        public IActionResult Edit(int id)
        {
            (bool result, DataRow dr) = IsExist(id);
            if (!result)
            {
                return NotFound("No data found.");
            }

            var item = new BlogModel
            {
                BlogTitle = dr["BlogTitle"].ToString(),
                BlogAuthor = dr["BlogAuthor"].ToString(),
                BlogContent = dr["BlogContent"].ToString(),
            };
            return Ok(item);
        }

        [HttpPost]
        public IActionResult Create(BlogModel blog)
        {
            _connection.Open();

            string query = "INSERT INTO Tbl_Blog (BlogTitle, BlogAuthor, BlogContent) VALUES (@BlogTitle, @BlogAuthor, @BlogContent)";
            SqlCommand cmd = new SqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@BlogTitle", blog.BlogTitle);
            cmd.Parameters.AddWithValue("@BlogAuthor", blog.BlogAuthor);
            cmd.Parameters.AddWithValue("@BlogContent", blog.BlogContent);

            int result = cmd.ExecuteNonQuery();
            _connection.Close();

            string message = result > 0 ? "Saving Successful." : "Saving Failed.";
            return Ok(message);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] BlogModel blog)
        {
            (bool exists, DataRow _) = IsExist(id);
            if (!exists)
            {
                return NotFound("No data found.");
            }

            _connection.Open();

            string query = "UPDATE Tbl_Blog SET BlogTitle = @BlogTitle, BlogAuthor = @BlogAuthor, BlogContent = @BlogContent WHERE BlogId = @BlogId";
            SqlCommand cmd = new SqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@BlogId", id);
            cmd.Parameters.AddWithValue("@BlogTitle", blog.BlogTitle);
            cmd.Parameters.AddWithValue("@BlogAuthor", blog.BlogAuthor);
            cmd.Parameters.AddWithValue("@BlogContent", blog.BlogContent);

            int result = cmd.ExecuteNonQuery();
            _connection.Close();

            string message = result > 0 ? "Updating Successful." : "Updating Failed.";
            return Ok(message);
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] BlogModel blog)
        {
            (bool exists, DataRow dr) = IsExist(id);
            if (!exists)
            {
                return NotFound("No data found.");
            }

            _connection.Open();

            string query = "UPDATE Tbl_Blog SET BlogTitle = COALESCE(@BlogTitle, BlogTitle), BlogAuthor = COALESCE(@BlogAuthor, BlogAuthor), BlogContent = COALESCE(@BlogContent, BlogContent) WHERE BlogId = @BlogId";
            SqlCommand cmd = new SqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@BlogId", id);
            cmd.Parameters.AddWithValue("@BlogTitle", string.IsNullOrEmpty(blog.BlogTitle) ? DBNull.Value : (object)blog.BlogTitle);
            cmd.Parameters.AddWithValue("@BlogAuthor", string.IsNullOrEmpty(blog.BlogAuthor) ? DBNull.Value : (object)blog.BlogAuthor);
            cmd.Parameters.AddWithValue("@BlogContent", string.IsNullOrEmpty(blog.BlogContent) ? DBNull.Value : (object)blog.BlogContent);

            int result = cmd.ExecuteNonQuery();
            _connection.Close();

            string message = result > 0 ? "Patching Successful." : "Patching Failed.";
            return Ok(message);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            (bool exists, DataRow _) = IsExist(id);
            if (!exists)
            {
                return NotFound("No data found.");
            }

            _connection.Open();

            string query = "DELETE FROM Tbl_Blog WHERE BlogId = @BlogId";
            SqlCommand cmd = new SqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@BlogId", id);

            int result = cmd.ExecuteNonQuery();
            _connection.Close();

            string message = result > 0 ? "Deleting Successful." : "Deleting Failed.";
            return Ok(message);
        }

        //checking data exist or not
        private (bool, DataRow) IsExist(int id)
        {
            _connection.Open();

            string query = "select * from Tbl_Blog where BlogId = @BlogId";
            SqlCommand cmd = new SqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@BlogId", id);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            _connection.Close();

            if (dt.Rows.Count > 0)
            {
                return (true, dt.Rows[0]);
            }
            return (false, null);
        }
    }
}

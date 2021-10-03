using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sommersoftware.dk.Models;
using Newtonsoft.Json;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace sommersoftware.dk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        // GET: api/<StudentController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<StudentController>/5
        [HttpGet("{id}")]
        public ActionResult<Student> Get(String id)
        {
            if (id == "20184142")
            {
                return new Student(Int64.Parse(id), "Jannik Lucas Sommer", "Student");
            }
            return null;
        }

        // POST api/<StudentController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<StudentController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<StudentController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

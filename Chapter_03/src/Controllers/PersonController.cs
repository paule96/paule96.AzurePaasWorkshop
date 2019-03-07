using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using src.Models;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        public PersonContext Context { get; }

        public PersonController(PersonContext context)
        {
            Context = context;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Person>> Get()
        {
            return Context.Person;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<Person> Get(int id)
        {
            return Context.Person.FirstOrDefault(d => d.Id == id);
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromBody] Person value)
        {
            await Context.Person.AddAsync(value);
            await Context.SaveChangesAsync();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] Person value)
        {
            value.Id = id;
            Context.Person.Update(value);
            await Context.SaveChangesAsync();

        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            Context.Person.Remove(Context.Person.FirstOrDefault(d => d.Id == id));
            await Context.SaveChangesAsync();
        }
    }
}

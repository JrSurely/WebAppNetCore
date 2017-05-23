using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;  
using WebAppNetCore.DTO;

namespace WebAppNetCore.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : ApiController
    {
        private IOptions<PayTypeConfigs> _option = null;
        public ValuesController(IOptions<PayTypeConfigs> option)
        {
            _option = option;
        }
        // GET api/values
        [HttpGet]
        public IDictionary<string, PayTypeInfo> Get()
        {
            return _option.Value.Types;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public HttpResponseMessage Post(StudentDto request)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = "Validation failed."
                });
            }
            return this.Request.CreateResponse(HttpStatusCode.OK);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

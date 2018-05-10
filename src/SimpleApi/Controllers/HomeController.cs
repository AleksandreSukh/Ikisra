using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;

namespace ProjectAPI.Controllers
{
    //[RoutePrefix("api")]
    public class HomeController : ApiController
    {
        //static FakeDataSource Data = new FakeDataSource();

        public IHttpActionResult Get()
        {
            return ParameterMissingError();
        }
        [Route("{test}")]
        public IHttpActionResult Get(string cityName)
        {

            if (string.IsNullOrEmpty(cityName))
            {
                return ParameterMissingError();
            }

            return new JsonResult<string>("test", new JsonSerializerSettings(), Encoding.UTF8, this);
        }

        private static IHttpActionResult ParameterMissingError()
        {
            return new ResponseMessageResult(new HttpResponseMessage()
            {
                Content = new StringContent("Parameter should be provided!")
            });
        }
    }
}

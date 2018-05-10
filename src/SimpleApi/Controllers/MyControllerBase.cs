using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using RestApiBase;

namespace ProjectAPI.Controllers
{

    public static class ControllerBaseExtensions
    {
        public static string RemoveController(this string controllerName)
        {
            var toRemoveAfterIndex = controllerName.ToLower().LastIndexOf("controller");
            return controllerName.Remove(toRemoveAfterIndex);
        }
    }
    public class MyControllerBase : ApiController
    {
        [AcceptVerbs("GET")]
        [Description("Get help about available actions and sublinks")]
        public async Task<IHttpActionResult> Help()
        {
            return Ok(GetLinkedPage(this.GetType().GetTypeInfo()));
        }





        public static string BaseUrl(Type ControllerType) => "api" + "/" + ControllerType.Name.RemoveController();


        public static List<HttpActionDescriptor> GetControllerActions(Type ControllerType)
        {
            if (!typeof(ApiController).IsAssignableFrom(ControllerType))
                throw new NotSupportedException($"Getting actions from controller:{ControllerType.Name} not supported because it doesn't inherit from type {typeof(ApiController).Name}");
            ApiControllerActionSelector ApiControllerSelection = new ApiControllerActionSelector();
            HttpControllerDescriptor ApiDescriptor = new HttpControllerDescriptor(new HttpConfiguration(), ControllerType.Name, ControllerType);
            ILookup<string, HttpActionDescriptor> ApiMappings = ApiControllerSelection.GetActionMapping(ApiDescriptor);

            return ApiMappings.SelectMany(a => a).ToList();
        }


        public static List<Link> GetLinks(Type controllerType)
        {
            var links = GetControllerActions(controllerType)
                .Select(A => ((ReflectedHttpActionDescriptor)(A)).MethodInfo)
                .Select(methodInfo => GetLinkFromMethodInfo(methodInfo))
                .ToList();
            return links;
        }

        public static List<Link> GetlinksForElement(Type controllertype)
        {
            var links = new List<Link>();
            foreach (var methodInfo in GetControllerActions(controllertype)
                .Select(A => ((ReflectedHttpActionDescriptor)(A)).MethodInfo)
                .Where(m => !m.GetParameters().All(p => p.Name.ToLower() != "id")))
            {
                links.Add(new Link() { Description = GetDescriptionAttributeValue(methodInfo), Name = methodInfo.Name, Url = "/" + methodInfo.Name });
            }

            return links;
        }

        private static string GetDescriptionAttributeValue(MethodInfo methodinfo)
        {
            var description = string.Empty;
            foreach (DescriptionAttribute da in methodinfo.GetCustomAttributes(typeof(DescriptionAttribute)))
            {
                description += da.Description + Environment.NewLine;
            }
            return description;
        }

        public static void AddLinks(LinkedResource lr, Type controllerType, int id)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("id"))
                lr.Links = GetlinksForElement(controllerType);
            lr.HRef = BaseUrl(controllerType) + $"/{id}";
        }

        public static Link GetLinkFromMethodInfo(MethodInfo methodInfo)
        {
            return new Link()
            {
                Description = GetDescriptionAttributeValue(methodInfo),
                Name = methodInfo.Name,
                Url = "/" + methodInfo.Name
            };
        }

        public static LinkedResourcePage GetLinkedPage(TypeInfo t)
        {
            return new LinkedResourcePage()
            {
                Title = t.Name.RemoveController(),
                Links = MyControllerBase.GetControllerActions(t)
                    .Select(A => ((ReflectedHttpActionDescriptor)(A)).MethodInfo)
                    .Select(methodInfo => MyControllerBase.GetLinkFromMethodInfo(methodInfo))
                    .ToList()
            };
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; } = null;

        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }


}

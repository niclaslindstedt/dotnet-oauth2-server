using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Etimo.Id.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class NoCacheAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers.Add("Cache-Control", "no-store");
            context.HttpContext.Response.Headers.Add("Pragma", "no-cache");

            base.OnResultExecuting(context);
        }
    }
}

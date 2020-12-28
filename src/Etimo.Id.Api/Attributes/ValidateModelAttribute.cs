using Etimo.Id.Service.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;

namespace Etimo.Id.Api.Attributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                IEnumerable<string> errorMessages = context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);

                throw new InvalidRequestException(string.Join(" ", errorMessages));
            }
        }
    }
}

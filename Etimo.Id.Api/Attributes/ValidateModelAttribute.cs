using System.Linq;
using Etimo.Id.Service.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Etimo.Id.Api.Attributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errorMessages = context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);

                throw new InvalidRequestException(string.Join(" ", errorMessages));
            }
        }
    }
}

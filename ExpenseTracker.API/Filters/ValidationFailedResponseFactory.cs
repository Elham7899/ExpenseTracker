using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ExpenseTracker.API.Filters
{
    public static class ValidationFailedResponseFactory
    {
        public static IActionResult Create(ModelStateDictionary modelState)
        {
            var errors = modelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var response = new
            {
                success = false,
                message = "Validation failed",
                errors
            };

            return new BadRequestObjectResult(response);
        }
    }
}
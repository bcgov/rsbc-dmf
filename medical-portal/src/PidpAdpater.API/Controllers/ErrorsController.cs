using FluentValidation;
using pdipadapter.Data.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Errors.Model;
using ValidationException = FluentValidation.ValidationException;

namespace pdipadapter.Controllers
{

    public class ErrorsController : ControllerBase
    {
        /// <summary>
        [HttpPut]
        /// </summary>
        /// <returns></returns>
        [Route("/error")]
        public IActionResult Error()
        {
            Exception? exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
            return Problem(title: exception?.Message, statusCode: GetStatusCode(exception: exception));
        }
        [HttpPost]
        /// </summary>
        /// <returns></returns>
        [Route("/error")]
        public IActionResult PostError()
        {
            Exception? exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
            return Problem(title: exception?.Message, statusCode: GetStatusCode(exception: exception));
        }
        [HttpGet]
        /// </summary>
        /// <returns></returns>
        [Route("/error")]
        public IActionResult GetError()
        {
            Exception? exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
            return Problem(title: exception?.Message, statusCode: GetStatusCode(exception: exception));
        }

        private static int GetStatusCode(Exception? exception) =>
      exception switch
      {
          BadRequestException => StatusCodes.Status400BadRequest,
          NotFoundException => StatusCodes.Status404NotFound,
          KeyNotFoundException => StatusCodes.Status404NotFound,
          NotAuthorizedException => StatusCodes.Status401Unauthorized,
          ValidationException => StatusCodes.Status422UnprocessableEntity,
          _ => StatusCodes.Status500InternalServerError
      };
    }
}

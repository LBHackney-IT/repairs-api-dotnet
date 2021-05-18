using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NUnit.Framework;
using System.Security.Claims;

namespace RepairsApi.Tests.V2.Controllers
{
    [TestFixture]
    public class ControllerTests
    {

        protected static T GetResultData<T>(IActionResult result)
        {
            return (T) (result as ObjectResult)?.Value;
        }

        protected static int? GetStatusCode(IActionResult result)
        {
            return (result as IStatusCodeActionResult).StatusCode;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NUnit.Framework;

namespace RepairsApi.Tests.V1.Controllers
{
    [TestFixture]
    public class ControllerTests
    {

        protected static T GetResultData<T>(IActionResult result)
            where T : class
        {
            return (result as ObjectResult)?.Value as T;
        }

        protected static int? GetStatusCode(IActionResult result)
        {
            return (result as IStatusCodeActionResult).StatusCode;
        }
    }
}

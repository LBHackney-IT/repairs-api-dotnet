using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NUnit.Framework;

namespace RepairsApi.Tests.V1.Controllers
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

using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RepairsApi.Tests.Helpers;
using RepairsApi.V2.Configuration;
using RepairsApi.V2.Controllers;
using RepairsApi.V2.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.Controllers
{
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Tests")]
    public class FilterControllerTests : ControllerTests
    {
        [Test]
        public async Task CallUseCase()
        {
            const string modelName = "modelName";
            var useCaseMock = new Mock<IGetFilterUseCase>();

            var classUnderTest = new FilterController(useCaseMock.Object);

            await classUnderTest.GetFilterInformation(modelName);

            useCaseMock.Verify(uc => uc.Execute(modelName));
        }
    }
}

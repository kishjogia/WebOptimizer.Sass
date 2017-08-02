using Microsoft.Extensions.FileProviders.Physical;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace WebOptimizer.Sass.Test
{
    public class CompilerTest
    {
        [Fact]
        public async Task Compile_Success()
        {
            var processor = new Compiler();
            var pipeline = new Mock<IAssetPipeline>().SetupAllProperties();
            var context = new Mock<IAssetContext>().SetupAllProperties();
            context.Object.Content = new Dictionary<string, string> {
                { "/file.css", "$bg: blue; div {background: $bg}" },
            };

            context.Setup(s => s.HttpContext.RequestServices.GetService(typeof(IAssetPipeline)))
                   .Returns(pipeline.Object);

            string temp = Path.GetTempPath();
            var inputFile = new PhysicalFileInfo(new FileInfo("site.css"));

            pipeline.Setup(s => s.FileProvider.GetFileInfo(It.IsAny<string>()))
                  .Returns(inputFile);

            await processor.ExecuteAsync(context.Object);
            var result = context.Object.Content.First().Value;
            Assert.Equal("div {\n  background: blue; }\n", result);
        }
    }
}

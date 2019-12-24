using System.Collections.Generic;
using PackageAnalyser.Core.Models;
using Xunit;

namespace PackageAnalyzer.Parser.Tests
{
    public class ProjectParserTests
    {
        [Fact]
        public void Parse_ValidProjectFilename_NoPackageFound()
        {
            // Arrange

            // Act 
            List<ProjectItem> projects = ProjectParser.Parse("");

            // Assert
        }
    }
}
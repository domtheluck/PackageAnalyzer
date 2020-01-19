// ***********************************************************************
// Copyright (c) 2019 Dominik Lachance
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.IO;
using System.IO.Compression;
using PackageAnalyzer.Core.Models;
using Xunit;

namespace PackageAnalyzer.Parser.Tests
{
    public class SolutionParserTests
    {
        [Theory]
        [InlineData("SolutionDotnetFrameworkTwoProjects.zip", 2)]
        [InlineData("SolutionMixedFrameworksThreeProjects.zip", 3)]
        public void Parse_ValidSolution_ExpectedProjectCount(string packageFilename, int expectedCount)
        {
            // Arrange
            string basePath = Path.Combine(AppContext.BaseDirectory, "TestData", "SolutionParser");
            string folderName = Path.GetFileNameWithoutExtension(packageFilename);
            string extractedFolderPath = Path.Combine(basePath, folderName);

            ZipFile.ExtractToDirectory(Path.Combine(basePath, packageFilename), basePath, true);

            // Act
            SolutionItem solution = SolutionParser.Parse(Path.Combine(extractedFolderPath, $"{folderName}.sln"));

            // Assert
            Assert.Equal(expectedCount, solution.Projects.Count);
        }

        [Fact]
        public void Parse_ValidSolution_ExpectedProjectOrder()
        {
            // Arrange
            const string packageFilename = "SolutionWithProjectDependencies.zip";

            string basePath = Path.Combine(AppContext.BaseDirectory, "TestData", "SolutionParser");
            string folderName = Path.GetFileNameWithoutExtension(packageFilename);
            string extractedFolderPath = Path.Combine(basePath, folderName);

            ZipFile.ExtractToDirectory(Path.Combine(basePath, packageFilename), basePath, true);

            // Act
            SolutionItem solution = SolutionParser.Parse(Path.Combine(extractedFolderPath, $"{folderName}.sln"));

            // Assert
            Assert.Equal("ProjectC", solution.Projects[0].Name);
            Assert.Equal("ProjectB", solution.Projects[1].Name);
            Assert.Equal("ProjectA", solution.Projects[2].Name);
        }
    }
}
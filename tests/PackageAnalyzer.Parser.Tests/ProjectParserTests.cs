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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using PackageAnalyzer.Core.Extensions;
using PackageAnalyzer.Core.Models;
using Xunit;

namespace PackageAnalyzer.Parser.Tests
{
    public class ProjectParserTests
    {
        [Theory]
        [InlineData("ProjectDotnetFrameworkWithoutPackage.zip", 0)]
        [InlineData("ProjectDotnetFrameworkWithPackages.zip", 2)]
        //[InlineData("ProjetDotnetFrameworkWithoutPackage.zipProjectDotnetCoreWithoutPackage.xml", null, 0)]
        //[InlineData("ProjectDotnetCoreWithPackages.xml", null, 3)]
        public void Parse_ValidProjectFilename_ExpectedCount(string packageFilename, int expectedCount)
        {
            // Arrange
            string basePath = Path.Combine(AppContext.BaseDirectory, "TestData", "ProjectParser");
            string folderName = Path.GetFileNameWithoutExtension(packageFilename);
            string extractedFolderPath = Path.Combine(basePath, folderName);

            ZipFile.ExtractToDirectory(Path.Combine(basePath, packageFilename), basePath, true);

            XDocument projectDocument = XDocument
                .Parse(File.ReadAllText(
                    Path.Combine(extractedFolderPath, $"{folderName}.csproj")))
                .RemoveNamespaces();

            string packageConfigurationFilename = Path.Combine(extractedFolderPath, "packages.config");

            XDocument packagesConfigurationDocument = null;

            if (File.Exists(packageConfigurationFilename))
            {
                packagesConfigurationDocument= XDocument
                    .Parse(File.ReadAllText(
                        Path.Combine(AppContext.BaseDirectory, "TestData", "ProjectParser",
                            packageConfigurationFilename)))
                    .RemoveNamespaces();
            }

            // Act
            List<PackageItem> packages = ProjectParser.Parse(projectDocument, packagesConfigurationDocument);

            // Assert
            Assert.Equal(expectedCount, packages.Count);
        }
    }
}
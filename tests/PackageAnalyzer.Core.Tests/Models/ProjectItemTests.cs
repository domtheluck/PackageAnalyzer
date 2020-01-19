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
using PackageAnalyzer.Core.Models;
using Xunit;

namespace PackageAnalyzer.Core.Tests.Models
{
    public class ProjectItemTests
    {
        [Fact]
        public void Instantiate_ValidContext_ObjectInstantiated()
        {
            // Arrange
            const string expectedName = "MyName";

            // TODO: To fix
            //// Act
            //ProjectItem projectItem = new ProjectItem(
            //    expectedName, 
            //    Guid.NewGuid().ToString(),
            //    new List<PackageReferenceItem> {new PackageReferenceItem("MyPackageId", "MyPackageVersion")});

            //// Assert
            //Assert.NotNull(projectItem);
            //Assert.Equal(expectedName, projectItem.Name);
            //Assert.NotEmpty(projectItem.Packages);
        }
    }
}
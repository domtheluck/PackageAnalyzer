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
using System.Linq;
using System.Xml.Linq;
using PackageAnalyzer.Core.Extensions;
using PackageAnalyzer.Core.Models;

namespace PackageAnalyzer.Parser
{
    public static class ProjectParser
    {
        #region Public Methods

        public static ProjectItem Parse(string filename, string guid)
        {
            if (!File.Exists(filename))
            {
                throw new ArgumentException($"Cannot find project {filename}");
            }

            XDocument projectDocument = XDocument.Parse(File.ReadAllText(filename)).RemoveNamespaces();

            List<PackageReferenceItem> packageReferences = ParsePackageReferences(filename, projectDocument);
            List<ProjectReferenceItem> projectReferences = ParseProjectReferences(projectDocument);

            return new ProjectItem(Path.GetFileNameWithoutExtension(filename), guid, packageReferences,
                projectReferences);
        }

        #endregion

        #region Private Methods

        private static List<ProjectReferenceItem> ParseProjectReferences(XDocument projectDocument)
        {
            return projectDocument.Descendants()
                .Where(descendant => descendant.Name.LocalName.Equals("ProjectReference")).Select(projectReference =>
                    new ProjectReferenceItem(projectReference.Value))
                .ToList();
        }

        private static List<PackageReferenceItem> ParsePackageReferences(string filename, XDocument projectDocument)
        {
            string packageConfigurationFilename = Path.Combine(Path.GetDirectoryName(filename), "packages.config");

            XDocument packageConfigurationDocument = null;

            if (File.Exists(packageConfigurationFilename))
            {
                packageConfigurationDocument = XDocument.Parse(File.ReadAllText(packageConfigurationFilename));
            }

            return packageConfigurationDocument != null
                ? GetPackageReferencesFromLegacyProject(projectDocument, packageConfigurationDocument)
                : GetPackageReferencesFromProject(projectDocument);
        }

        private static List<PackageReferenceItem> GetPackageReferencesFromLegacyProject(
            XContainer projectContainer,
            XContainer packagesConfiguration = null)
        {
            List<PackageReferenceItem> packages = new List<PackageReferenceItem>();

            List<XElement> references = projectContainer.Descendants()
                .Where(descendant => descendant.Name.LocalName.Equals("Reference")).ToList();

            Dictionary<string, string> packagesConfig = packagesConfiguration.Descendants()
                .Where(descendant => descendant.Name.LocalName.Equals("package"))
                .Select(p => new {id = p.Attribute("id")?.Value, version = p.Attribute("version")?.Value})
                .ToDictionary(item => item.id, item => item.version);

            foreach (XElement reference in references)
            {
                string id = reference.Attribute("Include")?.Value.Split(',').First();

                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                if (!packagesConfig.ContainsKey(id))
                {
                    continue;
                }

                packagesConfig.TryGetValue(id, out string value);
                packages.Add(new PackageReferenceItem(id, value));
            }

            return packages;
        }

        private static List<PackageReferenceItem> GetPackageReferencesFromProject(XContainer projectContainer)
        {
            return projectContainer.Descendants()
                .Where(descendant => descendant.Name.LocalName.Equals("PackageReference")).Select(packageReference =>
                    new PackageReferenceItem(
                        packageReference.Attribute("Include")?.Value,
                        packageReference.Attribute("Version")?.Value))
                .ToList();
        }

        #endregion
    }
}
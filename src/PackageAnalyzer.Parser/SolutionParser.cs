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
using Microsoft.Build.Construction;
using PackageAnalyzer.Core.Models;

namespace PackageAnalyzer.Parser
{
    public static class SolutionParser
    {
        #region Public Methods

        public static SolutionItem Parse(string solutionFilename)
        {
            SolutionFile solutionFile = SolutionFile.Parse(solutionFilename);

            List<ProjectItem> projects = solutionFile.ProjectsInOrder
                .Select(project => ProjectParser.Parse(project.AbsolutePath, project.ProjectGuid)).ToList();

            Dictionary<ProjectItem, List<string>> projectContainer = new Dictionary<ProjectItem, List<string>>();

            foreach (ProjectInSolution projectInSolution in solutionFile.ProjectsInOrder)
            {
                ProjectItem project =
                    ProjectParser.Parse(projectInSolution.AbsolutePath, projectInSolution.ProjectGuid);

                List<string> dependencies = projectInSolution.Dependencies.ToList();

                foreach (ProjectReferenceItem projectReference in project.ProjectReferences)
                {
                    ProjectInSolution referredProject = solutionFile.ProjectsInOrder.FirstOrDefault(p =>
                        Path.Combine(p.AbsolutePath, p.ProjectName).ToLowerInvariant().Equals(Path
                            .Combine(projectReference.AbsolutePath, projectReference.Name).ToLowerInvariant()));

                    if (referredProject != null && !dependencies.Contains(referredProject.ProjectGuid))
                    {
                        dependencies.Add(referredProject.ProjectGuid);
                    }
                }

                projectContainer.Add(project, dependencies);
            }

            return new SolutionItem(ResolveBuildOrder(projectContainer));
        }

        #endregion

        #region Private Methods

        private static List<ProjectItem> ResolveBuildOrder(Dictionary<ProjectItem, List<string>> projectContainer)
        {
            DirectedAcyclicGraph<string> graph = new DirectedAcyclicGraph<string>();

            foreach (KeyValuePair<ProjectItem, List<string>> kvp in projectContainer)
            {
                if (!kvp.Value.Any())
                {
                    graph.AddNode(kvp.Key.Guid);
                }

                foreach (string dependency in kvp.Value)
                {
                    graph.AddEdge(kvp.Key.Guid, dependency);
                }
            }

            string[] projectSorted = graph.Sort();

            return projectSorted.Select(projectGuid => projectContainer
                    .First(p => p.Key.Guid.Equals(projectGuid)).Key)
                .ToList();
        }

        #endregion
    }
}
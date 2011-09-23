using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gibbed.Avalanche.FileFormats
{
    public static class ProjectHelpers
    {
        public static ProjectData.HashList<uint> LoadListsFileNames(
            this ProjectData.Manager manager)
        {
            return manager.LoadLists(
                    "*.filelist",
                    s => Path.GetFileName(s).HashJenkins(),
                    s => s.ToLowerInvariant());
        }

        public static ProjectData.HashList<uint> LoadListsFileNames(
            this ProjectData.Project project)
        {
            return project.LoadLists(
                    "*.filelist",
                    s => Path.GetFileName(s).HashJenkins(),
                    s => s.ToLowerInvariant());
        }

        public static ProjectData.HashList<uint> LoadListsPropertyNames(
            this ProjectData.Manager manager)
        {
            return manager.LoadLists(
                    "*.namelist",
                    s => s.HashJenkins(),
                    s => s);
        }

        public static ProjectData.HashList<uint> LoadListsPropertyNames(
            this ProjectData.Project project)
        {
            return project.LoadLists(
                    "*.namelist",
                    s => s.HashJenkins(),
                    s => s);
        }
    }
}

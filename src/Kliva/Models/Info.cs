﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;

namespace Kliva.Models
{
    public class InfoOverviewItem
    {
        public string Header { get; set; }
        public ObservableCollection<string> Items { get; set; }
    }

    public class ApplicationInfo
    {
        public string VersionAsString { private get; set; }
        public string GeneralInfo { get; set; }
        public List<string> Features { get; set; }
        public List<string> BugFixes { get; set; }

        public ObservableCollection<InfoOverviewItem> OverviewItems { get; set; } = new ObservableCollection<InfoOverviewItem>();

        public AppVersion Version
        {
            get
            {
                if (!string.IsNullOrEmpty(VersionAsString))
                {
                    string[] versionInfo = VersionAsString.Split('.');
                    return new AppVersion(new PackageVersion())
                    {
                        Major = int.Parse(versionInfo[0]),
                        Minor = int.Parse(versionInfo[1]),
                        Patch = int.Parse(versionInfo[2]),
                        Iteration = int.Parse(versionInfo[3])
                    };
                }

                return null;
            }
        }
    }
}

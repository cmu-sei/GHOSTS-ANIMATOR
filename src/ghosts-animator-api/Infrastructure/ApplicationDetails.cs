// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.Diagnostics;
using System.Reflection;

namespace Ghosts.Animator.Api.Infrastructure;

public static class ApplicationDetails
{
    public static string Version
    {
        get
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version != null ? version.ToString() : "";
        }
    }
    
    public static string VersionFile
    {
        get
        {
            var fileName = Assembly.GetEntryAssembly()?.Location;
            return fileName != null ? FileVersionInfo.GetVersionInfo(fileName).FileVersion : "";
        }
    }
}
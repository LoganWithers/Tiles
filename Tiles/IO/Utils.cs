﻿namespace Tiles.Models
{

    using System;

    public static class Utils
    {

        private const string ProjectFolder = "\\Tiles\\Tiles\\";
        
        public static string GetPath()
        {
            var currentDirectory = Environment.CurrentDirectory;

            var systemIndependentPathPrefix = currentDirectory.IndexOf(ProjectFolder, StringComparison.OrdinalIgnoreCase);

            return $"{currentDirectory.Substring(0, systemIndependentPathPrefix)}{ProjectFolder}Output\\";

        }
    }
}

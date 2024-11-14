﻿namespace Rsbc.Dmf.DriverPortal.ViewModels
{
    public class ApplicationVersionInfo
    {
        /// <summary>
        /// Base Path of the application
        /// </summary>
        public string BasePath { get; set; }
        /// <summary>
        /// Base URI for the application
        /// </summary>
        public string BaseUri { get; set; }

        /// <summary>
        /// Dotnet Environment (Development, Staging, Production...)
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// File creation time for the running assembly
        /// </summary>
        public string FileCreationTime { get; set; }

        /// <summary>
        /// File version for the running assembly
        /// </summary>
        public string FileVersion { get; set; }

        public string ProductVersion { get; set; }

        /// <summary>
        /// Git commit used to build the application
        /// </summary>
        public string SourceCommit { get; set; }

        /// <summary>
        /// Git reference used to build the application
        /// </summary>
        public string SourceReference { get; set; }

        /// <summary>
        /// Git repository used to build the application
        /// </summary>
        public string SourceRepository { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace Morph.Server.Sdk.Model
{


    public class SpaceBrowsingInfo
    {
        public ulong FreeSpaceBytes { get; set; }
        public string SpaceName { get; set; }
        
        public List<SpaceFolderInfo> Folders { get; set; }        
        public List<SpaceFileInfo> Files { get; set; }
        public List<SpaceNavigation> NavigationChain { get; set; }

        public SpaceBrowsingInfo()
        {
            Folders = new List<SpaceFolderInfo>();
            Files = new List<SpaceFileInfo>();
            NavigationChain = new List<SpaceNavigation>();
        }

        public bool FileExists(string fileName)
        {
           return  Files.Any(x => String.Equals(fileName, x.Name, StringComparison.OrdinalIgnoreCase));
        }



    }

    
    public sealed class SpaceFileInfo
    {
        
        public string Name { get; set; }
        
        public string Extension { get; set; }
        
        public long FileSizeBytes { get; set; }
        
        public DateTime LastModified { get; set; }
        
        /// <summary>
        /// Timestamp in unix time, milliseconds
        /// </summary>
        public long? LastModifiedTimestamp { get; set; }

        public bool IsHidden { get; set; }
    }

    
    public sealed class SpaceFolderInfo
    {
        
        public string Name { get; set; }
        
        public DateTime LastModified { get; set; }
        public bool IsHidden { get; set; }

    }

    public sealed class SpaceNavigation
    {

        public string Name { get; set; }
        public string Path { get; set; }


    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.YoutubePlaylistDownloader.Business.Models.Save
{
    public class PlaylistCacheModel
    {
        public string Id { get; set; }
        
        public bool Download { get; set; }

        public List<VideoCacheModel> DownloadedVideos { get; set; }

        public List<VideoCacheModel> FailedVideos { get; set; }
    }
}

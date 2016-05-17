﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.YoutubePlaylistDownloader.Business.Models;
using Famoser.YoutubePlaylistDownloader.Business.Models.Data;

namespace Famoser.YoutubePlaylistDownloader.Business.Helpers
{
    public class Mp3Helper
    {
        private const int ProgrammVersion = 1;

        public static Mp3FileMetaData GetMp3FileMetaData(Mp3Model model)
        {
            return new Mp3FileMetaData()
            {
                Id = model.VideoInfo.Id,
                V = ProgrammVersion
            };
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services;

namespace Famoser.YoutubePlaylistDownloader.Business.Models
{
    public class ProgressAwareModel : BaseModel
    {
        private ProgressService _progressService;
        public ProgressService ProgressService
        {
            get { return _progressService; }
            set { Set(ref _progressService, value); }
        }
    }
}
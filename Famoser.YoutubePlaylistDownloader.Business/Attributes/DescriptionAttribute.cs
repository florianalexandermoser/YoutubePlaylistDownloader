﻿using System;

namespace Famoser.YoutubePlaylistDownloader.Business.Attributes
{
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; private set; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}

﻿using Famoser.YoutubePlaylistDownloader.Business.Models;
using Famoser.YoutubePlaylistDownloader.Business.Models.Save;

namespace Famoser.YoutubePlaylistDownloader.Business.Helpers.Converters
{
    public class VideoConverter : YoutubeConverter<VideoCacheModel, VideoModel>
    {
        public override VideoCacheModel Convert(VideoModel model)
        {
            var cacheModel = base.Convert(model);
            cacheModel.SaveStatus = model.SaveStatus;
            var converter = new Mp3Converter();
            cacheModel.Mp3Model = converter.Convert(model.Mp3Model);
            return cacheModel;
        }

        public override VideoModel Convert(VideoCacheModel model)
        {
            var cacheModel = base.Convert(model);
            cacheModel.SaveStatus = model.SaveStatus;
            var converter = new Mp3Converter();
            cacheModel.Mp3Model = converter.Convert(model.Mp3Model);
            return cacheModel;
        }
    }
}

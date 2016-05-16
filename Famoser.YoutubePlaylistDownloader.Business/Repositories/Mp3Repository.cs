﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.YoutubePlaylistDownloader.Business.Enums;
using Famoser.YoutubePlaylistDownloader.Business.Models;
using Famoser.YoutubePlaylistDownloader.Business.Repositories.Interfaces;
using Famoser.YoutubePlaylistDownloader.Business.Services;
using Famoser.YoutubePlaylistDownloader.Business.Services.Interfaces;
using TagLib;

namespace Famoser.YoutubePlaylistDownloader.Business.Repositories
{
    public class Mp3Repository : IMp3Respository
    {
        private readonly IFolderStorageService _folderStorageService;
        private static readonly FolderType _type = FolderType.Music;
        private static readonly string subFolder = "youtube";

        public Mp3Repository(IFolderStorageService folderStorageService)
        {
            _folderStorageService = folderStorageService;
        }

        public async Task<List<Mp3Model>> GetModelsForPlaylist(string playlistId, IProgressService service)
        {
            var files = await _folderStorageService.GetAllFilesFromFolder(_type, Path.Combine(subFolder, playlistId));
            var res = new List<Mp3Model>();
            foreach (var file in files)
            {
                var fileStream = await _folderStorageService.GetFile(_type, playlistId, file);
                var tagFile = File.Create(new StreamFileAbstraction(file,
                    fileStream, fileStream));

                var tags = tagFile.GetTag(TagTypes.Id3v2);
                var model = new Mp3Model()
                {
                    Album = tags.Album,
                    AlbumArtist = tags.AlbumArtists.FirstOrDefault(),
                    Artist = tags.Performers.FirstOrDefault(),
                    Comment = tags.Comment,
                    Genre = tags.Genres.FirstOrDefault(),
                    Title = tags.Title,
                    Year = tags.Year,
                    File = tagFile
                };
                res.Add(model);
            }
            return res;
        }

        public async Task<bool> SaveModelsOfPlaylist(string playlistId, IProgressService service, List<Mp3Model> models, bool deleteOthers = true)
        {
            try
            {
                foreach (var mp3Model in models)
                { 
                    //to avoid null reference exception
                    if (mp3Model.Genre == null)
                        mp3Model.Genre = "";
                    if (mp3Model.AlbumArtist == null)
                        mp3Model.AlbumArtist = "";
                    if (mp3Model.Artist == null)
                        mp3Model.Artist = "";

                    mp3Model.File.Tag.Album = mp3Model.Album;
                    mp3Model.File.Tag.AlbumArtists = new[] { mp3Model.AlbumArtist };
                    mp3Model.File.Tag.Performers = new[] { mp3Model.Artist };
                    mp3Model.File.Tag.Title = mp3Model.Title;
                    mp3Model.File.Tag.Comment = mp3Model.Comment;
                    mp3Model.File.Tag.Genres = new[] { mp3Model.Genre };
                    mp3Model.File.Tag.Year = mp3Model.Year;
                    if (mp3Model.AlbumCover != null)
                    {
                        var picture = await DownloadService.GetAlbumArt(mp3Model.AlbumCover);
                        if (picture != null)
                        {
                            mp3Model.File.Tag.Pictures = new IPicture[1] { picture };
                        }
                    }
                    mp3Model.File.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }
    }
}
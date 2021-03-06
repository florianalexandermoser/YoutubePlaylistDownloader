﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.UniversalWindows.Platform;
using Famoser.YoutubePlaylistDownloader.Business.Enums;
using Famoser.YoutubePlaylistDownloader.Business.Helpers;
using Famoser.YoutubePlaylistDownloader.Business.Services.Interfaces;
using File = TagLib.File;

namespace Famoser.YoutubePlaylistDownloader.Presentation.UniversalWindows.Platform
{
    public class FolderStorageService : StorageService, IFolderStorageService
    {
        public async Task<bool> MoveFile(FolderType type, string path, string newPath)
        {
            try
            {
                var fileName = Path.GetFileName(path);
                var folder = Path.GetDirectoryName(path);

                StorageFile storageFile = await (await GetFolder(type).GetFolderAsync(folder)).GetFileAsync(fileName);

                var newFileName = Path.GetFileName(newPath);
                var newFolder = Path.GetDirectoryName(newPath);

                if (folder == newFolder)
                    await storageFile.RenameAsync(newFileName, NameCollisionOption.ReplaceExisting);
                else
                {
                    var newStoragefolder = await GetFolder(type).CreateFolderAsync(folder, CreationCollisionOption.OpenIfExists);
                    await storageFile.MoveAsync(newStoragefolder, newFileName);
                }
                return true;

            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }

        public async Task<List<string>> GetAllFilesFromFolder(FolderType type, string folder)
        {
            try
            {
                var files = await (await GetFolder(type).GetFolderAsync(folder)).GetFilesAsync();

                return files.Select(storageFile => storageFile.Name).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return new List<string>();
        }

        public async Task<Stream> GetFile(FolderType type, string path)
        {
            try
            {
                var fileName = Path.GetFileName(path);
                var folder = Path.GetDirectoryName(path);

                StorageFile storageFile = await (await GetFolder(type).GetFolderAsync(folder)).GetFileAsync(fileName);

                var randomAccessStream = await storageFile.OpenAsync(FileAccessMode.Read);
                return randomAccessStream.AsStreamForRead();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return null;
        }

        public async Task<File.IFileAbstraction> GetTagLibFile(FolderType type, string path)
        {
            try
            {
                var fileName = Path.GetFileName(path);
                var folder = Path.GetDirectoryName(path);

                StorageFile storageFile = await (await GetFolder(type).GetFolderAsync(folder)).GetFileAsync(fileName);

                var sfa = new StreamFileAbstraction(storageFile);
                await sfa.Initialize();
                return sfa;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return null;
        }

        public async Task<bool> SaveTagLibFile(File.IFileAbstraction abstraction)
        {
            try
            {
                var abstr = abstraction as StreamFileAbstraction;
                if (abstr != null)
                {
                    await FileIO.WriteBytesAsync(abstr.File, StreamHelper.StreamToByte(abstr.WriteStream));
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }

        public async Task<bool> DeleteFilesInFolder(FolderType type, string folder, List<string> files)
        {
            try
            {
                var storageFiles = await (await GetFolder(type).GetFolderAsync(folder)).GetFilesAsync();
                foreach (var storageFile in storageFiles)
                {
                    if (files.Any(f => f == storageFile.Name))
                        await storageFile.DeleteAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }

        public async Task<bool> SaveFile(FolderType type, string path, Stream stream)
        {
            try
            {
                var fileName = Path.GetFileName(path);
                var folder = Path.GetDirectoryName(path);
                var storageFolder = GetFolder(type);

                var storageSubFolder = await storageFolder.CreateFolderAsync(folder, CreationCollisionOption.OpenIfExists);

                var si = await storageSubFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteBytesAsync(si, StreamHelper.StreamToByte(stream));
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
            return false;
        }

        private StorageFolder GetFolder(FolderType folder)
        {
            if (folder == FolderType.Music)
                return KnownFolders.MusicLibrary;
            return ApplicationData.Current.LocalCacheFolder;
        }
    }
}

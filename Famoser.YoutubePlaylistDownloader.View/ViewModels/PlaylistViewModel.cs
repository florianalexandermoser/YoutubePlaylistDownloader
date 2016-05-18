﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.YoutubePlaylistDownloader.Business.Models;
using Famoser.YoutubePlaylistDownloader.Business.Repositories.Interfaces;
using Famoser.YoutubePlaylistDownloader.View.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Famoser.YoutubePlaylistDownloader.View.ViewModels
{
    public class PlaylistViewModel : ViewModelBase
    {
        private readonly IPlaylistRepository _playlistRepository;
        private IProgressService _progressService;

        public PlaylistViewModel(IPlaylistRepository playlistRepository, IProgressService progressService)
        {
            _playlistRepository = playlistRepository;
            _progressService = progressService;

            _startDownload = new RelayCommand(StartDownload, () => CanExecuteStartDownloadCommand);
            _refreshPlaylist = new RelayCommand(RefreshPlaylist, () => CanExecuteRefreshPlaylistCommand);

            Messenger.Default.Register<PlaylistModel>(this, Messages.Select, EvaluateSelectMessage);

            if (IsInDesignMode)
            {
                SelectedPlaylist = _playlistRepository.GetDesignCollection()[0];
            }
        }

        private void EvaluateSelectMessage(PlaylistModel obj)
        {
            SelectedPlaylist = obj;
        }

        private readonly RelayCommand _refreshPlaylist;
        public ICommand RefreshPlaylistCommand => _refreshPlaylist;

        public bool CanExecuteRefreshPlaylistCommand => !_refreshPlaylistActive;

        private bool _refreshPlaylistActive;
        public async void RefreshPlaylist()
        {
            _refreshPlaylistActive = true;
            _refreshPlaylist.RaiseCanExecuteChanged();

            //todo: check if already actualizing playlist
            SelectedPlaylist.ProgressServie = new ProgressService();
            await _playlistRepository.RefreshPlaylist(SelectedPlaylist, SelectedPlaylist.ProgressServie);

            _refreshPlaylistActive = false;
            _refreshPlaylist.RaiseCanExecuteChanged();
        }

        private readonly RelayCommand _startDownload;
        public ICommand StartDownloadCommand => _startDownload;

        public bool CanExecuteStartDownloadCommand => SelectedPlaylist != null && !_startDownloadActive;

        private bool _startDownloadActive;
        public async void StartDownload()
        {
            _startDownloadActive = true;
            _startDownload.RaiseCanExecuteChanged();

            SelectedPlaylist.ProgressServie = new ProgressService();
            await _playlistRepository.DownloadVideosForPlaylist(SelectedPlaylist, SelectedPlaylist.ProgressServie);

            _startDownloadActive = false;
            _startDownload.RaiseCanExecuteChanged();
        }

        private PlaylistModel _selectedPlaylist;
        public PlaylistModel SelectedPlaylist
        {
            get { return _selectedPlaylist; }
            set { Set(ref _selectedPlaylist, value); }
        }
    }
}

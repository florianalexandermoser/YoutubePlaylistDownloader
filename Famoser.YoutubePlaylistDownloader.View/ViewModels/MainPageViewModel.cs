﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Commands;
using Famoser.YoutubePlaylistDownloader.Business.Models;
using Famoser.YoutubePlaylistDownloader.Business.Repositories.Interfaces;
using Famoser.YoutubePlaylistDownloader.View.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Famoser.YoutubePlaylistDownloader.View.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IProgressService _progressService;

        private readonly IHistoryNavigationService _historyNavigationService;

        public MainPageViewModel(IPlaylistRepository playlistRepository, IProgressService progressService, IHistoryNavigationService historyNavigationService)
        {
            _playlistRepository = playlistRepository;
            _progressService = progressService;
            _historyNavigationService = historyNavigationService;

            _refreshPlaylists = new RelayCommand(RefreshPlaylist, () => CanExecuteRefreshPlaylistsCommand);
            _startDownload = new RelayCommand(StartDownload, () => CanExecuteStartDownloadCommand);
            _addToPlaylistsCommand = new RelayCommand(AddToPlaylist, () => CanExecuteAddToPlaylistCommand);
            _selectPlaylist = new RelayCommand<PlaylistModel>(SelectPlaylist);

            Playlists = _playlistRepository.GetPlaylists();

            if (!IsInDesignMode)
                RefreshPlaylist();
        }

        public ProgressService ProgressService => _progressService as ProgressService;

        private readonly RelayCommand _refreshPlaylists;
        public ICommand RefreshPlaylistsCommand => _refreshPlaylists;

        private bool CanExecuteRefreshPlaylistsCommand => !_prozessActive;

        public async void RefreshPlaylist()
        {
            using (new IndeterminateProgressDisposable<IndeterminateProgressKey>(_refreshPlaylists, b => _prozessActive = b, IndeterminateProgressKey.RefreshingPlaylists, _progressService))
            {
                _startDownload.RaiseCanExecuteChanged();

                await _playlistRepository.RefreshAllPlaylists(_progressService);
            }
            _startDownload.RaiseCanExecuteChanged();
        }

        private readonly RelayCommand _startDownload;
        public ICommand StartDownloadCommand => _startDownload;

        private bool CanExecuteStartDownloadCommand => Playlists != null && Playlists.Any() && !_prozessActive;

        private bool _prozessActive;
        public async void StartDownload()
        {
            using (new IndeterminateProgressDisposable<IndeterminateProgressKey>(_startDownload, b => _prozessActive = b, IndeterminateProgressKey.StartingDownload, _progressService))
            {
                _refreshPlaylists.RaiseCanExecuteChanged();

                await _playlistRepository.DownloadVideosForAllPlaylists(_progressService);
            }
            _refreshPlaylists.RaiseCanExecuteChanged();
        }


        private string _playListLink;
        public string PlaylistLink
        {
            get { return _playListLink; }
            set
            {
                if (Set(ref _playListLink, value))
                    _addToPlaylistsCommand.RaiseCanExecuteChanged();
            }
        }

        private readonly RelayCommand _addToPlaylistsCommand;
        public ICommand AddToPlaylistCommand => _addToPlaylistsCommand;

        private bool CanExecuteAddToPlaylistCommand => !string.IsNullOrEmpty(PlaylistLink) && !_isAddingPlaylist;

        private bool _isAddingPlaylist;
        public async void AddToPlaylist()
        {
            using (new IndeterminateProgressDisposable<IndeterminateProgressKey>(_addToPlaylistsCommand, b => _isAddingPlaylist = b, IndeterminateProgressKey.AddingToPlaylist, _progressService))
            {
                await _playlistRepository.AddNewPlaylistByLink(PlaylistLink);
                PlaylistLink = null;
            }
        }

        private readonly RelayCommand<PlaylistModel> _selectPlaylist;
        public ICommand SelectPlaylistCommand => _selectPlaylist;

        public void SelectPlaylist(PlaylistModel model)
        {
            _historyNavigationService.NavigateTo(PageKeys.Playlist.ToString());
            Messenger.Default.Send(model, Messages.Select);
        }

        private ObservableCollection<PlaylistModel> _playlists;
        public ObservableCollection<PlaylistModel> Playlists
        {
            get { return _playlists; }
            set
            {
                if (Set(ref _playlists, value))
                    _startDownload.RaiseCanExecuteChanged();
            }
        }
    }
}

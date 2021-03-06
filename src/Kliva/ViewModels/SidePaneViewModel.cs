﻿using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Kliva.Models;
using System.Linq;
using System.Threading.Tasks;
using Kliva.Services.Interfaces;

namespace Kliva.ViewModels
{
    public class SidePaneViewModel : KlivaBaseViewModel
    {
        private readonly List<Type> _noSidePane = new List<Type>
        {
            typeof(LoginPage)
        };

        private Type _pageType;
        private readonly ISettingsService _settingsService;

        private bool _isPaneOpen = false;
        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set { Set(() => IsPaneOpen, ref _isPaneOpen, value); }
        }

        private SplitViewDisplayMode _displayMode = SplitViewDisplayMode.CompactOverlay;
        public SplitViewDisplayMode DisplayMode
        {
            get { return _displayMode; }
            set { Set(() => DisplayMode, ref _displayMode, value); }
        }

        private ObservableCollection<MenuItem> _topMenuItems = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> TopMenuItems
        {
            get { return _topMenuItems; }
            set { Set(() => TopMenuItems, ref _topMenuItems, value); }
        }

        private ObservableCollection<MenuItem> _bottomMenuItems = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> BottomMenuItems
        {
            get { return _bottomMenuItems; }
            set { Set(() => BottomMenuItems, ref _bottomMenuItems, value); }
        }

        private MenuItem _selectedTopMenuItem;
        public MenuItem SelectedTopMenuItem
        {
            get { return _selectedTopMenuItem; }
            set
            {
                if (Set(() => SelectedTopMenuItem, ref _selectedTopMenuItem, value))
                {
                    if (value != null)
                    {
                        if (IsPaneOpen)
                            IsPaneOpen = !IsPaneOpen;

                        switch (value.MenuItemType)
                        {
                            case MenuItemType.Home:
                                HomeCommand.Execute(null);
                                break;

                            case MenuItemType.Statistics:
                                StatisticsCommand.Execute(null);
                                break;

                            case MenuItemType.Profile:
                                ProfileCommand.Execute(null);
                                break;

                            case MenuItemType.Clubs:
                                ClubsCommand.Execute(null);
                                break;
                        }
                    }
                }
            }
        }

        private MenuItem _selectedBottomMenuItem;
        public MenuItem SelectedBottomMenuItem
        {
            get { return _selectedBottomMenuItem; }
            set
            {
                if (Set(() => SelectedBottomMenuItem, ref _selectedBottomMenuItem, value))
                {
                    if (value != null)
                    {
                        switch (value.MenuItemType)
                        {
                            case MenuItemType.LogOut:
                                LogoutCommand.Execute(null);
                                break;
                            case MenuItemType.Settings:
                                SettingsCommand.Execute(null);
                                break;
                            case MenuItemType.Empty:
                                HamburgerCommand.Execute(null);
                                break;
                        }
                    }
                }                
            }
        }

        private RelayCommand _logoutCommand;
        public RelayCommand LogoutCommand => _logoutCommand ?? (_logoutCommand = new RelayCommand(async () => await Logout()));

        private RelayCommand _homeCommand;
        public RelayCommand HomeCommand => _homeCommand ?? (_homeCommand = new RelayCommand(() => ChangePage<MainPage>()));

        //TODO: Glenn - We hooked this up twice, once in SidePaneViewModel and once in MainViewModel because of difference in UI on desktop ( sidebar ) and mobile ( bottom appbar )
        private RelayCommand _statisticsCommand;
        public RelayCommand StatisticsCommand => _statisticsCommand ?? (_statisticsCommand = new RelayCommand(() => ChangePage<StatsPage>()));

        private RelayCommand _profileCommand;
        public RelayCommand ProfileCommand => _profileCommand ?? (_profileCommand = new RelayCommand(() => ChangePage<ProfilePage>()));

        private RelayCommand _clubsCommand;
        public RelayCommand ClubsCommand => _clubsCommand ?? (_clubsCommand = new RelayCommand(() => ChangePage<ClubsPage>()));

        private RelayCommand _hamburgerCommand;
        public RelayCommand HamburgerCommand => _hamburgerCommand ?? (_hamburgerCommand = new RelayCommand(() => IsPaneOpen = !IsPaneOpen));

        //TODO: Glenn - We hooked this up twice, once in SidePaneViewModel and once in MainViewModel because of difference in UI on desktop ( sidebar ) and mobile ( bottom appbar )
        private RelayCommand _settingsCommand;
        public RelayCommand SettingsCommand => _settingsCommand ?? (_settingsCommand = new RelayCommand(() => ChangePage<SettingsPage>()));

        public SidePaneViewModel(INavigationService navigationService, ISettingsService settingsService) : base(navigationService)
        {
            if (!IsInDesignMode)
            {
                var view = ApplicationView.GetForCurrentView();
                view.VisibleBoundsChanged += OnVisibleBoundsChanged;
            }

            _settingsService = settingsService;

            TopMenuItems.Add(new MenuItem() { Icon = "", Title = "home", MenuItemType = MenuItemType.Home, MenuItemFontType = MenuItemFontType.MDL2 });
            //TopMenuItems.Add(new MenuItem() { Icon = "", Title = "statistics", MenuItemType = MenuItemType.Statistics, MenuItemFontType = MenuItemFontType.MDL2 });
            TopMenuItems.Add(new MenuItem() { Icon = "", Title = "statistics", MenuItemType = MenuItemType.Statistics, MenuItemFontType = MenuItemFontType.Material });
            TopMenuItems.Add(new MenuItem() { Icon = "", Title = "profile", MenuItemType = MenuItemType.Profile, MenuItemFontType = MenuItemFontType.MDL2 });
            TopMenuItems.Add(new MenuItem() { Icon = "", Title = "club", MenuItemType = MenuItemType.Clubs, MenuItemFontType = MenuItemFontType.Material });

            BottomMenuItems.Add(new MenuItem() { Icon = "", Title = "log out", MenuItemType = MenuItemType.LogOut, MenuItemFontType = MenuItemFontType.MDL2 });
            BottomMenuItems.Add(new MenuItem() { Icon = "", Title = "settings", MenuItemType = MenuItemType.Settings, MenuItemFontType = MenuItemFontType.MDL2 });
            BottomMenuItems.Add(new MenuItem() { Icon = "", Title = null, MenuItemType = MenuItemType.Empty, MenuItemFontType = MenuItemFontType.MDL2 });
        }

        private void OnVisibleBoundsChanged(ApplicationView sender, object args)
        {
            ShowHide();
        }

        internal void ShowHide(Type pageType = null)
        {
            //Set current pageType
            if (pageType != null)
                _pageType = pageType;

            bool show = true;

            //TODO: Glenn - Verify this solution with these remarks http://stackoverflow.com/questions/31936154/get-screen-resolution-in-win10-uwp-app
            var actualWidth = ApplicationView.GetForCurrentView().VisibleBounds.Width;
            if (actualWidth < 720)
                show = false;
            else
                show = !_noSidePane.Contains(_pageType);

            if (show)
                DisplayMode = SplitViewDisplayMode.CompactOverlay;
            else
            {
                DisplayMode = SplitViewDisplayMode.Inline;
                IsPaneOpen = false;
            }

            UpdateSelectionForPageType();
        }

        private void UpdateSelectionForPageType()
        {
            if (_pageType == typeof(SettingsPage))
            {
                SelectedBottomMenuItem =
                    BottomMenuItems.Where(i => i.MenuItemType == MenuItemType.Settings).First();
            }
            else
            {
                SelectedBottomMenuItem = null;
            }

            if (_pageType == typeof(ActivityDetailPage) || _pageType == typeof(MainPage))
            {
                SelectedTopMenuItem =
                    TopMenuItems.Where(i => i.MenuItemType == MenuItemType.Home).First();
            }
            else if (_pageType == typeof(ClubDetailPage) || _pageType == typeof(ClubsPage))
            {
                SelectedTopMenuItem =
                    TopMenuItems.Where(i => i.MenuItemType == MenuItemType.Clubs).First();
            }
            else if (_pageType == typeof(ProfilePage) && IsNoneParameter())
            {
                SelectedTopMenuItem =
                    TopMenuItems.Where(i => i.MenuItemType == MenuItemType.Profile).First();
            }
            else if (_pageType == typeof(StatsPage) && IsNoneParameter())
            {
                SelectedTopMenuItem =
                    TopMenuItems.Where(i => i.MenuItemType == MenuItemType.Statistics).First();
            }
            else
            {
                SelectedTopMenuItem = null;
            }
        }

        private void ChangePage<DestinationPageType>()
        {
            // The side pane does not pass a navigation parameter, we can use this to distinguish
            // between a top-level page versus some other page in the hierarchy
            if (typeof(DestinationPageType) != _pageType && IsNoneParameter())
            {
                NavigationService.Navigate<DestinationPageType>();
            }
        }

        private bool IsNoneParameter()
        {
            return string.IsNullOrEmpty(NavigationService.CurrentParameter as string);
        }

        private async Task Logout()
        {
            IsBusy = true;

            await _settingsService.RemoveStravaAccessTokenAsync();

            //Remove the current 'main page' back entry and navigate to the login page
            NavigationService.Navigate<LoginPage>();

            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();

            IsBusy = false;
        }
    }
}

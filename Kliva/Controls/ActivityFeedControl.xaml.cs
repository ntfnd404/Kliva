﻿using Kliva.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Kliva.Controls
{
    public sealed partial class ActivityFeedControl : UserControl
    {
        public ActivityFeedControl()
        {
            this.InitializeComponent();
            DataContextChanged += (sender, args) => this.Bindings.Update();
        }

        private MainViewModel ViewModel => DataContext as MainViewModel;
    }
}

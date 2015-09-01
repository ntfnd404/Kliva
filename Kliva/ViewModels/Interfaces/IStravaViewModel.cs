﻿using Kliva.Models;
using System.Collections.ObjectModel;

namespace Kliva.ViewModels.Interfaces
{
    public interface IStravaViewModel
    {
        ObservableCollection<ActivitySummary> Activities { get; set; }
    }
}

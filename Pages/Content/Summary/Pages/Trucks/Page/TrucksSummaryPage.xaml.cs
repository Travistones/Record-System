using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OxyPlot;
using Record_System.Pages.Content.Summary.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

namespace Record_System.Pages.Content.Summary.Pages.Trucks.Page
{
    public sealed partial class TrucksSummaryPage : Microsoft.UI.Xaml.Controls.Page
    {
        SummaryViewModel viewModel;

        public TrucksSummaryPage()
        {
            viewModel = new();

            this.InitializeComponent();

            Color blackColor50 = (Color)Application.Current.Resources["BlackColor50"];
            TrucksGraphLineSeries.Color = OxyColor.FromArgb(blackColor50.A, blackColor50.R, blackColor50.G, blackColor50.B);
            TrucksGraphPlotModel.PlotAreaBorderThickness = new OxyThickness(0);
        }
    }
}

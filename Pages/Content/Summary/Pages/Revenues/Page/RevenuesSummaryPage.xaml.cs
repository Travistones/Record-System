using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.UI;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Record_System.Pages.Content.Summary.ViewModel;

namespace Record_System.Pages.Content.Summary.Pages.Revenues.Page
{
    public sealed partial class RevenuesSummaryPage : Microsoft.UI.Xaml.Controls.Page
    {
        private SummaryViewModel viewModel;

        public RevenuesSummaryPage()
        {
            viewModel = new();

            this.InitializeComponent();

            RevenuesGraphPlotModel.PlotAreaBorderThickness = new OxyThickness(0);
        }

        private Color salesRevenuesColor = Color.FromArgb(255, 219, 228, 44);
        private Color otherRevenuesColor = Color.FromArgb(255, 219, 55, 185);
    }
}

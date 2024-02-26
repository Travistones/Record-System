using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OxyPlot;
using System;
using Windows.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Record_System.Pages.Content.Summary.ViewModel;

namespace Record_System.Pages.Content.Summary.Pages.Pebbles.Page
{
    public sealed partial class PebblesSummaryPage : Microsoft.UI.Xaml.Controls.Page
    {
        SummaryViewModel viewModel;

        public PebblesSummaryPage()
        {
            viewModel = new();

            this.InitializeComponent();

            PebblesGraphPlotModel.PlotAreaBorderThickness = new OxyThickness(0);
        }

        private Color gradeAColor = Color.FromArgb(255, 219, 228, 44);
        private Color gradeBColor = Color.FromArgb(255, 219, 55, 185);
        private Color gradeCColor = Color.FromArgb(255, 62, 118, 210);
    }
}

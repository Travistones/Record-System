using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Record_System.BackEnd.PlotModel
{
    public class ApplicationPlotModel : OxyPlot.PlotModel
    {
        public ApplicationPlotModel()
        {
            initTimer();
        }

        private Timer timer;

        private void initTimer()
        {
            timer = new();
            timer.Interval = 5000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            App.MainViewModel.DispatcherQueue.TryEnqueue
            (
                Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
                () => 
                {
                    this.InvalidatePlot(true);
                    this.OnUpdated();
                }
            );
        }
    }
}

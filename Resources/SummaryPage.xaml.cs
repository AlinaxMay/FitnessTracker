using Syncfusion.Maui.Charts;
using Microsoft.Maui.Controls;

namespace FitnessTracker
{
    public partial class SummaryPage : ContentPage
    {
        public StepHVModel StepHistory { get; set; }

        public SummaryPage()
        {
            InitializeComponent();

            //  BindingContext
            StepHistory = new StepHVModel();
            BindingContext = StepHistory;
        }
    }
}

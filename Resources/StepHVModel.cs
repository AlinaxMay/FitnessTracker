using System.Collections.ObjectModel;

namespace FitnessTracker;

public class StepHVModel
{
    public ObservableCollection<StepData> StepData { get; set; }

    public StepHVModel()
    {
        StepData = new ObservableCollection<StepData>
        {
            new StepData { Date = "Jan 9", Steps = 3000 },
            new StepData { Date = "Jan 10", Steps = 4500 },
            new StepData { Date = "Jan 11", Steps = 2000 },
        };

        StepData = new ObservableCollection<StepData>(
            StepData.OrderByDescending(s => DateTime.Parse(s.Date))
        );
    }
}

public class StepData
{
    public string Date { get; set; }
    public int Steps { get; set; }
}

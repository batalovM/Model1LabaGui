using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Model1LabaGui.Models;
using ReactiveUI;
using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;

namespace Model1LabaGui.ViewModels;
public class MainWindowViewModel : ViewModelBase
{
    private static List<double> Data { get; set; } = new();
    private double[] _intervals = { 0, 0.268, 0.586, 1, 2 };

    public double[] Intervals
    {
        get => _intervals;
        set => this.RaiseAndSetIfChanged(ref _intervals, value);
    }
    
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<double>
        {
            Values = Data, 
            Fill = new SolidColorPaint(SKColors.Blue) 
        }
    };
    
    public LabelVisual Title { get; set; } = new()
    {
        Text = "Результаты моделирования",
        TextSize = 20,
        Padding = new LiveChartsCore.Drawing.Padding(15),
    };
    
    public ReactiveCommand<Unit, Unit> GenerateChart { get; }

    public MainWindowViewModel()
    {
        GenerateChart = ReactiveCommand.Create(UpdateChart);
    }

    private void UpdateChart()
    {
        Data.Clear();
        var data1 = new List<double>();
        var data = new Approximation(1000).Modeling(data1);
        var first = SelectInterval(data, 0);
        var second = SelectInterval(data, 1);
        var third = SelectInterval(data, 2);
        var fourth = SelectInterval(data, 3);
        var data2 = new List<double>() { first, second, third, fourth };
        Data.AddRange(data2);
        Series = new ISeries[] { new ColumnSeries<double> {Values = Data, Fill = new SolidColorPaint(SKColors.Blue)} };
        this.RaisePropertyChanged(nameof(Data));
        this.RaisePropertyChanged(nameof(Series));
    }
    
    private int SelectInterval(List<double> data, int interval)
    {
        return data.Count(num => num >= _intervals[interval] && num < _intervals[interval + 1]);
    }
    
}

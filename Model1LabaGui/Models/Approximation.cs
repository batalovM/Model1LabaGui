using System;
using System.Collections.Generic;
using System.Linq;
namespace Model1LabaGui.Models;

public class Approximation
{
    private readonly int _size;
    private readonly double[] _intervals = { 0, 0.268, 0.586, 1, 2 };
    public Approximation(int size)
    {
        _size = size;
    }

    public List<double> Modeling(List<double> data)
    {
        for (var i = 0; i < _size; i++)
        {
            var randomXiPlus1 = GenerateRandomNumber();
            var y = GenerateY(randomXiPlus1);
            data.Add(y);
            Console.Write($"y: {y:F3} ");
        }
        KolmogorovTest(data);
        
        return data;
    }
    private double GenerateRandomNumber()
    {
        var random = new Random();
        return random.NextDouble();
    }
    private int SelectInterval(double randomNumber)
    {
        for (var i = 0; i < _intervals.Length - 1; i++)
        {
            if (randomNumber >= _intervals[i] && randomNumber < _intervals[i + 1])
            {
                return i;
            }
        }
        return -1; 
    }
    private double ScaleRandomNumber(double randomNumber, int intervalIndex)
    {
        return (randomNumber - _intervals[intervalIndex]) / (_intervals[intervalIndex + 1] - _intervals[intervalIndex]);
    }
    private double GenerateY(double randomNumber)
    {
        var intervalIndex = SelectInterval(randomNumber);
        var scaledRandomNumber = ScaleRandomNumber(randomNumber, intervalIndex);
        var y = _intervals[intervalIndex] + (_intervals[intervalIndex + 1] - _intervals[intervalIndex]) * scaledRandomNumber;
        return y;
    }
    private double TheoreticalCdf(double x)
    {
        if (x < 0)
        {
            return 0;
        }
        if (x is >= 0 and < 1)
        {
            return x;
        }
        return 1;
    }
    private double EmpiricalCdf(IReadOnlyCollection<double> data, double x)
    {
        var count = data.Count(d => d <= x);
        return (double)count / data.Count;
    }
    private void KolmogorovTest(IReadOnlyCollection<double> data)
    {
        var cdfValues = data.Select(val => EmpiricalCdf(data, val)).ToArray();
        var theoreticalCdfValues = data.Select(TheoreticalCdf).ToArray();
        double d = 0;
        for (var i = 0; i < data.Count; i++)
        {
            var diff1 = Math.Abs(cdfValues[i] - theoreticalCdfValues[i]);
            var diff2 = Math.Abs(theoreticalCdfValues[i] - cdfValues[i]);

            d = Math.Max(d, Math.Max(diff1, diff2));
        }

        const double alpha = 0.05; // Уровень значимости
        var criticalValue = Math.Sqrt(-0.5 * Math.Log(alpha / 2)) / Math.Sqrt(data.Count);

        Console.WriteLine($"\nСтатистика: {d:F3} ");
        Console.WriteLine($"Критическое значение: {criticalValue:F5}");

        Console.WriteLine(d <= criticalValue
            ? "Гипотеза о согласии с законом распределения не отвергается"
            : "Гипотеза о согласии с законом распределения отвергается");
    }
}
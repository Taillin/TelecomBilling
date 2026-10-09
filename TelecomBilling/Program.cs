using System;
using TelecomBilling;
using TelecomProcessor;
using Xunit;

namespace TelecomBilling;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Запуск TelecomBilling ===");
        
        var records = new CallRecord[]
        {
            new("rec1", "KZ", 4.0, false),
            default(CallRecord),
        };

        

        decimal seqTotal = CallPipeline.ProcessCallsSequential(records);
        decimal parTotal = CallPipeline.ProcessCallsParallel(records);

        Console.WriteLine($"Последовательный расчет: {seqTotal} KZT");
        Console.WriteLine($"Параллельный расчет:     {parTotal} KZT");
        Console.WriteLine(seqTotal == parTotal ? "Успех: суммы совпадают!" : "Ошибка: суммы не равны!");
    }
}
using System;
using System.Threading;
using TelecomBilling;

namespace TelecomProcessor;

public static class CallPipeline
{
    public static decimal ProcessCallsSequential(CallRecord[] records)
    {
        if (records == null)
            throw new ArgumentNullException(nameof(records));

        decimal total = 0m;
        foreach (var record in records)
        {
            total += CallPricing.CalculateCost(record);
        }

        return total;
    }

    public static decimal ProcessCallsParallel(CallRecord[] records)
    {
        if (records == null)
            throw new ArgumentNullException(nameof(records));

        if (records.Length == 0)
            return 0m;

        if (records.Length % 2 != 0)
            throw new ArgumentException("Array length must be even.", nameof(records));

        int mid = records.Length / 2;
        CallRecord[] firstHalf = records[..mid];
        CallRecord[] secondHalf = records[mid..];

        decimal[] output1 = new decimal[firstHalf.Length];
        decimal[] output2 = new decimal[secondHalf.Length];

        Exception? thread1Exception = null;
        Exception? thread2Exception = null;

        Thread worker1 = new Thread(() =>
        {
            try
            {
                for (int i = 0; i < firstHalf.Length; i++)
                {
                    output1[i] = CallPricing.CalculateCost(firstHalf[i]);
                }
            }
            catch (Exception ex)
            {
                thread1Exception = ex;
            }
        });

        Thread worker2 = new Thread(() =>
        {
            try
            {
                for (int i = 0; i < secondHalf.Length; i++)
                {
                    output2[i] = CallPricing.CalculateCost(secondHalf[i]);
                }
            }
            catch (Exception ex)
            {
                thread2Exception = ex;
            }
        });

        worker1.Start();
        worker2.Start();

        worker1.Join();
        worker2.Join();

        if (thread1Exception != null)
            throw new AggregateException("Worker 1 failed.", thread1Exception);

        if (thread2Exception != null)
            throw new AggregateException("Worker 2 failed.", thread2Exception);

        decimal totalSum = 0m;
        foreach (decimal cost in output1)
        {
            totalSum += cost;
        }

        foreach (decimal cost in output2)
        {
            totalSum += cost;
        }

        return totalSum;
    }
}
using System;

namespace TelecomBilling;
public readonly record struct CallRecord 
{ 
    public string RecordId { get; }
    public string DestinationCountry { get; }
    public double DurationMinutes { get; }
    public bool IsRoaming { get; }

    public CallRecord(string recordId, string destinationCountry, double durationMinutes, bool isRoaming)
    {
        if (string.IsNullOrWhiteSpace(recordId))
            throw new ArgumentException("RecordId cannot be null, empty or whitespace.", nameof(recordId));

        if (string.IsNullOrWhiteSpace(destinationCountry))
            throw new ArgumentException("DestinationCountry cannot be null, empty or whitespace.", nameof(destinationCountry));

        if (double.IsNaN(durationMinutes) || double.IsInfinity(durationMinutes))
            throw new ArgumentException("Duration must be a finite number.", nameof(durationMinutes));

        if (durationMinutes < 0.0 || durationMinutes > 10_000.0)
            throw new ArgumentOutOfRangeException(nameof(durationMinutes), "Duration must be between 0 and 10,000 minutes.");

        RecordId = recordId;
        DestinationCountry = destinationCountry;
        DurationMinutes = durationMinutes;
        IsRoaming = isRoaming;
    }
}
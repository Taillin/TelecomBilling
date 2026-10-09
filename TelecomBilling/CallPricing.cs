using System;
using TelecomBilling;

namespace TelecomProcessor;

public static class CallPricing
{
    public static decimal CalculateCost(in CallRecord record)
    {
        decimal cost = record switch
        {
            { RecordId: null } or { DestinationCountry: null } =>
                throw new ArgumentException("Invalid default record instance."),

            _ when string.IsNullOrWhiteSpace(record.RecordId) ||
                   string.IsNullOrWhiteSpace(record.DestinationCountry) ||
                   double.IsNaN(record.DurationMinutes) ||
                   double.IsInfinity(record.DurationMinutes) ||
                   record.DurationMinutes < 0.0 ||
                   record.DurationMinutes > 10_000.0 =>
                throw new ArgumentException("Record contains invalid fields or duration."),

            { IsRoaming: true, DestinationCountry: "KZ" } when record.DurationMinutes < 1.0 =>
                50.00m,

            { IsRoaming: false, DestinationCountry: "KZ" } =>
                (decimal)record.DurationMinutes * 15.00m,

            { IsRoaming: true } when record.DurationMinutes >= 10.0 =>
                (decimal)record.DurationMinutes * 120.00m,

            _ => (decimal)record.DurationMinutes * 45.00m
        };

        return Math.Round(cost, 2, MidpointRounding.AwayFromZero);
    }
}

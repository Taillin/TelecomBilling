 Multithreaded Telecom Call Processing Pipeline

Course: FP 3222 – Introduction to Functional Programming  
Platform: C# 12 / .NET 8  
Assignment Assignment 2  
Name: Yermakov Ilya

 1. How to Build and Run Tests

1. Open your terminal in the solution root folder.
2. Build the solution:
   dotnet build
3. Run all automated tests:
   dotnet test

2. Architecture Overview

CallRecord: An immutable value type (readonly record struct) representing a single call (ID, country, minutes, roaming status). The constructor validates all fields and define empty strings, negative minutes, NaN, and values over 10,000.

CallPricing.CalculateCost: A pure pricing function. It uses a single switch expression to select the correct tariff and rounds the final cost to two decimal places (KZT).

CallPipeline:
ProcessCallsSequential: Baseline loop running on a single thread.  
ProcessCallsParallel: Splits the array in half and processes both parts simultaneously using two explicit Thread instances

TelecomBilling.Tests: xUnit tests covering tariff rules, invalid inputs, boundary values, and 100-run sequential-parallel equality.

3. Pure vs. Impure Parts
Pure Components:
CallPricing.CalculateCost: Deterministic and side-effect-free. Given the same input, it always returns the exact same result without changing anything outside.
CallRecord: An immutable data model. Once created, its properties cannot be modified.
Impure Components:
CallPipeline.ProcessCallsParallel: Spawns operating system threads, waits with Join(), and writes into local output buffers.

4. Partitioning Rationale (Why Two Output Arrays?)
I use Data Partitioning:
The input array is sliced in half with range syntax: records[..mid] and records[mid..].

We allocate two separate output arrays (decimal[] output1 and decimal[] output2) before starting threads.

Worker 1 only writes to output1, and Worker 2 only writes to output2.

Race conditions are impossible.

No locking overhead or CPU cache fighting (no False Sharing).

Thread.Join() guarantees that the main thread only reads the results after both workers have finished their writes.
using System;

// Strategy interface for speed calculations
public interface ISpeedCalculator
{
    double CalculateSpeed(double distance, double timeHours);
    bool IsWithinSpeedLimit(double speed);
}

// Base Vehicle class with consistent speed calculation
public abstract class Vehicle
{
    protected readonly ISpeedCalculator _speedCalculator;
    public string VehicleId { get; }

    protected Vehicle(string vehicleId, ISpeedCalculator speedCalculator)
    {
        VehicleId = vehicleId;
        _speedCalculator = speedCalculator;
    }

    // Consistent speed calculation method for all vehicle types
    public virtual double CalculateSpeed(double distance, double timeHours)
    {
        return _speedCalculator.CalculateSpeed(distance, timeHours);
    }

    public virtual bool IsSpeedSafe(double speed)
    {
        return _speedCalculator.IsWithinSpeedLimit(speed);
    }
}

// Bike implementation
public class Bike : Vehicle
{
    public Bike(string vehicleId)
        : base(vehicleId, new BikeSpeedCalculator()) { }
}

// Car implementation
public class Car : Vehicle
{
    public Car(string vehicleId)
        : base(vehicleId, new CarSpeedCalculator()) { }
}

// Airplane implementation
public class Airplane : Vehicle
{
    public Airplane(string vehicleId)
        : base(vehicleId, new AirplaneSpeedCalculator()) { }
}

// Speed calculator implementations
public class BikeSpeedCalculator : ISpeedCalculator
{
    private const double SPEED_LIMIT_KMH = 25.0;

    public double CalculateSpeed(double distance, double timeHours)
    {
        return distance / timeHours;
    }

    public bool IsWithinSpeedLimit(double speed)
    {
        return speed <= SPEED_LIMIT_KMH;
    }
}

public class CarSpeedCalculator : ISpeedCalculator
{
    private const double SPEED_LIMIT_KMH = 120.0;

    public double CalculateSpeed(double distance, double timeHours)
    {
        return distance / timeHours;
    }

    public bool IsWithinSpeedLimit(double speed)
    {
        return speed <= SPEED_LIMIT_KMH;
    }
}

public class AirplaneSpeedCalculator : ISpeedCalculator
{
    private const double SPEED_LIMIT_KMH = 900.0;

    public double CalculateSpeed(double distance, double timeHours)
    {
        return distance / timeHours;
    }

    public bool IsWithinSpeedLimit(double speed)
    {
        return speed <= SPEED_LIMIT_KMH;
    }
}

// Usage example demonstrating consistent handling
public class VehicleTrackingSystem
{
    public void ProcessVehicles()
    {
        // All vehicles can be handled consistently
        Vehicle[] vehicles = { new Bike("BIKE001"), new Car("CAR001"), new Airplane("PLANE001") };

        double distance = 100; // km
        double timeHours = 1.5;

        foreach (var vehicle in vehicles)
        {
            // Same method call for all vehicle types
            double speed = vehicle.CalculateSpeed(distance, timeHours);
            bool isSafe = vehicle.IsSpeedSafe(speed);

            Console.WriteLine(
                $"{vehicle.VehicleId}: {speed:F1} km/h - {(isSafe ? "Safe" : "Exceeds limit")}"
            );
        }
    }
}

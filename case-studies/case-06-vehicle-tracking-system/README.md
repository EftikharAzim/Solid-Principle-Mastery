# Vehicle Tracking System

A vehicle tracking system calculates the speed for various vehicle types (e.g., bikes, cars, airplanes). The base Vehicle class defines basic speed-related methods, and subclasses such as Bike, Car, and Airplane may need different speed limit handling. The system should calculate speeds consistently across all vehicle types.

## Problem Solved

How to calculate speeds consistently for various vehicle types (bikes, cars, airplanes) without needing different handling logic for each type.

## Solution

Uses **Strategy Pattern** + **Polymorphism** to achieve consistent interface with vehicle-specific behavior.

## Key Classes

- `Vehicle` - Base class with consistent speed calculation methods
- `ISpeedCalculator` - Strategy interface for speed calculations
- `Bike`, `Car`, `Airplane` - Vehicle implementations with injected strategies

## Usage

```csharp
// All vehicles handled consistently
Vehicle[] vehicles = {
    new Bike("BIKE001"),
    new Car("CAR001"),
    new Airplane("PLANE001")
};

foreach (var vehicle in vehicles)
{
    double speed = vehicle.CalculateSpeed(distance, timeHours);
    bool isSafe = vehicle.IsSpeedSafe(speed);
}
```

## SOLID Principles Applied

- **SRP**: Separate speed calculation from vehicle management
- **OCP**: New vehicle types without modifying existing code
- **LSP**: All vehicles substitutable in tracking system
- **ISP**: Focused speed calculator interface
- **DIP**: Depends on abstractions, not concrete implementations

## Benefits

✅ Consistent processing across all vehicle types  
✅ No conditional logic or type checking required  
✅ Easy to extend with new vehicle types  
✅ Maintainable and testable code structure

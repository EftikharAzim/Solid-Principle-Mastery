# Event Management System - Architecture Design

## 🎯 Business Requirements Recap

- Users can view available events
- Make reservations with seat selection
- Pay for tickets and receive notifications
- Handle cancellations with refunds
- Real-time seat availability updates

## 🏗️ Class Architecture Overview

### Core Entities & Their Single Responsibilities

| Class                   | Primary Responsibility                   | Why Separate?                              |
| ----------------------- | ---------------------------------------- | ------------------------------------------ |
| `User`                  | Manage user profile & authentication     | User data changes independently            |
| `Event`                 | Store event information & business rules | Event details change independently         |
| `Venue`                 | Manage venue layout & capacity           | Venue configuration changes independently  |
| `Seat`                  | Individual seat state & properties       | Seat availability changes frequently       |
| `SeatAllocationService` | Handle seat reservations & availability  | Complex allocation logic needs isolation   |
| `Reservation`           | Manage booking lifecycle & state         | Reservation status has its own workflow    |
| `Payment`               | Process payments & store transactions    | Payment logic changes independently        |
| `RefundProcessor`       | Handle refund requests & business rules  | Refund policies change independently       |
| `Ticket`                | Generate & validate event tickets        | Ticket format/rules change independently   |
| `NotificationService`   | Send communications to users             | Notification channels change independently |

## 🎯 SOLID Principles Demonstrated

| Principle | Implementation                    | Example Classes                             |
| --------- | --------------------------------- | ------------------------------------------- |
| **S**RP   | Single responsibility per class   | `PaymentProcessor` vs `RefundProcessor`     |
| **O**CP   | Extensible via interfaces         | `IEvent` → `Concert`, `Conference`          |
| **L**SP   | Substitutable implementations     | `EmailService` ↔ `SMSService`               |
| **I**SP   | Focused, role-specific interfaces | `IPaymentProcessor`, `INotificationService` |
| **D**IP   | Dependency injection ready        | Services depend on abstractions             |

## 🔄 Key Relationships & Data Flow

### Reservation Flow:

```
User → Event → SeatAllocationService → Reservation → Payment → Ticket → NotificationService
```

### Cancellation Flow:

```
User → Reservation → RefundProcessor → Payment → NotificationService
```

### Real-time Updates:

```
SeatAllocationService ↔ Seat ↔ Venue (bidirectional updates)
```

## 💡 Key Learning Outcomes

### Architecture Decisions

- **Separated Concerns**: Payment ≠ Refund processing (SRP)
- **Interface-Driven**: All services behind abstractions (DIP)
- **Extension Points**: Add new event/venue types without code changes (OCP)
- **Substitutable Services**: Swap implementations at runtime (LSP)

### Real-World Benefits

- **Testability**: Each component easily unit tested
- **Maintainability**: Changes isolated to single classes
- **Flexibility**: Runtime behavior configuration
- **Scalability**: Independent service scaling

## 🔧 Extending the System

### Add New Event Type

```csharp
public class Conference : IEvent
{
    // OCP - extends without modifying existing code
}
```

### Add New Payment Method

```csharp
public class PayPalProcessor : IPaymentProcessor
{
    // LSP - fully substitutable with StripeProcessor
}
```

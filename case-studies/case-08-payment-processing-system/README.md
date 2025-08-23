# Payment Processing System

A payment processing system handles different types of transactions. Credit card payments require additional verification, such as CVV and expiry date checks, while bank transfers need account number validation. Digital wallets only need an email or phone number for verification. The payment interface should not impose methods that are irrelevant to each payment type.

## 🎯 Core Concept

**Problem**: Payment interfaces shouldn't force irrelevant methods on different payment types.

**Solution**: Segregated interfaces where each payment type implements only what it needs.

## 🏗️ Architecture

```
IPaymentProcessor (Common)
├── CreditCardPaymentProcessor
├── BankTransferPaymentProcessor
└── DigitalWalletPaymentProcessor

Validation Interfaces (Segregated)
├── ICreditCardValidator
├── IBankTransferValidator
└── IDigitalWalletValidator
```

## 🚀 Key Features

- **Interface Segregation**: No irrelevant methods forced on classes
- **Polymorphism**: All processors implement `IPaymentProcessor`
- **Result Pattern**: Clean error handling without exceptions
- **Factory Pattern**: Centralized object creation
- **Dependency Injection**: Loose coupling between components

## 💻 Usage

```csharp
// Factory creates any payment type
var processor = PaymentFactory.CreatePaymentProcessor(PaymentType.CreditCard);

// Same interface, different implementations
var result = processor.ProcessPayment(100.00m);

// Clean result handling
if (result.IsSuccess)
    Console.WriteLine($"✅ {result.Message}");
else
    Console.WriteLine($"❌ {result.Message}");
```

## 🧪 Payment Types

| Type               | Validation Requirements                                 |
| ------------------ | ------------------------------------------------------- |
| **Credit Card**    | Card Number, CVV (3-4 digits), Expiry Date (MM/yyyy)    |
| **Bank Transfer**  | Account Number (8-12 digits), Routing Number (9 digits) |
| **Digital Wallet** | Wallet ID (email/phone), OTP (6 digits)                 |

## 📋 Demo Features

1. **Individual Payment Processing** - Test each payment type
2. **Polymorphism Demo** - Same interface, different behaviors
3. **Error Handling Demo** - Validation failures with specific messages
4. **Interactive Menu** - User-friendly console interface

## ✅ SOLID Principles Applied

- **S**RP: Each class has one responsibility
- **O**CP: Extensible without modifying existing code
- **L**SP: All processors are interchangeable
- **I**SP: ⭐ **Focus** - Segregated interfaces, no irrelevant methods
- **D**IP: Depends on abstractions, not concretions

## 🔧 Running the Application

1. Clone the repository
2. Compile and run the console application
3. Follow the interactive menu
4. Observe ISP in action!

## 📚 Learning Outcomes

- Master Interface Segregation Principle
- Understand dependency injection patterns
- Learn result-based error handling
- Experience polymorphism in practice
- Apply factory pattern for object creation

## 🎓 Perfect for

- Learning SOLID principles
- Understanding clean architecture
- Practicing design patterns
- Building maintainable software
- Interview preparation

# 🛒 E-commerce Discount System

> **SOLID Principle:** Open/Closed Principle (OCP) Implementation  
> **Pattern:** Configuration-Driven Rule Engine  
> **Language:** C# Console Application

## 🎯 Problem Statement

An e-commerce platform needs a flexible discount system that:

- Supports multiple discount types (seasonal, loyalty, bulk purchase, first-time customer)
- Allows marketing teams to add new discount types frequently
- Applies multiple discounts intelligently without modifying existing code
- Provides detailed audit trails of applied discounts

**Challenge:** How do you design a system that's **open for extension** but **closed for modification**?

## 💡 Solution Overview

This project demonstrates a **production-ready implementation** of the Open/Closed Principle using:

### Core Patterns Applied

- **OCP Compliance:** New discount types added without modifying existing code
- **Configuration-Driven Rules:** Business logic separated from business rules
- **Composite Pattern:** Multiple discounts applied seamlessly
- **Factory Pattern:** Intelligent discount chain creation
- **Rich Domain Models:** Comprehensive context and result objects

### Architecture

```
Client → DiscountCalculatorService → DiscountFactory → CompositeDiscount → Individual Discounts
```

## 🚀 Quick Start

### Run the Demo

```csharp
dotnet run
```

### Expected Output

```
🛒 E-Commerce Discount System - SOLID OCP Demo

============================================================
Test Case 1: VIP Customer - Holiday Season - Bulk Purchase
============================================================
🔍 Analyzing discount eligibility...
Customer: VIP | Quantity: 15 | Amount: $1000

✓ Rule matched: VIP Loyalty Program
✓ Rule matched: Bulk Purchase Incentive
✓ Rule matched: Holiday Season Sale

💰 Discount Calculation Results:
Original Price: $1000.00
Total Discount: $336.00
Final Price: $664.00
Applied Discounts: VIP Loyalty Discount, Bulk Purchase Discount, Holiday Seasonal Discount
```

## 📁 Project Structure

```
ECommerceDiscountSystem/
├── Interfaces/
│   └── IDiscount.cs                 # Core abstraction
├── Models/
│   ├── DiscountContext.cs           # Rich context object
│   └── DiscountResult.cs            # Comprehensive result object
├── Discounts/
│   ├── BaseDiscount.cs              # Common functionality
│   ├── SeasonalDiscount.cs          # Seasonal promotions
│   ├── LoyaltyDiscount.cs           # Customer loyalty rewards
│   ├── BulkPurchaseDiscount.cs      # Volume-based discounts
│   ├── FirstTimeCustomerDiscount.cs # Welcome discounts
│   └── CompositeDiscount.cs         # Multiple discount handler
├── Configuration/
│   ├── DiscountRule.cs              # Rule definition
│   └── DiscountConfiguration.cs     # Business rules
├── Services/
│   ├── DiscountFactory.cs           # Smart chain creation
│   └── DiscountCalculatorService.cs # Main service
└── Program.cs                       # Demo application
```

## 🔧 Key Features

### ✅ OCP Compliance

- **Adding New Discount:** Create class implementing `IDiscount` ✨
- **No Code Changes:** Existing discounts remain untouched
- **Risk-Free Deployment:** New features don't affect working code

### ✅ Configuration-Driven Rules

```csharp
// Add new discount rule without coding
new DiscountRule
{
    Name = "Student Discount",
    Condition = ctx => ctx.CustomerType == CustomerType.Student,
    CreateDiscount = () => new LoyaltyDiscount(0.15m, CustomerType.Student)
}
```

### ✅ Intelligent Discount Combination

- **Sequential Application:** Each discount applies to remaining amount
- **Business Logic:** Realistic discount stacking behavior
- **Audit Trail:** Complete visibility into applied discounts

### ✅ Rich Information

- **Detailed Results:** Discount amounts, final prices, applied rules
- **Context Awareness:** Customer type, purchase history, seasonality
- **Extensible Models:** Easy to add new properties without breaking changes

## 🎓 Learning Outcomes

This project demonstrates mastery of:

### Design Principles

- **Open/Closed Principle:** Extension without modification
- **Single Responsibility:** Each discount handles one concern
- **Dependency Inversion:** Depend on abstractions, not concretions

### Design Patterns

- **Strategy Pattern:** Different discount calculation algorithms
- **Composite Pattern:** Combining multiple discounts
- **Factory Pattern:** Creating appropriate discount chains
- **Configuration Pattern:** Business rules as data, not code

### Enterprise Concepts

- **Rule Engines:** Condition-action pattern implementation
- **Business Logic Separation:** Rules vs algorithms
- **Extensible Architecture:** Future-proof system design
- **Domain Modeling:** Rich objects representing business concepts

## 🧪 Test Scenarios

### Test Case 1: VIP Customer - Multiple Discounts

```csharp
var context = new DiscountContext
{
    OriginalPrice = 1000m,
    CustomerType = CustomerType.VIP,      // 20% loyalty
    PurchaseQuantity = 15,                // 8% bulk discount
    PurchaseDate = new DateTime(2024, 12, 15), // 12% holiday sale
    IsFirstTimeCustomer = false
};
// Expected: ~33% total discount with sequential application
```

### Test Case 2: First-Time Customer

```csharp
var context = new DiscountContext
{
    OriginalPrice = 250m,
    CustomerType = CustomerType.Regular,
    IsFirstTimeCustomer = true,           // 15% welcome discount
    PurchaseDate = new DateTime(2024, 7, 10), // 5% summer sale
    PurchaseQuantity = 3
};
// Expected: ~19% total discount
```

### Test Case 3: No Applicable Discounts

```csharp
var context = new DiscountContext
{
    OriginalPrice = 100m,
    CustomerType = CustomerType.Regular,
    IsFirstTimeCustomer = false,
    PurchaseQuantity = 2,
    PurchaseDate = new DateTime(2024, 3, 15) // No seasonal promotion
};
// Expected: No discounts applied
```

## 🚀 Extending the System

### Add New Discount Type

1. **Create discount class:**

```csharp
public class StudentDiscount : BaseDiscount
{
    public override string Name => "Student Discount";

    public override bool IsApplicable(DiscountContext context)
    {
        return context.CustomerType == CustomerType.Student;
    }

    public override DiscountResult ApplyDiscount(DiscountContext context, decimal currentPrice)
    {
        var discountAmount = currentPrice * 0.15m;
        return CreateResult(currentPrice, discountAmount, Name);
    }
}
```

2. **Add configuration rule:**

```csharp
new DiscountRule
{
    Name = "Student Discount Program",
    Priority = 2,
    Condition = ctx => ctx.CustomerType == CustomerType.Student,
    CreateDiscount = () => new StudentDiscount()
}
```

3. **That's it!** ✨ No existing code modified.

### Add New Business Rules

```csharp
// Military discount
Condition = ctx => ctx.CustomerTags.Contains("Military"),

// Regional pricing
Condition = ctx => ctx.Region == "EU" && ctx.PurchaseDate.Month == 11,

// Minimum purchase requirement
Condition = ctx => ctx.OriginalPrice >= 500m && ctx.CustomerType == CustomerType.Premium,
```

## 🎯 Business Benefits

### For Development Teams

- **Reduced Risk:** New features don't break existing functionality
- **Faster Development:** Clear patterns for adding features
- **Easy Testing:** Individual components are isolated and testable
- **Code Reusability:** Discount logic can be shared across applications

### For Business Teams

- **Marketing Agility:** Launch new promotions without development delays
- **A/B Testing:** Easy to test different discount strategies
- **Seasonal Campaigns:** Quickly adapt to market conditions
- **Customer Segmentation:** Flexible rules for different customer types

### For Operations Teams

- **Audit Compliance:** Complete trail of discount applications
- **Performance Monitoring:** Track discount usage and effectiveness
- **Configuration Management:** Business rules version controlled
- **Rollback Capability:** Easy to disable problematic discounts

## 📚 Advanced Concepts Demonstrated

- **Sequential vs Parallel Discount Application**
- **Priority-Based Rule Evaluation**
- **Rich Context Objects for Future-Proofing**
- **Comprehensive Result Objects for Reporting**
- **Configuration-Driven Business Logic**
- **Composite Pattern for Complex Object Combinations**

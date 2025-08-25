# Product-Type managment (ISP)

> **"Clients should not be forced to depend on interfaces they do not use."**

## 🎯 Overview

Demonstration of the **Interface Segregation Principle** using an e-commerce product management system with different product types: Physical, Digital, and Service products.

Problem:
An e-commerce platform manages products with various types, such as physical goods, digital downloads, and services. Physical goods need methods for shipping, inventory tracking, and delivery, while digital products only require download and licensing methods. Service-based products may need scheduling functionality. The product interface should avoid imposing irrelevant methods on different product types.

## 🏗️ Design

### Problem

Different product types need different behaviors:

- **Physical Products**: shipping, inventory, delivery
- **Digital Products**: downloads, licensing
- **Service Products**: scheduling, appointments

### Solution

**Segregated interfaces** based on behavior, not entity type:

```csharp
// Base contract
public interface IProduct
{
    string GetName();
    decimal GetPrice();
    string GetProductInfo();
}

// Behavior-specific interfaces
public interface IShippable { ... }
public interface IDownloadable { ... }
public interface ISchedulable { ... }

// Clean implementations
public class PhysicalProduct : BaseProduct, IShippable { }
public class DigitalProduct : BaseProduct, IDownloadable { }
public class ServiceProduct : BaseProduct, ISchedulable { }
```

## 🚀 Key Benefits

✅ **No forced implementations** - Classes implement only relevant methods  
✅ **Compile-time safety** - Invalid operations prevented by type system  
✅ **LSP compliance** - No `NotImplementedException` anti-patterns  
✅ **Easy extension** - New product types without breaking existing code  
✅ **Single responsibility** - Each interface has focused purpose

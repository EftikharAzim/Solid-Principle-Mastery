# 🎯 Order Processing System

> **Dependency Inversion Principle** demonstration with e-commerce order processing
> An e-commerce system stores order data in a SQL database. Later, there may be a need to switch to a NoSQL database or another storage method for scalability. The high-level order processing logic should remain unaffected by these changes.

## 🚀 **Quick Start**

```csharp
// Switch storage without changing business logic
IOrderRepository repository = new SqlOrderRepository();     // or NoSqlOrderRepository()
var processor = new OrderProcessor(repository);

processor.ProcessNewOrder(userId: 123, items: orderItems);
```

## 🏗️ **Architecture Overview**

```
OrderProcessor → IOrderRepository ← SqlOrderRepository
                                  ← NoSqlOrderRepository
```

**Key Insight**: Business logic depends on abstraction, not concrete implementation.

## 📋 **Core Components**

### **Domain Models**

```csharp
public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> Items { get; set; }
}
```

### **Repository Interface**

```csharp
public interface IOrderRepository
{
    void SaveOrder(Order order);
    Order? GetOrderById(int orderId);
    List<Order> GetOrdersByUserId(int userId);
    void DeleteOrder(int orderId);
}
```

### **Business Logic**

```csharp
public class OrderProcessor
{
    private readonly IOrderRepository _orderRepository;

    public OrderProcessor(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;  // DIP: Depends on abstraction
    }
}
```

## 🔄 **Storage Implementations**

### **SQL Storage**

```csharp
public class SqlOrderRepository : IOrderRepository
{
    public void SaveOrder(Order order)
    {
        // SQL database implementation
    }
}
```

### **NoSQL Storage**

```csharp
public class NoSqlOrderRepository : IOrderRepository
{
    public void SaveOrder(Order order)
    {
        // NoSQL database implementation
    }
}
```

## 🎯 **DIP Benefits**

| ✅ **With DIP**             | ❌ **Without DIP**     |
| --------------------------- | ---------------------- |
| Switch SQL ↔ NoSQL easily   | Rewrite business logic |
| Test with mock repositories | Hard to unit test      |
| Add new storage types       | Modify existing code   |
| Business logic stays stable | Coupled to database    |

## 🧪 **Usage Examples**

### **SQL Implementation**

```csharp
IOrderRepository sqlRepo = new SqlOrderRepository();
var processor = new OrderProcessor(sqlRepo);
processor.ProcessNewOrder(123, orderItems);
```

### **NoSQL Implementation**

```csharp
IOrderRepository noSqlRepo = new NoSqlOrderRepository();
var processor = new OrderProcessor(noSqlRepo);  // Same interface!
processor.ProcessNewOrder(123, orderItems);     // Same method!
```

### **Testing with Mocks**

```csharp
IOrderRepository mockRepo = new MockOrderRepository();
var processor = new OrderProcessor(mockRepo);   // Easy unit testing
```

## 🔧 **Key Features**

- **🔄 Switchable Storage**: SQL ↔ NoSQL without code changes
- **🧪 Testable Design**: Mock repositories for unit testing
- **📦 Clean Separation**: Business logic independent of storage
- **🎯 Single Responsibility**: Each class has one job
- **🔌 Dependency Injection**: Loose coupling via constructor injection

## 📚 **SOLID Principles Applied**

- **🎯 DIP**: High-level modules don't depend on low-level modules
- **🔓 OCP**: Open for extension (new storage), closed for modification
- **📋 SRP**: Each class has single responsibility

## 🚀 **Getting Started**

1. **Clone** the repository
2. **Run** the console application
3. **Observe** same business logic with different storage
4. **Experiment** with new repository implementations

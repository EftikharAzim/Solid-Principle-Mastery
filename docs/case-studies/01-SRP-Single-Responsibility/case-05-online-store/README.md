# Online-Store Demo

An online store needs a backend to handle order placement, track payment, and manage shipment information. Each order involves checking inventory, processing payments, and updating shipment status. The system should also notify customers of order status changes and generate invoices for each order.

## Quick Start

Consider three scenarios:

1. Premium new customer (welcome gift + discount)
2. Regular customer with purchase history
3. Same flow using PayPal to showcase OCP

## Key Concepts (SOLID)

- **SRP**: Each service has a single responsibility.
- **OCP**: Easily swap payment processors without changing core logic.
- **LSP**: Any implementation of payment or notification interfaces works seamlessly.
- **ISP**: Focused interfaces for different notifications and premium features.
- **DIP**: High-level order processing depends on abstractions, injected via constructor.

## Architecture

```
Console UI
    ↓
MasterOrderProcessor
    ↓
Domain Services (Inventory, Payment, Invoice, Shipping, Loyalty, Premium, Notify, Recommend)
    ↓
Domain Model (Customer, Order, Product, PaymentInfo, Shipment)
```

A modular design enabling easy maintenance and extension.

## Extension Points

- Add new payment processors (e.g., ApplePay) without modifying core logic.
- Implement alternative notification services (Slack, WhatsApp).
- Extend shipping with carriers or split services for rates and labels.
- Add premium perks via small dedicated interfaces.
- Swap recommendation engines to improve suggestions.

## Notes

This demo highlights practical SOLID application in e-commerce scenarios, emphasizing maintainability, flexibility, and scalability.

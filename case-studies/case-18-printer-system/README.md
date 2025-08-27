# SOLID Principles: Interface Segregation Principle (ISP)

> "No client should be forced to depend on methods it does not use."

## 🖨️ Case Study: Printer System

### Problem

Different printers have different capabilities:

- **Basic Printer** - Only prints
- **Scanner-Printer** - Prints + Scans
- **All-in-One** - Prints + Scans + Copies + Faxes

How do we design interfaces without forcing simple printers to implement fax functionality?

## 🎯 Overview

This repository demonstrates the **Interface Segregation Principle (ISP)** through a practical printer system example. Learn how to design focused, cohesive interfaces that don't force unnecessary implementations.

## 📖 What You'll Learn

- **ISP Core Concept** - Why fat interfaces are problematic
- **Real-World Application** - Printer system with varying capabilities
- **Best Practices** - How to design clean, maintainable interfaces
- **SOLID Integration** - How ISP connects with other SOLID principles

## ✅ Benefits Demonstrated

| Benefit             | Example                                          |
| ------------------- | ------------------------------------------------ |
| **Flexibility**     | `SimplePrinter` doesn't implement unused methods |
| **Maintainability** | Fax changes don't affect printing logic          |
| **Testability**     | Mock only the interfaces you actually use        |
| **Extensibility**   | Easy to add new printer types                    |

## 🔗 SOLID Connections

- **SRP** - Each interface has single responsibility
- **LSP** - All implementations are properly substitutable
- **OCP** - Easy to extend with new printer types
- **DIP** - Depend on abstractions, not concretions

## 📚 Key Takeaways

- Design focused, cohesive interfaces
- Compose small interfaces rather than inherit large ones
- Think about who will implement your interfaces
- Avoid methods that throw `NotSupportedException`

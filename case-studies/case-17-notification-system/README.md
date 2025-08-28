# Notification System Case Study (Interface Segregation Principle)

## Background

A notification system must send different types of notifications: **SMS**, **Email**, and **Push Notifications**. Each type has unique requirements (e.g., SMS needs a phone number, Email needs an address and supports attachments). The goal is to design interfaces so that each notification type only implements what it needs—following the **Interface Segregation Principle (ISP)**.

## Design Approach

- **INotification**: Core interface for sending notifications.
- **IAttachmentSupport**: For notifications that support attachments (e.g., Email).
- **IDeliveryTracking**: For notifications that support delivery tracking (e.g., SMS, Push).
- **Value Objects**: Specific data models for each notification type.
- **Concrete Classes**: Each notification type implements only relevant interfaces.
- **Builder Pattern**: Used for constructing complex email notifications.

## How ISP Is Applied

- **No Fat Interfaces**: Each notification class only implements interfaces relevant to its features.
- **Extensible**: New notification types can be added without modifying existing interfaces.

## Usage

Run the demo in `Program.cs` to see:

- Sending SMS, Email (with attachments), and Push notifications.
- Checking delivery status for trackable notifications.
- Using the builder pattern for complex emails.
- Bulk notification processing.

## Example

```csharp
var sms = new SmsNotification(new SmsNotificationData("1234567890", "Hello SMS!"));
await notificationService.SendNotificationAsync(sms);

var email = new EmailNotification(new EmailNotificationData("test@example.com", "Subject", "Body"));
if (email is IAttachmentSupport attachmentSupport)
    attachmentSupport.AddAttachment("file.pdf", "Document");
await notificationService.SendNotificationAsync(email);
```

## Principles Demonstrated

- **Interface Segregation Principle (ISP)**
- **Single Responsibility Principle (SRP)**
- **Builder Pattern**

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface INotification
{
    Task<NotificationResult> SendAsync();
}

public interface IAttachmentSupport
{
    void AddAttachment(string filePath, string fileName);
}

public interface IDeliveryTracking
{
    string TrackingId { get; }
    Task<DeliveryStatus> GetDeliveryStatusAsync();
}

// ===== VALUE OBJECTS & RESULTS =====

public record NotificationResult(bool IsSuccess, string Message, string TrackingId = null);

public enum DeliveryStatus
{
    Pending,
    Delivered,
    Failed,
    Unknown
}

// ===== SPECIFIC NOTIFICATION DATA MODELS =====

public record SmsNotificationData(string PhoneNumber, string Message)
{
    public SmsNotificationData() : this(string.Empty, string.Empty) { }
}

public record EmailNotificationData(string EmailAddress, string Subject, string Body)
{
    public EmailNotificationData() : this(string.Empty, string.Empty, string.Empty) { }

    public List<string> CcAddresses { get; init; } = new();
    public List<string> BccAddresses { get; init; } = new();
}

public record PushNotificationData(string DeviceToken, string Message, string Title = "")
{
    public PushNotificationData() : this(string.Empty, string.Empty) { }

    public Dictionary<string, object> CustomData { get; init; } = new();
    public int Badge { get; init; } = 0;
}

// ===== CONCRETE IMPLEMENTATIONS =====

public class SmsNotification : INotification, IDeliveryTracking
{
    private readonly SmsNotificationData _data;
    public string TrackingId { get; private set; }

    public SmsNotification(SmsNotificationData data)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        ValidateData();
        TrackingId = Guid.NewGuid().ToString("N")[..8]; // Short tracking ID
    }

    public async Task<NotificationResult> SendAsync()
    {
        try
        {
            // Simulate SMS sending logic
            Console.WriteLine($"📱 Sending SMS to {_data.PhoneNumber}: {_data.Message}");
            Console.WriteLine($"   Tracking ID: {TrackingId}");

            // Simulate async operation
            await Task.Delay(100);

            return new NotificationResult(true, "SMS sent successfully", TrackingId);
        }
        catch (Exception ex)
        {
            return new NotificationResult(false, $"SMS failed: {ex.Message}");
        }
    }

    public async Task<DeliveryStatus> GetDeliveryStatusAsync()
    {
        await Task.Delay(50);
        return DeliveryStatus.Delivered; // Simplified for demo
    }

    private void ValidateData()
    {
        if (string.IsNullOrWhiteSpace(_data.PhoneNumber))
            throw new ArgumentException("Phone number is required for SMS notification");
        if (string.IsNullOrWhiteSpace(_data.Message))
            throw new ArgumentException("Message is required for SMS notification");
        if (_data.Message.Length > 160)
            throw new ArgumentException("SMS message cannot exceed 160 characters");
    }
}

public class EmailNotification : INotification, IAttachmentSupport
{
    private readonly EmailNotificationData _data;
    private readonly List<(byte[] Content, string FileName)> _attachments = new();

    public EmailNotification(EmailNotificationData data)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        ValidateData();
    }

    public async Task<NotificationResult> SendAsync()
    {
        try
        {
            Console.WriteLine($"📧 Sending Email to {_data.EmailAddress}");
            Console.WriteLine($"   Subject: {_data.Subject}");
            Console.WriteLine($"   Body: {_data.Body}");

            if (_data.CcAddresses.Any())
                Console.WriteLine($"   CC: {string.Join(", ", _data.CcAddresses)}");

            if (_attachments.Any())
                Console.WriteLine($"   Attachments: {_attachments.Count} file(s)");

            // Simulate async operation
            await Task.Delay(200);

            return new NotificationResult(true, "Email sent successfully");
        }
        catch (Exception ex)
        {
            return new NotificationResult(false, $"Email failed: {ex.Message}");
        }
    }

    public void AddAttachment(string filePath, string fileName)
    {
        // In real implementation, read file from filePath
        var content = System.Text.Encoding.UTF8.GetBytes($"Content of {fileName}");
        _attachments.Add((content, fileName));
    }

    private void ValidateData()
    {
        if (string.IsNullOrWhiteSpace(_data.EmailAddress))
            throw new ArgumentException("Email address is required");
        if (string.IsNullOrWhiteSpace(_data.Subject))
            throw new ArgumentException("Subject is required for email notification");
        if (string.IsNullOrWhiteSpace(_data.Body))
            throw new ArgumentException("Body is required for email notification");
    }
}

public class PushNotification : INotification, IDeliveryTracking
{
    private readonly PushNotificationData _data;
    public string TrackingId { get; private set; }

    public PushNotification(PushNotificationData data)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        ValidateData();
        TrackingId = Guid.NewGuid().ToString("N")[..8];
    }

    public async Task<NotificationResult> SendAsync()
    {
        try
        {
            Console.WriteLine($"🔔 Sending Push Notification to device {_data.DeviceToken[..8]}...");
            Console.WriteLine($"   Title: {_data.Title}");
            Console.WriteLine($"   Message: {_data.Message}");
            Console.WriteLine($"   Badge: {_data.Badge}");
            Console.WriteLine($"   Tracking ID: {TrackingId}");

            if (_data.CustomData.Any())
                Console.WriteLine($"   Custom Data: {_data.CustomData.Count} properties");

            await Task.Delay(150);

            return new NotificationResult(true, "Push notification sent successfully", TrackingId);
        }
        catch (Exception ex)
        {
            return new NotificationResult(false, $"Push notification failed: {ex.Message}");
        }
    }

    public async Task<DeliveryStatus> GetDeliveryStatusAsync()
    {
        await Task.Delay(50);
        return DeliveryStatus.Delivered;
    }

    private void ValidateData()
    {
        if (string.IsNullOrWhiteSpace(_data.DeviceToken))
            throw new ArgumentException("Device token is required for push notification");
        if (string.IsNullOrWhiteSpace(_data.Message))
            throw new ArgumentException("Message is required for push notification");
    }
}

// ===== SERVICES =====

public class NotificationService
{
    public async Task<NotificationResult> SendNotificationAsync(INotification notification)
    {
        if (notification == null)
            throw new ArgumentNullException(nameof(notification));

        Console.WriteLine($"\n🚀 Processing notification of type: {notification.GetType().Name}");

        var result = await notification.SendAsync();

        Console.WriteLine($"✅ Result: {(result.IsSuccess ? "SUCCESS" : "FAILED")} - {result.Message}");

        return result;
    }

    public async Task<List<NotificationResult>> SendBulkNotificationsAsync(IEnumerable<INotification> notifications)
    {
        var results = new List<NotificationResult>();

        foreach (var notification in notifications)
        {
            var result = await SendNotificationAsync(notification);
            results.Add(result);
        }

        return results;
    }
}

// ===== BUILDER PATTERN FOR COMPLEX NOTIFICATIONS =====

public class EmailNotificationBuilder
{
    private string _emailAddress;
    private string _subject;
    private string _body;
    private readonly List<string> _ccAddresses = new();
    private readonly List<string> _bccAddresses = new();

    public EmailNotificationBuilder To(string emailAddress)
    {
        _emailAddress = emailAddress;
        return this;
    }

    public EmailNotificationBuilder WithSubject(string subject)
    {
        _subject = subject;
        return this;
    }

    public EmailNotificationBuilder WithBody(string body)
    {
        _body = body;
        return this;
    }

    public EmailNotificationBuilder Cc(params string[] addresses)
    {
        _ccAddresses.AddRange(addresses);
        return this;
    }

    public EmailNotificationBuilder Bcc(params string[] addresses)
    {
        _bccAddresses.AddRange(addresses);
        return this;
    }

    public EmailNotification Build()
    {
        var data = new EmailNotificationData(_emailAddress, _subject, _body)
        {
            CcAddresses = _ccAddresses,
            BccAddresses = _bccAddresses
        };

        return new EmailNotification(data);
    }
}

// ===== DEMONSTRATION =====

public class Program
{
    public static async Task Main(string[] args)
    {
        var notificationService = new NotificationService();

        Console.WriteLine("🎯 ISP-Compliant Notification System Demo\n");

        // 1. Simple SMS
        var sms = new SmsNotification(new SmsNotificationData("1234567890", "Hello SMS!"));
        await notificationService.SendNotificationAsync(sms);

        // 2. Email with attachments (using additional interface)
        var email = new EmailNotification(new EmailNotificationData(
            "test@example.com",
            "Test Email",
            "This is a test email body"
        ));

        // Email implements IAttachmentSupport, so we can use it
        if (email is IAttachmentSupport attachmentSupport)
        {
            attachmentSupport.AddAttachment("document.pdf", "Important Document");
            attachmentSupport.AddAttachment("image.jpg", "Screenshot");
        }

        await notificationService.SendNotificationAsync(email);

        // 3. Push notification with tracking
        var push = new PushNotification(new PushNotificationData(
            "abc123xyz789",
            "New message received!",
            "App Notification"
        )
        { Badge = 5 });

        await notificationService.SendNotificationAsync(push);

        // 4. Check delivery status for trackable notifications
        Console.WriteLine("\n📊 Checking Delivery Status:");
        if (sms is IDeliveryTracking smsTracking)
        {
            var status = await smsTracking.GetDeliveryStatusAsync();
            Console.WriteLine($"SMS Status: {status}");
        }

        // 5. Builder pattern example
        var complexEmail = new EmailNotificationBuilder()
            .To("recipient@example.com")
            .WithSubject("Complex Email")
            .WithBody("This email was built using the builder pattern")
            .Cc("cc1@example.com", "cc2@example.com")
            .Build();

        await notificationService.SendNotificationAsync(complexEmail);

        // 6. Bulk notifications
        Console.WriteLine("\n📦 Bulk Notification Processing:");
        var bulkNotifications = new List<INotification>
        {
            new SmsNotification(new SmsNotificationData("111", "Bulk SMS 1")),
            new SmsNotification(new SmsNotificationData("222", "Bulk SMS 2")),
            new EmailNotification(new EmailNotificationData("bulk@test.com", "Bulk", "Bulk email"))
        };

        var results = await notificationService.SendBulkNotificationsAsync(bulkNotifications);
        Console.WriteLine($"Processed {results.Count} notifications, {results.Count(r => r.IsSuccess)} successful");
    }
}
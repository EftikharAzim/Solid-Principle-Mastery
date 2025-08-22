namespace EventManagementSystem
{
    // ==================== INTERFACES (Dependency Inversion Principle) ====================

    public interface IPaymentProcessor
    {
        PaymentResult ProcessPayment(decimal amount, string paymentMethod);
        bool ValidatePayment(string transactionId);
    }

    public interface IRefundProcessor
    {
        RefundResult ProcessRefund(string transactionId, decimal amount, string reason);
    }

    public interface INotificationService
    {
        void SendNotification(string recipient, string subject, string message);
        bool ValidateRecipient(string recipient);
    }

    public interface IEvent
    {
        string Id { get; }
        string Name { get; }
        DateTime EventDate { get; }
        decimal BasePrice { get; }
        string GetEventDetails();
        bool IsAvailable();
    }

    public interface IVenue
    {
        string Id { get; }
        string Name { get; }
        int Capacity { get; }
        List<Seat> GetAvailableSeats();
        bool ReserveSeat(string seatId);
        void ReleaseSeat(string seatId);
    }

    // ==================== ENUMS ====================

    public enum SeatType
    {
        Regular,
        Premium,
        VIP,
    }

    public enum SeatStatus
    {
        Available,
        Reserved,
        Sold,
    }

    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed,
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded,
    }

    // ==================== RESULT CLASSES ====================

    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public PaymentStatus Status { get; set; }
    }

    public class RefundResult
    {
        public bool IsSuccess { get; set; }
        public string RefundId { get; set; }
        public string Message { get; set; }
        public decimal RefundAmount { get; set; }
    }

    // ==================== ENTITY CLASSES (Single Responsibility) ====================

    // SRP: User manages only user-related data and authentication
    public class User
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public DateTime CreatedDate { get; private set; }

        public User(string name, string email, string phone)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Email = email;
            Phone = phone;
            CreatedDate = DateTime.Now;
        }

        public bool IsValidUser()
        {
            return !string.IsNullOrEmpty(Email) && Email.Contains("@");
        }
    }

    // SRP: Seat manages only individual seat data and status
    public class Seat
    {
        public string Id { get; private set; }
        public string SeatNumber { get; private set; }
        public SeatType Type { get; private set; }
        public SeatStatus Status { get; private set; }
        public decimal PriceMultiplier { get; private set; }

        public Seat(string seatNumber, SeatType type)
        {
            Id = Guid.NewGuid().ToString();
            SeatNumber = seatNumber;
            Type = type;
            Status = SeatStatus.Available;
            PriceMultiplier = GetPriceMultiplier(type);
        }

        private decimal GetPriceMultiplier(SeatType type)
        {
            return type switch
            {
                SeatType.Regular => 1.0m,
                SeatType.Premium => 1.5m,
                SeatType.VIP => 2.0m,
                _ => 1.0m,
            };
        }

        public void Reserve() => Status = SeatStatus.Reserved;

        public void Sell() => Status = SeatStatus.Sold;

        public void Release() => Status = SeatStatus.Available;

        public bool IsAvailable() => Status == SeatStatus.Available;
    }

    // SRP: Concert is a specific implementation of IEvent
    // Open/Closed: We can add new event types without modifying existing code
    public class Concert : IEvent
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public DateTime EventDate { get; private set; }
        public decimal BasePrice { get; private set; }
        public string Artist { get; private set; }
        public string Genre { get; private set; }

        public Concert(
            string name,
            DateTime eventDate,
            decimal basePrice,
            string artist,
            string genre
        )
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            EventDate = eventDate;
            BasePrice = basePrice;
            Artist = artist;
            Genre = genre;
        }

        public string GetEventDetails()
        {
            return $"Concert: {Name} by {Artist}\nGenre: {Genre}\nDate: {EventDate:yyyy-MM-dd HH:mm}\nBase Price: ${BasePrice}";
        }

        public bool IsAvailable() => EventDate > DateTime.Now;
    }

    // SRP: Theater venue implementation
    // Liskov Substitution: Can substitute any IVenue implementation
    public class TheaterVenue : IVenue
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public int Capacity { get; private set; }
        private List<Seat> _seats;

        public TheaterVenue(string name, int capacity)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Capacity = capacity;
            _seats = new List<Seat>();
            InitializeSeats();
        }

        private void InitializeSeats()
        {
            // Create seats with different types
            for (int i = 1; i <= Capacity; i++)
            {
                SeatType type =
                    i <= 10 ? SeatType.VIP
                    : i <= 30 ? SeatType.Premium
                    : SeatType.Regular;
                _seats.Add(new Seat($"T-{i:D3}", type));
            }
        }

        public List<Seat> GetAvailableSeats() => _seats.Where(s => s.IsAvailable()).ToList();

        public bool ReserveSeat(string seatId)
        {
            var seat = _seats.FirstOrDefault(s => s.Id == seatId);
            if (seat != null && seat.IsAvailable())
            {
                seat.Reserve();
                return true;
            }
            return false;
        }

        public void ReleaseSeat(string seatId)
        {
            var seat = _seats.FirstOrDefault(s => s.Id == seatId);
            seat?.Release();
        }
    }

    // SRP: Manages seat allocation logic only
    public class SeatAllocationService
    {
        private readonly IVenue _venue;

        public SeatAllocationService(IVenue venue)
        {
            _venue = venue;
        }

        public List<Seat> FindAvailableSeats(int requestedSeats)
        {
            return _venue.GetAvailableSeats().Take(requestedSeats).ToList();
        }

        public bool AllocateSeats(List<string> seatIds)
        {
            return seatIds.All(seatId => _venue.ReserveSeat(seatId));
        }

        public void ReleaseSeats(List<string> seatIds)
        {
            foreach (var seatId in seatIds)
            {
                _venue.ReleaseSeat(seatId);
            }
        }

        public int GetAvailableSeatsCount()
        {
            return _venue.GetAvailableSeats().Count;
        }
    }

    // SRP: Manages reservation lifecycle only
    public class Reservation
    {
        public string Id { get; private set; }
        public string UserId { get; private set; }
        public string EventId { get; private set; }
        public List<string> SeatIds { get; private set; }
        public decimal TotalAmount { get; private set; }
        public ReservationStatus Status { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? ConfirmedDate { get; private set; }

        public Reservation(string userId, string eventId, List<string> seatIds, decimal totalAmount)
        {
            Id = Guid.NewGuid().ToString();
            UserId = userId;
            EventId = eventId;
            SeatIds = seatIds;
            TotalAmount = totalAmount;
            Status = ReservationStatus.Pending;
            CreatedDate = DateTime.Now;
        }

        public void Confirm()
        {
            Status = ReservationStatus.Confirmed;
            ConfirmedDate = DateTime.Now;
        }

        public void Cancel() => Status = ReservationStatus.Cancelled;

        public void Complete() => Status = ReservationStatus.Completed;

        public bool IsCancellable() =>
            Status == ReservationStatus.Confirmed || Status == ReservationStatus.Pending;
    }

    // SRP: Handles payment processing only
    // Interface Segregation: Implements only payment-related interface
    public class StripePaymentProcessor : IPaymentProcessor
    {
        public PaymentResult ProcessPayment(decimal amount, string paymentMethod)
        {
            // Simulate payment processing
            Console.WriteLine($"Processing ${amount} payment via Stripe using {paymentMethod}...");

            // Simulate success/failure
            bool isSuccess = amount > 0; // Simple validation

            return new PaymentResult
            {
                IsSuccess = isSuccess,
                TransactionId = isSuccess ? $"stripe_{Guid.NewGuid().ToString()[..8]}" : null,
                Message = isSuccess ? "Payment successful" : "Payment failed",
                Status = isSuccess ? PaymentStatus.Completed : PaymentStatus.Failed,
            };
        }

        public bool ValidatePayment(string transactionId)
        {
            return !string.IsNullOrEmpty(transactionId) && transactionId.StartsWith("stripe_");
        }
    }

    // SRP: Handles refund processing only (separate from payment)
    public class RefundProcessor : IRefundProcessor
    {
        private readonly IPaymentProcessor _paymentProcessor;

        public RefundProcessor(IPaymentProcessor paymentProcessor)
        {
            _paymentProcessor = paymentProcessor;
        }

        public RefundResult ProcessRefund(string transactionId, decimal amount, string reason)
        {
            Console.WriteLine($"Processing refund of ${amount} for transaction {transactionId}...");
            Console.WriteLine($"Reason: {reason}");

            if (!_paymentProcessor.ValidatePayment(transactionId))
            {
                return new RefundResult
                {
                    IsSuccess = false,
                    Message = "Invalid transaction ID",
                    RefundAmount = 0,
                };
            }

            // Apply refund policy (e.g., 10% processing fee)
            decimal refundAmount = amount * 0.9m;

            return new RefundResult
            {
                IsSuccess = true,
                RefundId = $"ref_{Guid.NewGuid().ToString()[..8]}",
                Message = "Refund processed successfully",
                RefundAmount = refundAmount,
            };
        }
    }

    // SRP: Handles ticket generation and validation only
    public class Ticket
    {
        public string Id { get; private set; }
        public string ReservationId { get; private set; }
        public string EventId { get; private set; }
        public List<string> SeatIds { get; private set; }
        public DateTime IssuedDate { get; private set; }
        public string QRCode { get; private set; }

        public Ticket(string reservationId, string eventId, List<string> seatIds)
        {
            Id = Guid.NewGuid().ToString();
            ReservationId = reservationId;
            EventId = eventId;
            SeatIds = seatIds;
            IssuedDate = DateTime.Now;
            QRCode = GenerateQRCode();
        }

        private string GenerateQRCode()
        {
            return $"QR_{Id[..8]}_{EventId[..8]}";
        }

        public string GetTicketDetails()
        {
            return $"Ticket ID: {Id}\nEvent: {EventId}\nSeats: {string.Join(", ", SeatIds)}\nIssued: {IssuedDate:yyyy-MM-dd}\nQR: {QRCode}";
        }
    }

    // SRP: Handles email notifications only
    // Open/Closed: New notification types can be added without modification
    public class EmailNotificationService : INotificationService
    {
        public void SendNotification(string recipient, string subject, string message)
        {
            Console.WriteLine($"📧 EMAIL SENT");
            Console.WriteLine($"To: {recipient}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
            Console.WriteLine(new string('=', 50));
        }

        public bool ValidateRecipient(string recipient)
        {
            return !string.IsNullOrEmpty(recipient) && recipient.Contains("@");
        }
    }

    // Alternative implementation - shows Open/Closed principle
    public class SMSNotificationService : INotificationService
    {
        public void SendNotification(string recipient, string subject, string message)
        {
            Console.WriteLine($"📱 SMS SENT");
            Console.WriteLine($"To: {recipient}");
            Console.WriteLine($"Message: {subject} - {message}");
            Console.WriteLine(new string('=', 50));
        }

        public bool ValidateRecipient(string recipient)
        {
            return !string.IsNullOrEmpty(recipient)
                && recipient.All(char.IsDigit)
                && recipient.Length >= 10;
        }
    }

    // ==================== SERVICE ORCHESTRATION ====================

    // SRP: Orchestrates the entire booking process
    // Dependency Inversion: Depends on abstractions, not concretions
    public class EventBookingService
    {
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly IRefundProcessor _refundProcessor;
        private readonly INotificationService _notificationService;
        private readonly SeatAllocationService _seatAllocationService;

        public EventBookingService(
            IPaymentProcessor paymentProcessor,
            IRefundProcessor refundProcessor,
            INotificationService notificationService,
            SeatAllocationService seatAllocationService
        )
        {
            _paymentProcessor = paymentProcessor;
            _refundProcessor = refundProcessor;
            _notificationService = notificationService;
            _seatAllocationService = seatAllocationService;
        }

        public (bool Success, Reservation Reservation, Ticket Ticket) BookEvent(
            User user,
            IEvent eventItem,
            int requestedSeats,
            string paymentMethod
        )
        {
            Console.WriteLine($"🎫 Starting booking process for {user.Name}...");

            // Step 1: Find available seats
            var availableSeats = _seatAllocationService.FindAvailableSeats(requestedSeats);
            if (availableSeats.Count < requestedSeats)
            {
                Console.WriteLine("❌ Not enough seats available");
                return (false, null, null);
            }

            // Step 2: Calculate total amount
            decimal totalAmount = availableSeats.Sum(seat =>
                eventItem.BasePrice * seat.PriceMultiplier
            );

            // Step 3: Create reservation
            var reservation = new Reservation(
                user.Id,
                eventItem.Id,
                availableSeats.Select(s => s.Id).ToList(),
                totalAmount
            );

            // Step 4: Allocate seats
            if (!_seatAllocationService.AllocateSeats(reservation.SeatIds))
            {
                Console.WriteLine("❌ Failed to allocate seats");
                return (false, null, null);
            }

            // Step 5: Process payment
            var paymentResult = _paymentProcessor.ProcessPayment(totalAmount, paymentMethod);
            if (!paymentResult.IsSuccess)
            {
                _seatAllocationService.ReleaseSeats(reservation.SeatIds);
                Console.WriteLine($"❌ Payment failed: {paymentResult.Message}");
                return (false, null, null);
            }

            // Step 6: Confirm reservation
            reservation.Confirm();

            // Step 7: Generate ticket
            var ticket = new Ticket(reservation.Id, eventItem.Id, reservation.SeatIds);

            // Step 8: Send confirmation notification
            _notificationService.SendNotification(
                user.Email,
                "Booking Confirmed!",
                $"Your booking for {eventItem.Name} has been confirmed. Total: ${totalAmount}"
            );

            Console.WriteLine("✅ Booking completed successfully!");
            return (true, reservation, ticket);
        }

        public bool CancelBooking(User user, Reservation reservation, string reason)
        {
            Console.WriteLine($"🚫 Processing cancellation for {user.Name}...");

            if (!reservation.IsCancellable())
            {
                Console.WriteLine("❌ Reservation cannot be cancelled");
                return false;
            }

            // Process refund
            var refundResult = _refundProcessor.ProcessRefund(
                $"payment_{reservation.Id}",
                reservation.TotalAmount,
                reason
            );

            if (!refundResult.IsSuccess)
            {
                Console.WriteLine($"❌ Refund failed: {refundResult.Message}");
                return false;
            }

            // Release seats
            _seatAllocationService.ReleaseSeats(reservation.SeatIds);

            // Cancel reservation
            reservation.Cancel();

            // Send cancellation notification
            _notificationService.SendNotification(
                user.Email,
                "Booking Cancelled",
                $"Your booking has been cancelled. Refund amount: ${refundResult.RefundAmount}"
            );

            Console.WriteLine("✅ Cancellation completed successfully!");
            return true;
        }
    }

    // ==================== CONSOLE APPLICATION ====================

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🎭 SOLID Event Management System Demo");
            Console.WriteLine("=====================================");

            // Setup dependencies (Dependency Injection in real app)
            var paymentProcessor = new StripePaymentProcessor();
            var refundProcessor = new RefundProcessor(paymentProcessor);
            var notificationService = new EmailNotificationService();
            var venue = new TheaterVenue("Grand Theater", 50);
            var seatAllocationService = new SeatAllocationService(venue);

            var bookingService = new EventBookingService(
                paymentProcessor,
                refundProcessor,
                notificationService,
                seatAllocationService
            );

            // Create test data
            var user = new User("John Doe", "john@example.com", "1234567890");
            var concert = new Concert(
                "Rock Festival 2025",
                DateTime.Now.AddDays(30),
                100m,
                "Rock Stars",
                "Rock"
            );

            Console.WriteLine("📋 Event Details:");
            Console.WriteLine(concert.GetEventDetails());
            Console.WriteLine();

            Console.WriteLine(
                $"💺 Available seats: {seatAllocationService.GetAvailableSeatsCount()}"
            );
            Console.WriteLine();

            // Demonstrate booking
            var (success, reservation, ticket) = bookingService.BookEvent(
                user,
                concert,
                3,
                "Credit Card"
            );

            if (success)
            {
                Console.WriteLine("🎫 Ticket Details:");
                Console.WriteLine(ticket.GetTicketDetails());
                Console.WriteLine();

                Console.WriteLine(
                    $"💺 Remaining seats: {seatAllocationService.GetAvailableSeatsCount()}"
                );
                Console.WriteLine();

                // Demonstrate cancellation
                Console.WriteLine("Testing cancellation...");
                Console.WriteLine();
                bookingService.CancelBooking(user, reservation, "Change of plans");

                Console.WriteLine(
                    $"💺 Seats after cancellation: {seatAllocationService.GetAvailableSeatsCount()}"
                );
            }

            Console.WriteLine();
            Console.WriteLine("Demo completed! Press any key to exit...");
            Console.ReadKey();
        }
    }
}

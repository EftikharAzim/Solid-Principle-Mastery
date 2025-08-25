
namespace ISPEcommerceDemo
{
    public interface IProduct
    {
        int GetId();
        string GetName();
        decimal GetPrice();
        string GetDescription();
        string GetCategory();
        string GetProductInfo();
    }

    public interface IShippable
    {
        decimal CalculateShippingCost(string destinationAddress, decimal weight);
        ShippingResult Ship(string shippingAddress, string shippingMethod);
        List<string> GetAvailableShippingMethods();
        DateTime GetEstimatedDeliveryDate(string destination);
    }

    public interface IDownloadable
    {
        DownloadResult StartDownload(string userEmail);
        DownloadResult PauseDownload(string downloadId);
        DownloadResult CancelDownload(string downloadId);
        bool ValidateLicense(string licenseKey);
        long GetFileSize();
        string GenerateSecureDownloadLink(string userEmail, TimeSpan expirationTime);
    }

    public interface ISchedulable
    {
        SchedulingResult Schedule(DateTime preferredDate, TimeSpan duration, Customer customer);
        SchedulingResult Reschedule(string appointmentId, DateTime newDate);
        bool CancelAppointment(string appointmentId);
        List<AvailableTimeSlot> GetAvailableTimeSlots(DateTime date);
        AppointmentDetails GetAppointmentDetails(string appointmentId);
    }

    // ================================
    // SUPPORTING CLASSES
    // ================================

    public class ShippingResult
    {
        public bool IsSuccess { get; set; }
        public string TrackingNumber { get; set; }
        public string Message { get; set; }
        public DateTime EstimatedDelivery { get; set; }
    }

    public class DownloadResult
    {
        public bool IsSuccess { get; set; }
        public string DownloadId { get; set; }
        public string Message { get; set; }
        public DownloadStatus Status { get; set; }
    }

    public class DownloadStatus
    {
        public string DownloadId { get; set; }
        public bool IsDownloading { get; set; }
        public bool IsPaused { get; set; }
        public bool IsCompleted { get; set; }
        public int ProgressPercentage { get; set; }
        public DateTime StartTime { get; set; }
    }

    public class SchedulingResult
    {
        public bool IsSuccess { get; set; }
        public string AppointmentId { get; set; }
        public string Message { get; set; }
        public DateTime ScheduledTime { get; set; }
    }

    public class AvailableTimeSlot
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public decimal Price { get; set; } // Different time slots might have different pricing
    }

    public class AppointmentDetails
    {
        public string AppointmentId { get; set; }
        public DateTime ScheduledTime { get; set; }
        public TimeSpan Duration { get; set; }
        public Customer Customer { get; set; }
        public string Status { get; set; } // Scheduled, Confirmed, Completed, Cancelled
    }

    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }

    // ================================
    // BASE PRODUCT CLASS
    // ================================

    public abstract class BaseProduct : IProduct
    {
        protected int _id;
        protected string _name;
        protected decimal _price;
        protected string _description;
        protected string _category;

        protected BaseProduct(int id, string name, decimal price, string description, string category)
        {
            _id = id;
            _name = name;
            _price = price;
            _description = description;
            _category = category;
        }

        public virtual int GetId() => _id;
        public virtual string GetName() => _name;
        public virtual decimal GetPrice() => _price;
        public virtual string GetDescription() => _description;
        public virtual string GetCategory() => _category;

        public virtual string GetProductInfo()
        {
            return $"{_name} - ${_price:F2} ({_category})";
        }
    }

    // ================================
    // CONCRETE IMPLEMENTATIONS
    // ================================

    public class PhysicalProduct : BaseProduct, IShippable
    {
        private readonly decimal _weight;
        private readonly List<string> _shippingMethods;

        public PhysicalProduct(int id, string name, decimal price, string description,
                              decimal weight, List<string> shippingMethods)
            : base(id, name, price, description, "Physical")
        {
            _weight = weight;
            _shippingMethods = shippingMethods ?? new List<string> { "Standard", "Express" };
        }

        public decimal CalculateShippingCost(string destinationAddress, decimal weight)
        {
            // Simple shipping calculation logic, may use destinationAddress for future complex calculation
            decimal baseCost = 5.00m;
            decimal weightCost = weight * 0.50m;

            // Add distance calculation logic here in real implementation
            return baseCost + weightCost;
        }

        public ShippingResult Ship(string shippingAddress, string shippingMethod)
        {
            if (string.IsNullOrEmpty(shippingAddress))
            {
                return new ShippingResult
                {
                    IsSuccess = false,
                    Message = "Shipping address is required"
                };
            }

            if (!_shippingMethods.Contains(shippingMethod))
            {
                return new ShippingResult
                {
                    IsSuccess = false,
                    Message = "Invalid shipping method"
                };
            }

            // Simulate shipping process
            return new ShippingResult
            {
                IsSuccess = true,
                TrackingNumber = $"TRK{DateTime.Now.Ticks}",
                Message = "Product shipped successfully",
                EstimatedDelivery = DateTime.Now.AddDays(shippingMethod == "Express" ? 1 : 3)
            };
        }

        public List<string> GetAvailableShippingMethods() => new List<string>(_shippingMethods);

        public DateTime GetEstimatedDeliveryDate(string destination)
        {
            // Simplified logic - in real world, this would use destination
            return DateTime.Now.AddDays(3);
        }
    }

    public class DigitalProduct : BaseProduct, IDownloadable
    {
        private readonly long _fileSize;
        private readonly string _licenseType;
        private readonly Dictionary<string, DownloadStatus> _activeDownloads;

        public DigitalProduct(int id, string name, decimal price, string description,
                             long fileSize, string licenseType)
            : base(id, name, price, description, "Digital")
        {
            _fileSize = fileSize;
            _licenseType = licenseType;
            _activeDownloads = new Dictionary<string, DownloadStatus>();
        }

        public DownloadResult StartDownload(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return new DownloadResult { IsSuccess = false, Message = "User email is required" };
            }

            string downloadId = Guid.NewGuid().ToString();
            var downloadStatus = new DownloadStatus
            {
                DownloadId = downloadId,
                IsDownloading = true,
                IsPaused = false,
                IsCompleted = false,
                ProgressPercentage = 0,
                StartTime = DateTime.Now
            };

            _activeDownloads[downloadId] = downloadStatus;

            return new DownloadResult
            {
                IsSuccess = true,
                DownloadId = downloadId,
                Message = "Download started successfully",
                Status = downloadStatus
            };
        }

        public DownloadResult PauseDownload(string downloadId)
        {
            if (!_activeDownloads.ContainsKey(downloadId))
            {
                return new DownloadResult { IsSuccess = false, Message = "Download not found" };
            }

            var status = _activeDownloads[downloadId];
            status.IsDownloading = false;
            status.IsPaused = true;

            return new DownloadResult { IsSuccess = true, Message = "Download paused", Status = status };
        }

        public DownloadResult CancelDownload(string downloadId)
        {
            if (!_activeDownloads.ContainsKey(downloadId))
            {
                return new DownloadResult { IsSuccess = false, Message = "Download not found" };
            }

            _activeDownloads.Remove(downloadId);
            return new DownloadResult { IsSuccess = true, Message = "Download cancelled" };
        }

        public bool ValidateLicense(string licenseKey)
        {
            // Simplified license validation
            return !string.IsNullOrEmpty(licenseKey) && licenseKey.Length > 10;
        }

        public long GetFileSize() => _fileSize;

        public string GenerateSecureDownloadLink(string userEmail, TimeSpan expirationTime)
        {
            // Generate a secure, time-limited download link
            var expiry = DateTime.Now.Add(expirationTime);
            return $"https://secure-downloads.com/{_id}?user={Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(userEmail))}&expires={expiry.Ticks}";
        }
    }

    public class ServiceProduct : BaseProduct, ISchedulable
    {
        private readonly TimeSpan _serviceDuration;
        private readonly Dictionary<string, AppointmentDetails> _appointments;
        private readonly List<AvailableTimeSlot> _availableSlots;

        public ServiceProduct(int id, string name, decimal price, string description,
                             TimeSpan serviceDuration)
            : base(id, name, price, description, "Service")
        {
            _serviceDuration = serviceDuration;
            _appointments = new Dictionary<string, AppointmentDetails>();
            _availableSlots = GenerateAvailableSlots();
        }

        public SchedulingResult Schedule(DateTime preferredDate, TimeSpan duration, Customer customer)
        {
            if (customer == null)
            {
                return new SchedulingResult { IsSuccess = false, Message = "Customer information is required" };
            }

            var availableSlot = _availableSlots.FirstOrDefault(s =>
                s.StartTime.Date == preferredDate.Date &&
                s.StartTime.TimeOfDay == preferredDate.TimeOfDay &&
                s.IsAvailable);

            if (availableSlot == null)
            {
                return new SchedulingResult
                {
                    IsSuccess = false,
                    Message = "Selected time slot is not available"
                };
            }

            string appointmentId = Guid.NewGuid().ToString();
            var appointment = new AppointmentDetails
            {
                AppointmentId = appointmentId,
                ScheduledTime = preferredDate,
                Duration = duration,
                Customer = customer,
                Status = "Scheduled"
            };

            _appointments[appointmentId] = appointment;
            availableSlot.IsAvailable = false;

            return new SchedulingResult
            {
                IsSuccess = true,
                AppointmentId = appointmentId,
                Message = "Appointment scheduled successfully",
                ScheduledTime = preferredDate
            };
        }

        public SchedulingResult Reschedule(string appointmentId, DateTime newDate)
        {
            if (!_appointments.ContainsKey(appointmentId))
            {
                return new SchedulingResult { IsSuccess = false, Message = "Appointment not found" };
            }

            var appointment = _appointments[appointmentId];

            // Free up the old slot
            var oldSlot = _availableSlots.FirstOrDefault(s =>
                s.StartTime == appointment.ScheduledTime);
            if (oldSlot != null)
                oldSlot.IsAvailable = true;

            // Check new slot availability
            var newSlot = _availableSlots.FirstOrDefault(s =>
                s.StartTime == newDate && s.IsAvailable);

            if (newSlot == null)
            {
                return new SchedulingResult
                {
                    IsSuccess = false,
                    Message = "New time slot is not available"
                };
            }

            appointment.ScheduledTime = newDate;
            newSlot.IsAvailable = false;

            return new SchedulingResult
            {
                IsSuccess = true,
                AppointmentId = appointmentId,
                Message = "Appointment rescheduled successfully",
                ScheduledTime = newDate
            };
        }

        public bool CancelAppointment(string appointmentId)
        {
            if (!_appointments.ContainsKey(appointmentId))
                return false;

            var appointment = _appointments[appointmentId];

            // Free up the slot
            var slot = _availableSlots.FirstOrDefault(s =>
                s.StartTime == appointment.ScheduledTime);
            if (slot != null)
                slot.IsAvailable = true;

            appointment.Status = "Cancelled";
            return true;
        }

        public List<AvailableTimeSlot> GetAvailableTimeSlots(DateTime date)
        {
            return _availableSlots
                .Where(s => s.StartTime.Date == date.Date && s.IsAvailable)
                .ToList();
        }

        public AppointmentDetails GetAppointmentDetails(string appointmentId)
        {
            return _appointments.ContainsKey(appointmentId) ? _appointments[appointmentId] : null;
        }

        private List<AvailableTimeSlot> GenerateAvailableSlots()
        {
            var slots = new List<AvailableTimeSlot>();
            var startDate = DateTime.Today.AddDays(1); // Start from tomorrow

            // Generate slots for the next 30 days
            for (int day = 0; day < 30; day++)
            {
                var currentDate = startDate.AddDays(day);

                // Generate slots from 9 AM to 5 PM
                for (int hour = 9; hour < 17; hour++)
                {
                    var slotStart = currentDate.AddHours(hour);
                    var slotEnd = slotStart.Add(_serviceDuration);

                    slots.Add(new AvailableTimeSlot
                    {
                        StartTime = slotStart,
                        EndTime = slotEnd,
                        IsAvailable = true,
                        Price = _price
                    });
                }
            }

            return slots;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Interface Segregation Principle Demo ===");
            Console.WriteLine("ProductType Management System\n");

            // Create different product types
            var physicalProduct = CreatePhysicalProduct();
            var digitalProduct = CreateDigitalProduct();
            var serviceProduct = CreateServiceProduct();

            // Store products in a list (all implement IProduct)
            var products = new List<IProduct> { physicalProduct, digitalProduct, serviceProduct };

            // Demonstrate polymorphism with base interface
            Console.WriteLine("1. ALL PRODUCTS (Common Interface):");
            Console.WriteLine("=====================================");
            foreach (var product in products)
            {
                DisplayProductInfo(product);
            }

            Console.WriteLine("\n2. PHYSICAL PRODUCT OPERATIONS (IShippable):");
            Console.WriteLine("==============================================");
            DemonstrateShippableOperations(physicalProduct);

            Console.WriteLine("\n3. DIGITAL PRODUCT OPERATIONS (IDownloadable):");
            Console.WriteLine("===============================================");
            DemonstrateDownloadableOperations(digitalProduct);

            Console.WriteLine("\n4. SERVICE PRODUCT OPERATIONS (ISchedulable):");
            Console.WriteLine("==============================================");
            DemonstrateSchedulableOperations(serviceProduct);

            Console.WriteLine("\n5. ISP VIOLATION PREVENTION DEMO:");
            Console.WriteLine("==================================");
            DemonstrateISPBenefits(products);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static PhysicalProduct CreatePhysicalProduct()
        {
            return new PhysicalProduct(
                id: 1,
                name: "Laptop",
                price: 999.99m,
                description: "High-performance laptop",
                weight: 2.5m,
                shippingMethods: new List<string> { "Standard", "Express", "Overnight" }
            );
        }

        static DigitalProduct CreateDigitalProduct()
        {
            return new DigitalProduct(
                id: 2,
                name: "Photo Editing Software",
                price: 199.99m,
                description: "Professional photo editing suite",
                fileSize: 2147483648, // 2GB in bytes
                licenseType: "Single User"
            );
        }

        static ServiceProduct CreateServiceProduct()
        {
            return new ServiceProduct(
                id: 3,
                name: "IT Consultation",
                price: 150.00m,
                description: "One-hour IT consultation session",
                serviceDuration: TimeSpan.FromHours(1)
            );
        }

        static void DisplayProductInfo(IProduct product)
        {
            Console.WriteLine($"Name: {product.GetName()}");
            Console.WriteLine($"Price: {product.GetPrice():C}");
            Console.WriteLine($"Product: {product.GetProductInfo()}");
            Console.WriteLine($"Category: {product.GetCategory()}");
            Console.WriteLine($"Description: {product.GetDescription()}");
            Console.WriteLine();
        }

        static void DemonstrateShippableOperations(PhysicalProduct product)
        {
            // Only PhysicalProduct implements IShippable
            IShippable shippableProduct = product; // This is safe because PhysicalProduct implements IShippable

            Console.WriteLine($"Operating on: {product.GetName()}");
            Console.WriteLine();

            // Calculate shipping cost
            var shippingCost = shippableProduct.CalculateShippingCost("123 Main St, New York, NY", 2.5m);
            Console.WriteLine($"Shipping Cost to New York: ${shippingCost:F2}");

            // Get available shipping methods
            var methods = shippableProduct.GetAvailableShippingMethods();
            Console.WriteLine($"Available Shipping Methods: {string.Join(", ", methods)}");

            // Get estimated delivery date
            var deliveryDate = shippableProduct.GetEstimatedDeliveryDate("New York, NY");
            Console.WriteLine($"Estimated Delivery: {deliveryDate:yyyy-MM-dd}");

            // Ship the product
            var shippingResult = shippableProduct.Ship("123 Main St, New York, NY 10001", "Express");
            Console.WriteLine($"Shipping Status: {(shippingResult.IsSuccess ? "Success" : "Failed")}");
            Console.WriteLine($"Message: {shippingResult.Message}");
            if (shippingResult.IsSuccess)
            {
                Console.WriteLine($"Tracking Number: {shippingResult.TrackingNumber}");
                Console.WriteLine($"Estimated Delivery: {shippingResult.EstimatedDelivery:yyyy-MM-dd HH:mm}");
            }
        }

        static void DemonstrateDownloadableOperations(DigitalProduct product)
        {
            // Only DigitalProduct implements IDownloadable
            IDownloadable downloadableProduct = product; // Safe cast

            Console.WriteLine($"Operating on: {product.GetName()}");
            Console.WriteLine();

            // Check file size
            var fileSize = downloadableProduct.GetFileSize();
            Console.WriteLine($"File Size: {fileSize / (1024 * 1024 * 1024):F2} GB");

            // Validate license
            var isValidLicense = downloadableProduct.ValidateLicense("VALID-LICENSE-KEY-12345");
            Console.WriteLine($"License Validation: {(isValidLicense ? "Valid" : "Invalid")}");

            // Generate secure download link
            var downloadLink = downloadableProduct.GenerateSecureDownloadLink("user@example.com", TimeSpan.FromHours(24));
            Console.WriteLine($"Secure Download Link: {downloadLink}");

            // Start download
            var downloadResult = downloadableProduct.StartDownload("user@example.com");
            Console.WriteLine($"Download Start: {(downloadResult.IsSuccess ? "Success" : "Failed")}");
            Console.WriteLine($"Message: {downloadResult.Message}");

            if (downloadResult.IsSuccess)
            {
                Console.WriteLine($"Download ID: {downloadResult.DownloadId}");
                Console.WriteLine($"Status: {(downloadResult.Status.IsDownloading ? "Downloading" : "Not Downloading")}");

                // Simulate pause operation
                var pauseResult = downloadableProduct.PauseDownload(downloadResult.DownloadId);
                Console.WriteLine($"Pause Download: {(pauseResult.IsSuccess ? "Success" : "Failed")}");
            }
        }

        static void DemonstrateSchedulableOperations(ServiceProduct product)
        {
            // Only ServiceProduct implements ISchedulable
            ISchedulable schedulableProduct = product; // Safe cast

            Console.WriteLine($"Operating on: {product.GetName()}");
            Console.WriteLine();

            // Create a customer
            var customer = new Customer
            {
                CustomerId = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "555-1234",
                Address = "456 Oak Street"
            };

            // Get available time slots for tomorrow
            var tomorrow = DateTime.Today.AddDays(1);
            var availableSlots = schedulableProduct.GetAvailableTimeSlots(tomorrow);
            Console.WriteLine($"Available slots for {tomorrow:yyyy-MM-dd}:");

            var displaySlots = availableSlots.Take(5).ToList(); // Show first 5 slots
            foreach (var slot in displaySlots)
            {
                Console.WriteLine($"  {slot.StartTime:HH:mm} - {slot.EndTime:HH:mm} (${slot.Price:F2})");
            }

            // Schedule an appointment
            if (availableSlots.Any())
            {
                var preferredTime = availableSlots.First().StartTime;
                var schedulingResult = schedulableProduct.Schedule(
                    preferredTime,
                    TimeSpan.FromHours(1),
                    customer
                );

                Console.WriteLine($"\nScheduling Attempt: {(schedulingResult.IsSuccess ? "Success" : "Failed")}");
                Console.WriteLine($"Message: {schedulingResult.Message}");

                if (schedulingResult.IsSuccess)
                {
                    Console.WriteLine($"Appointment ID: {schedulingResult.AppointmentId}");
                    Console.WriteLine($"Scheduled Time: {schedulingResult.ScheduledTime:yyyy-MM-dd HH:mm}");

                    // Get appointment details
                    var appointmentDetails = schedulableProduct.GetAppointmentDetails(schedulingResult.AppointmentId);
                    if (appointmentDetails != null)
                    {
                        Console.WriteLine($"Customer: {appointmentDetails.Customer.Name}");
                        Console.WriteLine($"Duration: {appointmentDetails.Duration.TotalHours} hour(s)");
                        Console.WriteLine($"Status: {appointmentDetails.Status}");
                    }

                    // Demonstrate rescheduling
                    var newTime = preferredTime.AddHours(2);
                    var rescheduleResult = schedulableProduct.Reschedule(schedulingResult.AppointmentId, newTime);
                    Console.WriteLine($"Reschedule to {newTime:HH:mm}: {(rescheduleResult.IsSuccess ? "Success" : "Failed")}");
                }
            }
            else
            {
                Console.WriteLine("No available slots for demonstration");
            }
        }

        static void DemonstrateISPBenefits(List<IProduct> products)
        {
            Console.WriteLine("ISP Benefits Demonstration:");
            Console.WriteLine("- Each product type implements only relevant interfaces");
            Console.WriteLine("- No forced implementation of irrelevant methods");
            Console.WriteLine("- Compile-time safety prevents invalid operations\n");

            foreach (var product in products)
            {
                Console.WriteLine($"Product: {product.GetName()} ({product.GetCategory()})");

                // Check which interfaces each product implements
                bool isShippable = product is IShippable;
                bool isDownloadable = product is IDownloadable;
                bool isSchedulable = product is ISchedulable;

                Console.WriteLine($"  Implements IShippable: {isShippable}");
                Console.WriteLine($"  Implements IDownloadable: {isDownloadable}");
                Console.WriteLine($"  Implements ISchedulable: {isSchedulable}");

                // Demonstrate safe operations based on interface implementation
                if (isShippable)
                {
                    var shippable = (IShippable)product;
                    var cost = shippable.CalculateShippingCost("Test Address", 1.0m);
                    Console.WriteLine($"  ✅ Can calculate shipping: ${cost:F2}");
                }
                else
                {
                    Console.WriteLine("  ❌ Cannot calculate shipping (doesn't implement IShippable)");
                }

                if (isDownloadable)
                {
                    var downloadable = (IDownloadable)product;
                    var fileSize = downloadable.GetFileSize();
                    Console.WriteLine($"  ✅ Can get file size: {fileSize / (1024 * 1024):F0} MB");
                }
                else
                {
                    Console.WriteLine("  ❌ Cannot download (doesn't implement IDownloadable)");
                }

                if (isSchedulable)
                {
                    var schedulable = (ISchedulable)product;
                    var slots = schedulable.GetAvailableTimeSlots(DateTime.Today.AddDays(1));
                    Console.WriteLine($"  ✅ Can check schedule: {slots.Count} slots available");
                }
                else
                {
                    Console.WriteLine("  ❌ Cannot schedule (doesn't implement ISchedulable)");
                }

                Console.WriteLine();
            }

            Console.WriteLine("🎯 Key ISP Benefits Observed:");
            Console.WriteLine("1. Clean separation of concerns");
            Console.WriteLine("2. No unnecessary method implementations");
            Console.WriteLine("3. Type safety at compile time");
            Console.WriteLine("4. Easy to extend with new product types");
            Console.WriteLine("5. Each interface has a single, focused responsibility");
        }
    }

    // ================================
    // BONUS: Extension Methods for Clean Code
    // ================================

    /// <summary>
    /// Extension methods to make the demo code cleaner
    /// </summary>
    public static class ProductExtensions
    {
        public static bool CanBeShipped(this IProduct product)
        {
            return product is IShippable;
        }

        public static bool CanBeDownloaded(this IProduct product)
        {
            return product is IDownloadable;
        }

        public static bool CanBeScheduled(this IProduct product)
        {
            return product is ISchedulable;
        }

        public static void ProcessShipping(this IProduct product, string address)
        {
            if (product is IShippable shippable)
            {
                Console.WriteLine($"Processing shipping for {product.GetName()}...");
                var result = shippable.Ship(address, "Standard");
                Console.WriteLine($"Result: {result.Message}");
            }
            else
            {
                Console.WriteLine($"Cannot ship {product.GetName()} - not a shippable product");
            }
        }
    }
}
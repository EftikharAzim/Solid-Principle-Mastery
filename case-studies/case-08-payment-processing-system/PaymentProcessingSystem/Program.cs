using System.Globalization;
using System.Text.RegularExpressions;

public interface IPaymentProcessor
{
    PaymentResult ProcessPayment(decimal amount);
}

public interface ICreditCardValidator
{
    ValidationResult ValidateCVV(string cvv);
    ValidationResult ValidateExpiryDate(string expiryDate);
    ValidationResult ValidateCardNumber(string cardNumber);
}

public interface IBankTransferValidator
{
    ValidationResult ValidateAccountNumber(string accountNumber);
    ValidationResult ValidateRoutingNumber(string routingNumber);
}

public interface IDigitalWalletValidator
{
    ValidationResult ValidateWalletId(string walletId);
    ValidationResult ValidateOTP(string otp);
}

public class PaymentResult
{
    public bool IsSuccess { get; set; }
    public required string Message { get; set; }
    public string? TransactionId { get; set; }

    public static PaymentResult Success(string message = "Operation completed successfully.")
    {
        return new PaymentResult { IsSuccess = true, Message = message };
    }

    public static PaymentResult Failure(string message)
    {
        return new PaymentResult { IsSuccess = false, Message = message };
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public required string Message { get; set; }

    public static ValidationResult Success(string message = "Validation succeeded.")
    {
        return new ValidationResult { IsValid = true, Message = message };
    }

    public static ValidationResult Failure(string message)
    {
        return new ValidationResult { IsValid = false, Message = message };
    }
}

public class CreditCardValidator : ICreditCardValidator
{
    public ValidationResult ValidateCVV(string cvv)
    {
        var cvvRegex = new Regex(@"^\d{3,4}$");

        return cvvRegex.IsMatch(cvv)
            ? ValidationResult.Success()
            : ValidationResult.Failure("Invalid CVV.");
    }

    public ValidationResult ValidateExpiryDate(string expiryDate)
    {
        if (
            !DateTime.TryParseExact(
                expiryDate,
                "MM/yyyy",
                null,
                DateTimeStyles.None,
                out DateTime parsedDate
            )
        )
        {
            return ValidationResult.Failure("Invalid expiry date format.");
        }

        int expiryMonth = parsedDate.Month;
        int expiryYear = parsedDate.Year;

        if (expiryMonth < 1 || expiryMonth > 12)
        {
            return ValidationResult.Failure("Invalid expiry month.");
        }
        if (expiryYear < DateTime.Now.Year || expiryYear > DateTime.Now.Year + 20)
        {
            return ValidationResult.Failure("Invalid expiry year.");
        }
        DateTime currentDate = DateTime.Now;
        int daysInExpiryMonth = DateTime.DaysInMonth(expiryYear, expiryMonth);
        DateTime cardExpiryDate = new DateTime(
            expiryYear,
            expiryMonth,
            daysInExpiryMonth,
            23,
            59,
            59
        );
        return cardExpiryDate > currentDate
            ? ValidationResult.Success()
            : ValidationResult.Failure("Card has expired.");
    }

    public ValidationResult ValidateCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return ValidationResult.Failure("Card number is required.");
        }

        string cleanedCardNumber = new string(cardNumber.Where(char.IsDigit).ToArray());

        CardType detectedCardType = DetectCardType(cleanedCardNumber);

        if (detectedCardType == CardType.Unknown)
        {
            return ValidationResult.Failure("Unknown card type.");
        }

        return IsValidCardLength(cleanedCardNumber, detectedCardType)
            ? ValidationResult.Success()
            : ValidationResult.Failure("Invalid card number length.");
    }

    private static bool IsValidCardLength(string cardNumber, CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Amex:
                return cardNumber.Length == 15;
            case CardType.Visa:
            case CardType.Mastercard:
                return cardNumber.Length == 16;
            // Add more cases for other card types and their specific lengths
            default:
                return false;
        }
    }

    private static CardType DetectCardType(string cardNumber)
    {
        // Simple prefix-based detection (example, may need more robust logic)
        if (cardNumber.StartsWith("34") || cardNumber.StartsWith("37"))
        {
            return CardType.Amex;
        }
        else if (cardNumber.StartsWith("4"))
        {
            return CardType.Visa;
        }
        else if (
            cardNumber.Length >= 2
            && int.TryParse(cardNumber.Substring(0, 2), out int prefix)
            && prefix >= 51
            && prefix <= 55
        )
        {
            return CardType.Mastercard;
        }
        // Add more prefixes for other card types
        return CardType.Unknown;
    }
}

public enum CardType
{
    Unknown,
    Amex,
    Visa,
    Mastercard,
}

public class CreditCardPaymentProcessor : IPaymentProcessor
{
    private readonly ICreditCardValidator _validator;
    private readonly string _cvv;
    private readonly string _expiryDate;
    private readonly string _cardNumber;

    public CreditCardPaymentProcessor(
        ICreditCardValidator validator,
        string cvv,
        string expiryDate,
        string cardNumber
    )
    {
        _validator = validator;
        _cvv = cvv;
        _expiryDate = expiryDate;
        _cardNumber = cardNumber;
    }

    public PaymentResult ProcessPayment(decimal amount)
    {
        var cvvValidation = _validator.ValidateCVV(_cvv);
        if (!cvvValidation.IsValid)
        {
            return PaymentResult.Failure(cvvValidation.Message); // Use specific error
        }
        var expiryDateValidation = _validator.ValidateExpiryDate(_expiryDate);
        if (!expiryDateValidation.IsValid)
        {
            return PaymentResult.Failure(expiryDateValidation.Message);
        }
        var cardValidation = _validator.ValidateCardNumber(_cardNumber);
        if (!cardValidation.IsValid)
        {
            return PaymentResult.Failure(cardValidation.Message);
        }
        if (amount > 0)
            return PaymentResult.Success("Credit card payment processed successfully.");
        else
            return PaymentResult.Failure("Invalid payment amount.");
    }
}

public class BankTransferValidator : IBankTransferValidator
{
    public ValidationResult ValidateAccountNumber(string accountNumber)
    {
        var accountRegex = new Regex(@"^\d{8,12}$");

        return accountRegex.IsMatch(accountNumber)
            ? ValidationResult.Success()
            : ValidationResult.Failure("Invalid account number.");
    }

    public ValidationResult ValidateRoutingNumber(string routingNumber)
    {
        var routingRegex = new Regex(@"^\d{9}$");

        return routingRegex.IsMatch(routingNumber)
            ? ValidationResult.Success()
            : ValidationResult.Failure("Invalid routing number.");
    }
}

public class BankTransferPaymentProcessor : IPaymentProcessor
{
    private readonly IBankTransferValidator _validator;
    private readonly string _accountNumber;
    private readonly string _routingNumber;

    public BankTransferPaymentProcessor(
        IBankTransferValidator validator,
        string accountNumber,
        string routingNumber
    )
    {
        _validator = validator;
        _accountNumber = accountNumber;
        _routingNumber = routingNumber;
    }

    public PaymentResult ProcessPayment(decimal amount)
    {
        var accountValidation = _validator.ValidateAccountNumber(_accountNumber);
        if (!accountValidation.IsValid)
        {
            return PaymentResult.Failure(accountValidation.Message);
        }
        var routingValidation = _validator.ValidateRoutingNumber(_routingNumber);
        if (!routingValidation.IsValid)
        {
            return PaymentResult.Failure(routingValidation.Message);
        }
        if (amount > 0)
            return PaymentResult.Success("Bank transfer payment processed successfully.");
        else
            return PaymentResult.Failure("Invalid payment amount.");
    }
}

public class DigitalWalletValidator : IDigitalWalletValidator
{
    public ValidationResult ValidateWalletId(string walletId)
    {
        if (string.IsNullOrWhiteSpace(walletId))
        {
            return ValidationResult.Failure("Wallet ID is required.");
        }

        // Check if it's email OR phone number
        if (IsValidEmail(walletId) || IsValidPhoneNumber(walletId))
        {
            return ValidationResult.Success();
        }

        return ValidationResult.Failure("Wallet ID must be a valid email or phone number.");
    }

    private static bool IsValidEmail(string email)
    {
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return emailRegex.IsMatch(email);
    }

    private static bool IsValidPhoneNumber(string phone)
    {
        var phoneRegex = new Regex(@"^\+?[\d\s\-\(\)]{10,15}$");
        return phoneRegex.IsMatch(phone);
    }

    public ValidationResult ValidateOTP(string otp)
    {
        var otpRegex = new Regex(@"^\d{6}$");

        return otpRegex.IsMatch(otp)
            ? ValidationResult.Success()
            : ValidationResult.Failure("Invalid OTP.");
    }
}

public class DigitalWalletPaymentProcessor : IPaymentProcessor
{
    private readonly IDigitalWalletValidator _validator;
    private readonly string _walletId;
    private readonly string _otp;

    public DigitalWalletPaymentProcessor(
        IDigitalWalletValidator validator,
        string walletId,
        string otp
    )
    {
        _validator = validator;
        _walletId = walletId;
        _otp = otp;
    }

    public PaymentResult ProcessPayment(decimal amount)
    {
        var walletIdValidation = _validator.ValidateWalletId(_walletId);
        if (!walletIdValidation.IsValid)
        {
            return PaymentResult.Failure(walletIdValidation.Message);
        }
        var otpValidation = _validator.ValidateOTP(_otp);
        if (!otpValidation.IsValid)
        {
            return PaymentResult.Failure(otpValidation.Message);
        }
        if (amount > 0)
            return PaymentResult.Success("Digital wallet payment processed successfully.");
        else
            return PaymentResult.Failure("Invalid payment amount.");
    }
}

public static class PaymentFactory
{
    public static IPaymentProcessor CreatePaymentProcessor(PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.CreditCard => CreateCreditCardProcessor(),
            PaymentType.BankTransfer => CreateBankTransferProcessor(),
            PaymentType.DigitalWallet => CreateDigitalWalletProcessor(),
            _ => throw new ArgumentException($"Unsupported payment type: {paymentType}"),
        };
    }

    private static IPaymentProcessor CreateCreditCardProcessor()
    {
        var validator = new CreditCardValidator();
        return new CreditCardPaymentProcessor(
            validator,
            cvv: "123",
            expiryDate: "12/2025",
            cardNumber: "4111111111111111" // Valid Visa test number
        );
    }

    private static IPaymentProcessor CreateBankTransferProcessor()
    {
        var validator = new BankTransferValidator();
        return new BankTransferPaymentProcessor(
            validator,
            accountNumber: "12345678901",
            routingNumber: "123456789"
        );
    }

    private static IPaymentProcessor CreateDigitalWalletProcessor()
    {
        var validator = new DigitalWalletValidator();
        return new DigitalWalletPaymentProcessor(
            validator,
            walletId: "user@example.com",
            otp: "123456"
        );
    }
}

public enum PaymentType
{
    CreditCard,
    BankTransfer,
    DigitalWallet,
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== SOLID Principles Payment Processing System ===");
        Console.WriteLine("Demonstrating Interface Segregation Principle (ISP)\n");

        bool exit = false;
        while (!exit)
        {
            ShowMenu();
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ProcessSinglePayment(PaymentType.CreditCard);
                    break;
                case "2":
                    ProcessSinglePayment(PaymentType.BankTransfer);
                    break;
                case "3":
                    ProcessSinglePayment(PaymentType.DigitalWallet);
                    break;
                case "4":
                    DemonstratePolymorphism();
                    break;
                case "5":
                    DemonstrateErrorHandling();
                    break;
                case "6":
                    exit = true;
                    Console.WriteLine("Thank you for using the Payment System!");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.\n");
                    break;
            }
        }
    }

    static void ShowMenu()
    {
        Console.WriteLine("\n--- Payment Processing Menu ---");
        Console.WriteLine("1. Process Credit Card Payment");
        Console.WriteLine("2. Process Bank Transfer Payment");
        Console.WriteLine("3. Process Digital Wallet Payment");
        Console.WriteLine("4. Demonstrate Polymorphism (All Payment Types)");
        Console.WriteLine("5. Demonstrate Error Handling");
        Console.WriteLine("6. Exit");
        Console.Write("\nEnter your choice (1-6): ");
    }

    static void ProcessSinglePayment(PaymentType paymentType)
    {
        Console.WriteLine($"\n--- Processing {paymentType} Payment ---");

        // Get payment amount from user
        decimal amount = GetPaymentAmount();

        // Create processor using factory
        var processor = PaymentFactory.CreatePaymentProcessor(paymentType);

        // Process payment - demonstrating polymorphism
        var result = processor.ProcessPayment(amount);

        // Display result
        DisplayResult(result, paymentType.ToString());
    }

    static void DemonstratePolymorphism()
    {
        Console.WriteLine("\n--- Polymorphism Demonstration ---");
        Console.WriteLine("Processing same amount with different payment types...\n");

        decimal amount = 250.00m;
        Console.WriteLine($"Payment Amount: ${amount:F2}\n");

        // Create different payment processors
        var paymentProcessors = new[]
        {
            (PaymentFactory.CreatePaymentProcessor(PaymentType.CreditCard), "Credit Card"),
            (PaymentFactory.CreatePaymentProcessor(PaymentType.BankTransfer), "Bank Transfer"),
            (PaymentFactory.CreatePaymentProcessor(PaymentType.DigitalWallet), "Digital Wallet"),
        };

        // Process payments polymorphically - same interface, different implementations
        foreach (var (processor, typeName) in paymentProcessors)
        {
            Console.WriteLine($"Processing {typeName}...");
            var result = processor.ProcessPayment(amount); // Same method call!
            DisplayResult(result, typeName);
            Console.WriteLine();
        }

        Console.WriteLine("✨ Notice: Same ProcessPayment() method call, different behaviors!");
        Console.WriteLine("This demonstrates the power of polymorphism and Interface Segregation!");
    }

    static void DemonstrateErrorHandling()
    {
        Console.WriteLine("\n--- Error Handling Demonstration ---");
        Console.WriteLine("Testing validation failures...\n");

        // Test invalid credit card
        TestInvalidCreditCard();

        // Test invalid bank transfer
        TestInvalidBankTransfer();

        // Test invalid digital wallet
        TestInvalidDigitalWallet();
    }

    static void TestInvalidCreditCard()
    {
        Console.WriteLine("1. Testing Invalid Credit Card:");
        var validator = new CreditCardValidator();
        var processor = new CreditCardPaymentProcessor(
            validator,
            cvv: "12", // Invalid CVV (too short)
            expiryDate: "01/2020", // Expired card
            cardNumber: "1234" // Invalid card number
        );

        var result = processor.ProcessPayment(100);
        DisplayResult(result, "Invalid Credit Card");
    }

    static void TestInvalidBankTransfer()
    {
        Console.WriteLine("\n2. Testing Invalid Bank Transfer:");
        var validator = new BankTransferValidator();
        var processor = new BankTransferPaymentProcessor(
            validator,
            accountNumber: "123", // Too short
            routingNumber: "12345678" // Wrong length
        );

        var result = processor.ProcessPayment(100);
        DisplayResult(result, "Invalid Bank Transfer");
    }

    static void TestInvalidDigitalWallet()
    {
        Console.WriteLine("\n3. Testing Invalid Digital Wallet:");
        var validator = new DigitalWalletValidator();
        var processor = new DigitalWalletPaymentProcessor(
            validator,
            walletId: "invalid-wallet", // Not email or phone
            otp: "12345" // Wrong OTP length
        );

        var result = processor.ProcessPayment(100);
        DisplayResult(result, "Invalid Digital Wallet");
    }

    static decimal GetPaymentAmount()
    {
        while (true)
        {
            Console.Write("Enter payment amount: $");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
            {
                return amount;
            }
            Console.WriteLine("Please enter a valid positive amount.");
        }
    }

    static void DisplayResult(PaymentResult result, string paymentType)
    {
        if (result.IsSuccess)
        {
            Console.WriteLine($"✅ SUCCESS: {result.Message}");
        }
        else
        {
            Console.WriteLine($"❌ FAILED: {result.Message}");
        }
    }
}

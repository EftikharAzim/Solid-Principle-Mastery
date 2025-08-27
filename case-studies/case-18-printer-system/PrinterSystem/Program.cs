using System;
using System.Threading.Tasks;

namespace SOLIDPrinciples.ISP
{
    // Domain Model - Enhanced Document
    public class Document
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int PageCount { get; set; } = 1;

        public override string ToString()
        {
            return $"Document: {Title} ({PageCount} pages)";
        }
    }

    // ISP - Segregated Interfaces
    public interface IPrinter
    {
        Task<bool> Print(Document document);
    }

    public interface IScanner
    {
        Task<Document> Scan();
    }

    public interface ICopier
    {
        Task<Document> Copy(Document document);
    }

    public interface IFaxer
    {
        Task<bool> Fax(Document document, string phoneNumber);
    }

    // Concrete Implementations
    public class SimplePrinter : IPrinter
    {
        public string ModelName { get; }

        public SimplePrinter(string modelName)
        {
            ModelName = modelName;
        }

        public async Task<bool> Print(Document document)
        {
            Console.WriteLine($"[{ModelName}] Printing: {document}");
            await Task.Delay(1000); // Simulate printing time
            Console.WriteLine($"[{ModelName}] Print completed successfully!");
            return true;
        }
    }

    public class PrinterScanner : IPrinter, IScanner
    {
        public string ModelName { get; }

        public PrinterScanner(string modelName)
        {
            ModelName = modelName;
        }

        public async Task<bool> Print(Document document)
        {
            Console.WriteLine($"[{ModelName}] Printing: {document}");
            await Task.Delay(1000);
            Console.WriteLine($"[{ModelName}] Print completed!");
            return true;
        }

        public async Task<Document> Scan()
        {
            Console.WriteLine($"[{ModelName}] Scanning document...");
            await Task.Delay(1500);

            var scannedDoc = new Document
            {
                Title = "Scanned Document",
                Content = "Scanned content from paper",
                PageCount = 1
            };

            Console.WriteLine($"[{ModelName}] Scan completed: {scannedDoc}");
            return scannedDoc;
        }
    }

    public class MultiFunctionPrinter : IPrinter, IScanner, ICopier, IFaxer
    {
        public string ModelName { get; }

        public MultiFunctionPrinter(string modelName)
        {
            ModelName = modelName;
        }

        public async Task<bool> Print(Document document)
        {
            Console.WriteLine($"[{ModelName}] High-quality printing: {document}");
            await Task.Delay(800);
            Console.WriteLine($"[{ModelName}] Print completed with premium quality!");
            return true;
        }

        public async Task<Document> Scan()
        {
            Console.WriteLine($"[{ModelName}] High-resolution scanning...");
            await Task.Delay(1200);

            var scannedDoc = new Document
            {
                Title = "High-Res Scanned Document",
                Content = "Professional quality scanned content",
                PageCount = 1
            };

            Console.WriteLine($"[{ModelName}] Professional scan completed!");
            return scannedDoc;
        }

        public async Task<Document> Copy(Document document)
        {
            Console.WriteLine($"[{ModelName}] Copying: {document}");
            await Task.Delay(1000);

            var copiedDoc = new Document
            {
                Title = $"Copy of {document.Title}",
                Content = document.Content,
                PageCount = document.PageCount
            };

            Console.WriteLine($"[{ModelName}] Copy completed!");
            return copiedDoc;
        }

        public async Task<bool> Fax(Document document, string phoneNumber)
        {
            Console.WriteLine($"[{ModelName}] Faxing {document} to {phoneNumber}");
            await Task.Delay(2000);
            Console.WriteLine($"[{ModelName}] Fax sent successfully to {phoneNumber}!");
            return true;
        }
    }

    // Demonstration Program
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== SOLID ISP Demonstration ===\n");

            // Test Document
            var document = new Document
            {
                Title = "Important Report",
                Content = "This is a critical business document",
                PageCount = 3
            };

            // Different printer types - each implements only what they need
            var basicPrinter = new SimplePrinter("HP LaserJet Basic");
            var printerScanner = new PrinterScanner("Canon PIXMA Pro");
            var allInOne = new MultiFunctionPrinter("Xerox WorkCentre Pro");

            Console.WriteLine("1. Basic Printer (Only Printing):");
            await basicPrinter.Print(document);

            Console.WriteLine("\n2. Printer-Scanner (Print + Scan):");
            await printerScanner.Print(document);
            var scannedDoc = await printerScanner.Scan();

            Console.WriteLine("\n3. All-in-One Printer (All Functions):");
            await allInOne.Print(document);
            var scannedDoc2 = await allInOne.Scan();
            var copiedDoc = await allInOne.Copy(document);
            await allInOne.Fax(document, "+1-555-0123");

            Console.WriteLine("\n=== ISP Benefits Demonstrated ===");
            Console.WriteLine("✅ Each class implements only what it needs");
            Console.WriteLine("✅ No forced unnecessary implementations");
            Console.WriteLine("✅ Clean, maintainable, and extensible design");
        }
    }
}
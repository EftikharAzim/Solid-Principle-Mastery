using System;
using System.Collections.Generic;
using PayrollSystem.Models;
using PayrollSystem.Services;

namespace PayrollSystem
{
    /// <summary>
    /// Main program demonstrating the Liskov Substitution Principle (LSP)
    /// through a payroll management system.
    ///
    /// Key LSP Concepts Demonstrated:
    /// 1. Substitutability - All employee types can replace the base Employee class
    /// 2. Behavioral contracts - Each subclass honors the same calculation contract
    /// 3. Interface segregation - Optional capabilities separated into interfaces
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SOLID Principles - Liskov Substitution Principle Demo");
            Console.WriteLine("====================================================");
            Console.WriteLine();

            try
            {
                // Create sample employees of different types
                var employees = CreateSampleEmployees();

                // Create payroll service
                var payrollService = new PayrollService();

                // First, demonstrate LSP substitutability
                payrollService.DemonstrateSubstitutability(employees);
                Console.WriteLine();

                // Then run actual payroll processing
                payrollService.ProcessPayroll(employees);

                Console.WriteLine();
                Console.WriteLine("=== KEY LSP OBSERVATIONS ===");
                Console.WriteLine(
                    "1. PayrollService.ProcessPayroll() works with ANY Employee type"
                );
                Console.WriteLine(
                    "2. Each employee type has different calculation logic, but same interface"
                );
                Console.WriteLine(
                    "3. Bonus capability is optional - only some employees implement it"
                );
                Console.WriteLine(
                    "4. Adding new employee types requires no changes to PayrollService"
                );
                Console.WriteLine();

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Creates a diverse set of employees to demonstrate LSP with different types
        /// </summary>
        private static List<Employee> CreateSampleEmployees()
        {
            return new List<Employee>
            {
                // Regular employee eligible for bonus (employed > 12 months)
                new RegularEmployee
                {
                    Name = "Alice Johnson",
                    EmployeeId = "REG001",
                    HireDate = DateTime.Now.AddMonths(-18), // 18 months ago
                    HourlyRate = 25m,
                    HoursWorked = 42, // Includes overtime
                    PerformanceRating = 1.5m, // Above average performance
                },
                // Regular employee not yet eligible for bonus (< 12 months)
                new RegularEmployee
                {
                    Name = "David Wilson",
                    EmployeeId = "REG002",
                    HireDate = DateTime.Now.AddMonths(-8), // 8 months ago
                    HourlyRate = 20m,
                    HoursWorked = 40, // No overtime
                    PerformanceRating = 1.2m,
                },
                // Contract employee (no bonus eligibility by design)
                new ContractEmployee
                {
                    Name = "Bob Smith",
                    EmployeeId = "CON001",
                    HireDate = DateTime.Now.AddMonths(-6),
                    ContractAmount = 60000m,
                    PayPeriods = 12,
                    ContractStartDate = DateTime.Now.AddMonths(-6),
                    ContractEndDate = DateTime.Now.AddMonths(6),
                },
                // Intern eligible for retention bonus (> 6 months)
                new InternEmployee
                {
                    Name = "Charlie Brown",
                    EmployeeId = "INT001",
                    HireDate = DateTime.Now.AddMonths(-8), // 8 months ago
                    MonthlyStipend = 2000m,
                    University = "State University",
                },
                // New intern not yet eligible for bonus (< 6 months)
                new InternEmployee
                {
                    Name = "Emma Davis",
                    EmployeeId = "INT002",
                    HireDate = DateTime.Now.AddMonths(-3), // 3 months ago
                    MonthlyStipend = 1800m,
                    University = "Tech College",
                },
            };
        }
    }
}

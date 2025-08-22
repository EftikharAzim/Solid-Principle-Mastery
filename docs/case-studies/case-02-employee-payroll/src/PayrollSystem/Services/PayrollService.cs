using System;
using System.Collections.Generic;
using PayrollSystem.Interfaces;
using PayrollSystem.Models;

namespace PayrollSystem.Services
{
    /// <summary>
    /// Payroll service that demonstrates the power of LSP.
    /// This service can work with ANY employee type that extends Employee,
    /// without knowing or caring about the specific implementation details.
    /// </summary>
    public class PayrollService
    {
        /// <summary>
        /// Processes payroll for a list of employees.
        /// This method demonstrates LSP in action - it works identically
        /// whether the list contains RegularEmployee, ContractEmployee,
        /// InternEmployee, or any future employee type.
        /// </summary>
        public void ProcessPayroll(List<Employee> employees)
        {
            Console.WriteLine("=== PAYROLL PROCESSING ===");
            Console.WriteLine();

            decimal totalPayrollCost = 0;

            foreach (Employee employee in employees)
            {
                Console.WriteLine(employee.GetEmployeeInfo());

                // This line is the magic of LSP - it works identically for ALL employee types
                // The method call is the same, but each type implements its own calculation logic
                decimal baseSalary = employee.CalculateSalary();
                decimal totalPay = baseSalary;

                Console.WriteLine($"  Base Salary: ${baseSalary:F2}");

                // Interface segregation in action - only employees that implement
                // IBonusEligible will have their bonus calculated
                if (employee is IBonusEligible bonusEligible)
                {
                    decimal bonus = bonusEligible.CalculateBonus();
                    totalPay += bonus;
                    Console.WriteLine($"  Bonus: ${bonus:F2}");

                    if (bonus == 0)
                    {
                        Console.WriteLine(
                            $"    (Not eligible yet - needs {GetRequiredMonthsForBonus(employee)} months)"
                        );
                    }
                }
                else
                {
                    Console.WriteLine($"  Bonus: Not eligible (contract employee)");
                }

                Console.WriteLine($"  Total Pay: ${totalPay:F2}");
                Console.WriteLine();

                totalPayrollCost += totalPay;
            }

            Console.WriteLine($"TOTAL PAYROLL COST: ${totalPayrollCost:F2}");
        }

        /// <summary>
        /// Demonstrates LSP substitutability by showing how different employee types
        /// can be treated identically from the client code perspective.
        /// </summary>
        public void DemonstrateSubstitutability(List<Employee> employees)
        {
            Console.WriteLine("=== DEMONSTRATING LSP SUBSTITUTABILITY ===");
            Console.WriteLine();

            // This loop demonstrates perfect substitutability
            // Each call to CalculateSalary() works identically regardless of the actual type
            foreach (Employee emp in employees)
            {
                Console.WriteLine($"Processing {emp.GetType().Name}: {emp.Name}");
                Console.WriteLine($"  Salary Calculation Result: ${emp.CalculateSalary():F2}");

                // This is the LSP test: can we treat all employees identically?
                // YES - because they all honor the same behavioral contract
                Console.WriteLine($"  Months Employed: {emp.GetMonthsEmployed()}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Helper method to determine bonus eligibility requirements for different employee types
        /// </summary>
        private int GetRequiredMonthsForBonus(Employee employee)
        {
            return employee switch
            {
                RegularEmployee => 12,
                InternEmployee => 6,
                _ => 0,
            };
        }
    }
}

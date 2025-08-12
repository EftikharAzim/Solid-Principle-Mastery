using System;

namespace PayrollSystem.Models
{
    /// <summary>
    /// Abstract base class representing the core contract for all employees.
    /// This is our LSP foundation - any subclass must be completely substitutable
    /// for this base class in any context where Employee is used.
    /// </summary>
    public abstract class Employee
    {
        public string Name { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }

        /// <summary>
        /// The core behavioral contract that all employees must honor.
        /// Each subclass implements this differently, but the behavior must be consistent:
        /// - Always returns a valid, non-negative decimal
        /// - Never throws exceptions under normal conditions
        /// - Is deterministic given the same input state
        /// </summary>
        public abstract decimal CalculateSalary();

        /// <summary>
        /// Common behavior that applies to all employee types.
        /// This demonstrates shared functionality that inheritance provides well.
        /// </summary>
        public virtual string GetEmployeeInfo()
        {
            return $"Employee: {Name} (ID: {EmployeeId})";
        }

        /// <summary>
        /// Helper method to calculate months employed - useful for bonus eligibility
        /// </summary>
        public int GetMonthsEmployed()
        {
            return (int)Math.Floor((DateTime.Now - HireDate).TotalDays / 30);
        }
    }
}

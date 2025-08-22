using System;
using PayrollSystem.Interfaces;

namespace PayrollSystem.Models
{
    /// <summary>
    /// Regular hourly employees who are eligible for performance-based bonuses.
    /// Demonstrates LSP compliance: can substitute for Employee anywhere.
    /// </summary>
    public class RegularEmployee : Employee, IBonusEligible
    {
        public decimal HourlyRate { get; set; }
        public int HoursWorked { get; set; }
        public decimal PerformanceRating { get; set; } = 1.0m; // Default to standard performance

        // IBonusEligible implementation
        public decimal BaseSalaryForBonus => CalculateSalary();
        public int MonthsEmployed => GetMonthsEmployed();

        /// <summary>
        /// LSP-compliant salary calculation for regular employees.
        /// Includes overtime pay calculation for hours over 40.
        /// Always returns a valid salary amount.
        /// </summary>
        public override decimal CalculateSalary()
        {
            // Regular pay for up to 40 hours
            decimal regularPay = HourlyRate * Math.Min(HoursWorked, 40);

            // Overtime pay at 1.5x rate for hours over 40
            decimal overtimeHours = Math.Max(0, HoursWorked - 40);
            decimal overtimePay = HourlyRate * 1.5m * overtimeHours;

            return regularPay + overtimePay;
        }

        /// <summary>
        /// Performance-based bonus calculation for regular employees.
        /// Requires at least 12 months of employment.
        /// </summary>
        public decimal CalculateBonus()
        {
            // Must be employed for at least a year to get bonus
            if (MonthsEmployed < 12)
                return 0;

            // Performance-based bonus: base salary × performance rating × 10%
            return BaseSalaryForBonus * PerformanceRating * 0.1m;
        }

        public override string GetEmployeeInfo()
        {
            return base.GetEmployeeInfo() + $" - Regular Employee (${HourlyRate}/hr)";
        }
    }
}

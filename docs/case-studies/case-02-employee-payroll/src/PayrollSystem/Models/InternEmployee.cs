using PayrollSystem.Interfaces;

namespace PayrollSystem.Models
{
    /// <summary>
    /// Intern employees with stipend-based pay and small retention bonuses.
    /// Demonstrates how a simple employee type can still implement bonus eligibility
    /// with different rules than regular employees.
    /// </summary>
    public class InternEmployee : Employee, IBonusEligible
    {
        public decimal MonthlyStipend { get; set; }
        public string University { get; set; } = string.Empty;

        // IBonusEligible implementation
        public decimal BaseSalaryForBonus => MonthlyStipend;
        public int MonthsEmployed => GetMonthsEmployed();

        /// <summary>
        /// LSP-compliant salary calculation for interns.
        /// Simple stipend payment - no overtime or complex calculations.
        /// </summary>
        public override decimal CalculateSalary()
        {
            return MonthlyStipend;
        }

        /// <summary>
        /// Simple retention bonus for interns who stay longer than 6 months.
        /// Fixed amount rather than percentage-based like regular employees.
        /// </summary>
        public decimal CalculateBonus()
        {
            // Interns need to be here for 6 months to get retention bonus
            if (MonthsEmployed < 6)
                return 0;

            // Fixed retention bonus for interns
            return 500m; // $500 retention bonus
        }

        public override string GetEmployeeInfo()
        {
            return base.GetEmployeeInfo()
                + $" - Intern from {University} (${MonthlyStipend}/month)";
        }
    }
}

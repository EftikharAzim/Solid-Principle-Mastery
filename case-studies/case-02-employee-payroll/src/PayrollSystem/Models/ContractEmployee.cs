using System;

namespace PayrollSystem.Models
{
    /// <summary>
    /// Contract employees with fixed payment amounts.
    /// Notice: Does NOT implement IBonusEligible - this is key to LSP compliance.
    /// The class doesn't promise capabilities it can't deliver.
    /// </summary>
    public class ContractEmployee : Employee
    {
        public decimal ContractAmount { get; set; }
        public int PayPeriods { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }

        /// <summary>
        /// LSP-compliant salary calculation for contract employees.
        /// Simple division of total contract amount by pay periods.
        /// Completely different logic than RegularEmployee, but honors the same contract.
        /// </summary>
        public override decimal CalculateSalary()
        {
            // Prevent division by zero
            if (PayPeriods <= 0)
                throw new InvalidOperationException("Pay periods must be greater than zero");

            return ContractAmount / PayPeriods;
        }

        public override string GetEmployeeInfo()
        {
            return base.GetEmployeeInfo() + $" - Contract Employee (${ContractAmount} total)";
        }

        /// <summary>
        /// Helper method to check if contract is currently active
        /// </summary>
        public bool IsContractActive()
        {
            var now = DateTime.Now;
            return now >= ContractStartDate && now <= ContractEndDate;
        }
    }
}

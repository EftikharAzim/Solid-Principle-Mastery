namespace PayrollSystem.Interfaces
{
    /// <summary>
    /// Interface segregation principle in action.
    /// This interface represents an optional capability that only some employees have.
    /// By separating this from the base Employee class, we avoid forcing all employees
    /// to implement bonus logic they don't support (which would violate LSP).
    /// </summary>
    public interface IBonusEligible
    {
        /// <summary>
        /// The base salary amount used for bonus calculations.
        /// Different employee types might calculate this differently.
        /// </summary>
        decimal BaseSalaryForBonus { get; }

        /// <summary>
        /// Number of months the employee has been with the company.
        /// Used to determine bonus eligibility.
        /// </summary>
        int MonthsEmployed { get; }

        /// <summary>
        /// Calculates the bonus amount for this employee.
        /// Returns 0 if the employee is not currently eligible for a bonus.
        /// </summary>
        decimal CalculateBonus();
    }
}

using System;
using System.Windows;
using System.Windows.Input;

namespace LoenBeregnerWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Attach event handler for Enter key press
            this.KeyDown += new KeyEventHandler(OnKeyDownHandler);
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CalculateButton_Click(this, new RoutedEventArgs());
            }
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Retrieve values from input fields
                if (!int.TryParse(HoursWorkedTextBox.Text, out int hoursWorked))
                {
                    ShowError("Indtast venligst et gyldigt antal timer.");
                    return;
                }

                if (!int.TryParse(ExtraHoursWorkedTextBox.Text, out int extraHoursWorked))
                {
                    ShowError("Indtast venligst et gyldigt antal timer med tillæg.");
                    return;
                }

                decimal hourlyWage = 139m;
                if (!string.IsNullOrWhiteSpace(HourlyWageTextBox.Text) && !decimal.TryParse(HourlyWageTextBox.Text, out hourlyWage))
                {
                    ShowError("Indtast venligst en gyldig timeløn.");
                    return;
                }

                decimal SU = 1824m;
                if (!string.IsNullOrWhiteSpace(SUTextBox.Text) && !decimal.TryParse(SUTextBox.Text, out SU))
                {
                    ShowError("Indtast venligst en gyldig SU sats.");
                    return;
                }

                decimal taxDeduction = 4352m;
                if (!string.IsNullOrWhiteSpace(TaxDeductionTextBox.Text) && !decimal.TryParse(TaxDeductionTextBox.Text, out taxDeduction))
                {
                    ShowError("Indtast venligst et gyldigt personfradrag.");
                    return;
                }

                decimal AMPercentage = 0.08m;
                decimal taxRate = 0.38m;
                decimal extraHourlyWage = 169m;

                // Calculate gross income
                decimal grossIncome = Math.Round((hoursWorked * hourlyWage) + (extraHoursWorked * extraHourlyWage), 2);

                // Calculate AM contribution
                decimal AMContribution = Math.Round(AMPercentage * grossIncome, 2);

                // Calculate income after AM contribution
                decimal incomeAfterAM = Math.Round(grossIncome - AMContribution, 2);

                // Calculate taxable income
                decimal taxableIncome = Math.Round(incomeAfterAM - taxDeduction, 2);

                // Calculate tax
                decimal tax = Math.Round(taxRate * taxableIncome, 2);

                // Calculate net income after tax
                decimal netIncomeAfterTax = Math.Round(incomeAfterAM - tax, 2);

                // Calculate feriepenge
                decimal feriepenge = Math.Round(0.12m * netIncomeAfterTax, 2);

                // Calculate ATP contribution
                decimal ATPContribution = 0m;
                if (hoursWorked < 78)
                {
                    ATPContribution = 33m;
                }
                else if (hoursWorked < 117)
                {
                    ATPContribution = 66m;
                }
                else
                {
                    ATPContribution = 99m;
                }

                // Calculate net income after ATP contribution
                netIncomeAfterTax -= ATPContribution;

                // Total udbetaling
                decimal totalUdbetaling = Math.Round(netIncomeAfterTax + SU, 2);

                // Display the result
                ResultTextBlock.Text = $"Bruttoløn: {grossIncome} kr\n" +
                                        $"AM-bidrag: {AMContribution} kr\n" +
                                        $"Løn efter AM-bidrag: {incomeAfterAM} kr\n" +
                                        $"Skattepligtig indkomst: {taxableIncome} kr\n" +
                                        $"Skat: {tax} kr\n" +
                                        $"ATP-bidrag: {ATPContribution} kr\n" +
                                        $"Nettoindkomst efter skat og ATP: {netIncomeAfterTax} kr\n" +
                                        $"Feriepenge: {feriepenge} kr\n" +
                                        $"Total udbetaling: {totalUdbetaling} kr";

                // Show New Calculation button and hide Calculate button
                CalculateButton.Visibility = Visibility.Collapsed;
                NewCalculationButton.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Der opstod en fejl: " + ex.Message);
            }
        }

        private void NewCalculationButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear all input fields and result
            HoursWorkedTextBox.Text = string.Empty;
            ExtraHoursWorkedTextBox.Text = string.Empty;
            HourlyWageTextBox.Text = string.Empty;
            SUTextBox.Text = string.Empty;
            TaxDeductionTextBox.Text = string.Empty;
            ResultTextBlock.Text = string.Empty;

            // Reset buttons visibility
            CalculateButton.Visibility = Visibility.Visible;
            NewCalculationButton.Visibility = Visibility.Collapsed;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message);
            ResultTextBlock.Text = string.Empty;
        }
    }
}

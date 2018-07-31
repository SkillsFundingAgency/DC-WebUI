namespace DC.Web.Ui.Services.ViewModels
{
    public class ReturnPeriodViewModel
    {
        private readonly string _periodName;

        public ReturnPeriodViewModel(int periodNumber)
        {
            PeriodNumber = periodNumber;
            _periodName = $"R{periodNumber}";
        }

        public int PeriodNumber { get; set; }

        public string PeriodName() => _periodName;
    }
}

namespace DC.Web.Ui.Services.ViewModels
{
    public class ValidationReport
    {
        public int TotalErrors { get; set; }

        public int TotalWarnings { get; set; }

        public int WarningLearners { get; set; }

        public int ErrorLearners { get; set; }

        public string PeriodName { get; set; }
    }

    public class LearnerSummary
    {
        public string Type { get; set; }

        public int CleanLearners { get; set; }

        public int WarningLearners { get; set; }

        public int ErrorLearners { get; set; }
    }

    public class LearningDeliverySummary
    {
        public string Type { get; set; }

        public int Count { get; set; }
    }

    public class LearnerDestinationProgressionSummary
    {
        public string Type { get; set; }

        public int Count { get; set; }
    }
}

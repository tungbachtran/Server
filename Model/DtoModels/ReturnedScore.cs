using api.Model.DatabaseModels;

namespace api.Model.DtoModels
{
    public class ReturnedScore
    {
        public string Student { get; set; }
        public string userId { get; set; }
        public Score Score { get; set; }
        public double TotalScore { get; set; }
    }
}

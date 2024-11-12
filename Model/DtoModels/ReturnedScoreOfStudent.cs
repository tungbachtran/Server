using api.Model.DatabaseModels;

namespace api.Model.DtoModels
{
    public class ReturnedScoreOfStudent
    {
        public CourseClassroom CourseClassroom { get; set; }
        public Score Score { get; set; }
        public double TotalScore { get; set; }
    }
}

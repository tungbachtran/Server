


namespace api.Model.DatabaseModels
{
    public class CourseEducationProgram
    {
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.Always)]
        public Course Course { get; set; }
        public string CourseId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.Always)]
        public EducationalProgram EducationalProgram { get; set; }
        public string EducationalProgramId { get; set; }
        public int Semester { get; set; }
    }
}

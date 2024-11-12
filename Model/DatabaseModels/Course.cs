namespace api.Model.DatabaseModels
{
    public class Course
    {
      
        public string CourseId { get; set; }
        public string Name { get; set; }
        public double Credits { get; set; }
        public string? requiredCourseId { get; set; }
        public bool? isAvailable { get; set; }
       
    }
}
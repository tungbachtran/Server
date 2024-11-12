using api.Model.DatabaseModels;

namespace api.Model.DtoModels
{
    public class ReturnedCourseClassroomDto
    {
        public CourseClassroom CourseClassroom { get; set; }
        public List<Schedule> Schedule { get; set; }
        public int NumberOfRegisteredStudent { get; set; }
        public string TeacherName { get; set; }
    }
}

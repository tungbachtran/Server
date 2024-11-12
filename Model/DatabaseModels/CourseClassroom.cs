using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace api.Model.DatabaseModels
{
    public class CourseClassroom
    {
        [Key]
        public string CourseClassId { get; set; }
        public string TeacherName { get; set; }
        public Boolean isComplete { get; set; }
        public int Capacity { get; set; }
        public Course Course { get; set; }
        public string CourseId { get; set; }
        public ICollection<Documents> Documents { get; set; }
        public ICollection<ChatMessages> ChatMessages { get; set; } = new List<ChatMessages>();

    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace api.Model.DatabaseModels
{
    public class Classroom
    {
        [Key]
        public string ClassroomId { get; set; }
        public string Name { get; set; }
        public int AcademicYear { get; set; }
        public List<User> Students { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Faculty Faculty { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string FacultyId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public EducationalProgram EducationalProgram { get; set; }
        public string EducationalProgramId { get; set; }
    }
}

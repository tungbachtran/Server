using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Model.DatabaseModels
{
    public class Faculty
    {
        [Key]
        public string FacultyId { get; set; }
        public string Name { get; set; }
        List<Classroom> Classrooms { get; set; }
        List<User> Users { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace api.Model.DatabaseModels
{
    public class EducationalProgram
    {
        [Key]
        public string EducationalProgramId { get; set; }
        public string Name { get; set; }
    }
}

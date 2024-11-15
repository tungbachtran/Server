using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace api.Model.DatabaseModels
{
    public class Documents
    {
        [Key]
        public int documentId { get; set; }

        [Required]
        public string Content { get; set; }

        public byte[] Details { get; set; }
        public string? MimeType { get; set; }
        // Khóa ngoại tham chiếu đến CourseClassroom
        [ForeignKey("CourseClassroom")]
        public string CourseClassId { get; set; }

        // Điều hướng đến CourseClassroom
        public CourseClassroom CourseClassroom { get; set; }


    }
}

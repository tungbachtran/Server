using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace api.Model.DatabaseModels
{
    public class ChatMessages
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public string CourseClassId { get; set; }

        [Required]
        public string Content { get; set; }

        [ForeignKey("Sender")]
        public string? SenderId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual User? Sender { get; set; }
        public virtual CourseClassroom? CourseClassroom { get; set; }
    }
}

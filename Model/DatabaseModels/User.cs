using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace api.Model.DatabaseModels
{
    public class User
    {
        [Key]
        public string UserId { get; set; }
        public string Name { get; set; }
        public string? Dob { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool Gender { get; set; }
        public string ImageUrl { get; set; }
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.Always)]
        public Classroom Classroom { get; set; }
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.Always)]
        public string? ClassroomId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Faculty Faculty { get; set; }  // Navigation property
        [System.Text.Json.Serialization.JsonIgnore]
        public string? FacultyId {  get; set; }

        public ICollection<ChatMessages> ChatMessages { get; set; } = new List<ChatMessages>();

        public byte[]? ProfileImage { get; set; }
    }
}

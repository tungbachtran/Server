using Microsoft.EntityFrameworkCore;
using api.Model.DatabaseModels;
using System.Reflection.Metadata;

namespace api.Data
{
    public class StudentContext : DbContext
    {
        public StudentContext(DbContextOptions<StudentContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseEducationProgram>()
                .HasKey(courseEdu => new { courseEdu.CourseId, courseEdu.EducationalProgramId });
            modelBuilder.Entity<UserCourseClassroom>()
                .HasKey(userCourseClass => new { userCourseClass.UserId, userCourseClass.CourseClassroomId });
            modelBuilder.Entity<UserCourse>()
                .HasKey(userCourse => new { userCourse.UserId, userCourse.CourseId });

            modelBuilder.Entity<ChatMessages>()
       .HasOne(m => m.Sender)
       .WithMany(u => u.ChatMessages)
       .HasForeignKey(m => m.SenderId)
       .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatMessages>()
                .HasOne(m => m.CourseClassroom)
                .WithMany(cc => cc.ChatMessages)
                .HasForeignKey(m => m.CourseClassId)
                .OnDelete(DeleteBehavior.Cascade);


        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<EducationalProgram> EducationalProgram { get; set; }
        public DbSet<CourseEducationProgram> CourseEducationProgram { get; set; }
        public DbSet<CourseClassroom> CourseClassroom { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<UserCourseClassroom> UserCourseClassroom { get; set; }
        public DbSet<Classroom> Classroom { get; set; }
        public DbSet<Faculty> Faculty { get; set; }
        public DbSet<Score> Score { get; set; }
        public DbSet<UserCourse> UserCourse { get; set; }
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<Documents> Documents { get; set; }
        public DbSet<ChatMessages> ChatMessages { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Model.DatabaseModels;
using api.Model.DtoModels;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Account = api.Model.DatabaseModels.Account;

namespace api.Controllers
{

    [EnableCors("Allow CORS")]
    [Route("api/user")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly StudentContext _context;
        public static IWebHostEnvironment _enviroment;
        public UsersController(StudentContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _enviroment = environment;
        }

        // GET: api/Users
        [HttpGet]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            if (_context.User == null)
            {
                return NotFound();
            }

            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        // [Authorize]
        public async Task<ActionResult<object>> Get(string id)
        {
            // Lấy thông tin người dùng
            User userInformation = await _context.User.FindAsync(id);
            if (userInformation == null)
            {
                return NotFound("User not found");
            }
            //Chuyển đổi ProfileImage thành chuỗi base64 nếu có
            string profileImageBase64 = userInformation.ProfileImage != null
                ? Convert.ToBase64String(userInformation.ProfileImage)
                : null;
            //Kiểm tra nếu mã người dùng bắt đầu bằng "GV"
            if (id.StartsWith("GV"))
            {
                // Nếu mã bắt đầu bằng "GV", chỉ trả về thông tin người dùng
                return Ok(new { UserInformation = userInformation });
            }

            // Lấy thông tin lớp học
            Classroom classroom = await _context.Classroom.FindAsync(userInformation.ClassroomId);
            if (classroom == null)
            {
                return NotFound("Classroom not found");
            }

            // Lấy thông tin khoa
            Faculty faculty = await _context.Faculty.FindAsync(classroom.FacultyId);
            if (faculty == null)
            {
                return NotFound("Faculty not found");
            }

            // Lấy thông tin chương trình đào tạo
            EducationalProgram educationalProgram = await _context.EducationalProgram.FindAsync(classroom.EducationalProgramId);
            if (educationalProgram == null)
            {
                return NotFound("Educational Program not found");
            }

            // Trả về thông tin đầy đủ nếu mã không bắt đầu bằng "GV"
            var returnedInformation = new
            {
                UserInformation = userInformation,
                ProfileImage = profileImageBase64,
                ClassroomName = classroom.Name,
                EducationalProgram = educationalProgram,
                Faculty = faculty
            };

            return Ok(returnedInformation);
        }



        [HttpGet("class/{classroomId}")]
        //[Authorize]
        public async Task<ActionResult<List<User>>> GetAllStudentInClass(string classroomId)
        {
            var studentList = _context.User.Where(w => w.ClassroomId == classroomId).ToList();
            return Ok(studentList);
        }

        [HttpGet("faculty/{facultyId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Classroom>>> GetAllStudentInFaculty(string facultyId)
        {
            Faculty faculty = await _context.Faculty.FindAsync(facultyId);
            List<Classroom> classList = _context.Classroom.Where(w => w.FacultyId == faculty.FacultyId).ToList();
            foreach (var Class in classList)
            {
                List<User> tempList =
                    _context.User.Where(w => w.ClassroomId == Class.ClassroomId).ToList();
                Class.Students = tempList;
            }

            return Ok(classList);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{userId}")]
        //[Authorize]
        public async Task<IActionResult> PutUser(string userId, InformationDto request)
        {
            User newUserInformation = await _context.User.FindAsync(userId);
            newUserInformation.Name = request.Name;
            newUserInformation.Dob = request.Dob;
            newUserInformation.Email = request.Email;
            newUserInformation.Gender = request.Gender;
            newUserInformation.PhoneNumber = request.PhoneNumber;
            await _context.SaveChangesAsync();
            return Ok(newUserInformation);
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (_context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("course-class/{UserId}")]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult<List<CourseClassroom>>> GetAllCourseClassroom(string UserId)
        {
            var classList = (from w in _context.UserCourseClassroom
                where w.UserId == UserId
                select w).ToList();
            var resCourseClassList = new List<CourseClassroom>();
            foreach (var courseClassroom in classList)
            {
                CourseClassroom findingClass = await _context.CourseClassroom.FindAsync(courseClassroom.CourseClassroomId);
                resCourseClassList.Add(findingClass);
            }
            return Ok(resCourseClassList);
        }
        private bool UserExists(string id)
        {
            return (_context.User?.Any(e => e.UserId == id)).GetValueOrDefault();
        }

        public class FileUpLoadAPI
        {
            public IFormFile files { get; set; }
        }

        [HttpPost("image-upload/{userId}")]
        //[Authorize]
        public async Task<ActionResult<string>> UploadImage([FromForm] FileUpLoadAPI objFiles, string userId)
        {
            // Thay thế thông tin này bằng thông tin thực tế từ tài khoản Cloudinary của bạn
            var cloudName = "your_cloud_name";   // Cloud Name của bạn
            var apiKey = "your_api_key";          // API Key của bạn
            var apiSecret = "your_api_secret";    // API Secret của bạn

            // Khởi tạo đối tượng Cloudinary
            var account = new CloudinaryDotNet.Account(cloudName, apiKey, apiSecret);
            Cloudinary cloudinary = new Cloudinary(account);
            cloudinary.Api.Secure = true;

            User userInformation = _context.User.Find(userId);
            try
            {
                if (objFiles.files.Length > 0)
                {
                    string uploadDirectory = Path.Combine(_enviroment.WebRootPath, "Upload");
                    if (!Directory.Exists(uploadDirectory))
                    {
                        Directory.CreateDirectory(uploadDirectory);
                    }

                    string filePath = Path.Combine(uploadDirectory, userInformation.UserId);
                    using (FileStream fileStream = System.IO.File.Create(filePath))
                    {
                        await objFiles.files.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                    }

                    // Xóa hình ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(userInformation.ImageUrl))
                    {
                        var deletionParams = new DeletionParams(userInformation.UserId);
                        cloudinary.Destroy(deletionParams);
                    }

                    // Tải hình ảnh lên Cloudinary
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(filePath),
                        PublicId = $"{userInformation.UserId.Substring(0, 3)}/{userInformation.UserId}"
                    };
                    var uploadResult = cloudinary.Upload(uploadParams);

                    // Xóa tệp đã tải lên từ thư mục tạm
                    System.IO.File.Delete(filePath);

                    // Cập nhật URL hình ảnh cho người dùng
                    userInformation.ImageUrl = uploadResult.PublicId;
                    _context.SaveChanges();

                    return Ok(uploadResult.Url);
                }
                else
                {
                    return BadRequest("No files were uploaded.");
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("teacher/course-classroom/{teacherName}")]
        //[Authorize(Roles = "Teacher")]
        public async Task<ActionResult<List<ReturnedCourseClassroomDto>>> GetAllClassesOfTeacher(string teacherName)
        {
            List<CourseClassroom> courseClassrooms = _context.CourseClassroom
                .Where(courseClass => courseClass.TeacherName == teacherName).ToList();
            List<ReturnedCourseClassroomDto> resList = new List<ReturnedCourseClassroomDto>();
            foreach (var courseClass in courseClassrooms)
            {
                List<Schedule> schedules = _context.Schedule
                    .Where(schedule => schedule.CourseClassId == courseClass.CourseClassId).ToList();
                List<UserCourseClassroom> userCourseClassrooms = _context.UserCourseClassroom
                    .Where(userCourseClass => userCourseClass.CourseClassroomId == courseClass.CourseClassId)
                    .ToList();
                var tempData = new ReturnedCourseClassroomDto()
                {
                    CourseClassroom = courseClass, 
                    Schedule = schedules,
                    NumberOfRegisteredStudent = userCourseClassrooms.Count(),
                    TeacherName = teacherName
                };
                resList.Add(tempData);
            }

            return Ok(resList);
        }
    }
}

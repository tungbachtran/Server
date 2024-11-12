﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Model.DatabaseModels;
using api.Model.DtoModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using OfficeOpenXml;

namespace api.Controllers
{

    [EnableCors("Allow CORS")]
    [Route("api/education-program")]
    [ApiController]
    public class EducationalProgramsController : ControllerBase
    {
        private readonly StudentContext _context;
        public static IWebHostEnvironment _enviroment;

        public EducationalProgramsController(StudentContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _enviroment = environment;
        }

        // GET: api/EducationalPrograms
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EducationalProgram>>> GetEducationalProgram()
        {
          if (_context.EducationalProgram == null)
          {
              return NotFound();
          }
            return await _context.EducationalProgram.ToListAsync();
        }

        // GET: api/EducationalPrograms/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EducationalProgram>> GetEducationalProgram(string id)
        {
          if (_context.EducationalProgram == null)
          {
              return NotFound();
          }
            var educationalProgram = await _context.EducationalProgram.FindAsync(id);

            if (educationalProgram == null)
            {
                return NotFound();
            }

            return educationalProgram;
        }

        [HttpGet("course/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ReturnedInEPDTO>>> GetAllCourseInEducationalProgram(string id)
        {
            IEnumerable<CourseEducationProgram> courseEducationProgrames = _context.CourseEducationProgram.Where(courseEdu => courseEdu.EducationalProgramId == id).OrderBy(courseEdu => courseEdu.Semester).ToList();
            List<ReturnedInEPDTO> reslist = new List<ReturnedInEPDTO>();
            foreach (var courseEducation in courseEducationProgrames)
            {
                Course findingCourse = _context.Courses.Find(courseEducation.CourseId);

                var newDTO = new ReturnedInEPDTO
                {
                    Course = findingCourse,
                    Semester = courseEducation.Semester
                };
                reslist.Add(newDTO);
            }

            return Ok(reslist);
        }
        // POST: api/EducationalPrograms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<EducationalProgram>> PostEducationalProgram(CreateEducationalProgramDto request)
        {
          if (_context.EducationalProgram == null)
          {
              return Problem("Entity set 'ApplicationDbContext.EducationalProgram'  is null.");
          }
          var educationalProgram = new EducationalProgram()
          {
              EducationalProgramId = request.EducationalProgramId,
              Name = request.Name,
          };
            _context.EducationalProgram.Add(educationalProgram);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EducationalProgramExists(educationalProgram.EducationalProgramId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEducationalProgram", new { id = educationalProgram.EducationalProgramId }, educationalProgram);
        }

        // DELETE: api/EducationalPrograms/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEducationalProgram(string id)
        {
            if (_context.EducationalProgram == null)
            {
                return NotFound();
            }
            var educationalProgram = await _context.EducationalProgram.FindAsync(id);
            if (educationalProgram == null)
            {
                return NotFound();
            }

            _context.EducationalProgram.Remove(educationalProgram);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        //Add course to Educational Program
        [HttpPost("course")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<CourseEducationProgram>> AddCourse(AddCourseToEducationalProgramDto request)
        {
            Course course = _context.Courses.Find(request.CourseId);
            EducationalProgram educationalProgram = _context.EducationalProgram.Find(request.EducationalProgramId);
            if (course != null && educationalProgram != null)
            {
                var courseEducationalProgram = new CourseEducationProgram
                {
                    Course = course,
                    EducationalProgram = educationalProgram,
                    Semester = request.Semester,
                };
                _context.CourseEducationProgram.Add(courseEducationalProgram);
                _context.SaveChanges(); 
                return Ok(courseEducationalProgram);
            }

            return NotFound();
        }

        //Remove course from Educational Program
        [HttpDelete("course")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<CourseEducationProgram>> DeleteCourse(DeleteCourseFromEducationalProgramDto request)
        {
            CourseEducationProgram courseEducationProgram = _context.CourseEducationProgram
                .FirstOrDefault(item => item.CourseId == request.CourseId && item.EducationalProgramId == request.EducationalProgramId);
            if (courseEducationProgram != null)
            {
                _context.CourseEducationProgram.Remove(courseEducationProgram);
                try
                {
                    _context.SaveChanges();
                    return Ok("Deleted");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return NotFound();
        }


        public class FileUpLoadAPI
        {
            public IFormFile files { get; set; }
        }
        [HttpPost("upload-file")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<CreateCourseDto>>> UploadTask([FromForm] FileUpLoadAPI data)
        {
            //download file from client
            if (data.files.Length > 0)
            {
                if (!Directory.Exists(_enviroment.WebRootPath + "\\Download\\"))
                {
                    Directory.CreateDirectory(_enviroment.WebRootPath + "\\Download\\");
                }

                using (FileStream fileStream =
                       System.IO.File.Create(_enviroment.WebRootPath + "\\Download\\" + data.files.FileName))
                {
                    data.files.CopyTo(fileStream);
                    fileStream.Flush();
                }

                //work with excel file
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(_enviroment.WebRootPath + "\\Download\\" + data.files.FileName);
                ExcelPackage excelPackage = new ExcelPackage(fileInfo);
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                List<AddCourseToEducationalProgramDto> request = new List<AddCourseToEducationalProgramDto>();
                List<CourseEducationProgram> courses = new List<CourseEducationProgram>();
                int rows = worksheet.Dimension.Rows;
                for (int i = 2; i <= rows; i++)
                {
                    var newCourse = new AddCourseToEducationalProgramDto()
                    {
                        CourseId = worksheet.Cells[i, 3].Text,
                        EducationalProgramId = worksheet.Cells[i,2].Text,
                        Semester = Convert.ToInt32(worksheet.Cells[i,1].Text)
                    };
                    request.Add(newCourse);
                }

                foreach (var item in request)
                {
                    Course course = _context.Courses.Find(item.CourseId);
                    EducationalProgram educationalProgram = _context.EducationalProgram.Find(item.EducationalProgramId);
                    if (course != null && educationalProgram != null)
                    {
                        if (CourseEducationalProgramExists(item.CourseId, item.EducationalProgramId) == false)
                        {
                            CourseEducationProgram courseEducationalProgram = new CourseEducationProgram();
                            courseEducationalProgram.Course = course;
                            courseEducationalProgram.EducationalProgram = educationalProgram;
                            courseEducationalProgram.Semester = item.Semester;
                            courseEducationalProgram.CourseId = item.CourseId;
                            courseEducationalProgram.EducationalProgramId = item.EducationalProgramId;
                            courses.Add(courseEducationalProgram);
                        }
                    }
                }

                _context.CourseEducationProgram.AddRange(courses);
                try
                {
                    _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    throw;
                }
                return Ok(courses);
            }

            return BadRequest();
        }
        private bool EducationalProgramExists(string id)
        {
            return (_context.EducationalProgram?.Any(e => e.EducationalProgramId == id)).GetValueOrDefault();
        }

        private bool CourseEducationalProgramExists(string courseId, string educationalProgramId)
        {
            return (_context.CourseEducationProgram?.Any(e => e.EducationalProgramId == educationalProgramId && e.CourseId == courseId)).GetValueOrDefault();
        }
    }
}

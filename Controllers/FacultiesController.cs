﻿using System;
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

namespace api.Controllers
{

    [EnableCors("Allow CORS")]
    [Route("api/faculty")]
    [ApiController]
    public class FacultiesController : ControllerBase
    {
        private readonly StudentContext _context;

        public FacultiesController(StudentContext context)
        {
            _context = context;
        }

        // GET: api/Faculties
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Faculty>>> GetFaculty()
        {
          if (_context.Faculty == null)
          {
              return NotFound();
          }
            return await _context.Faculty.ToListAsync();
        }

        // GET: api/Faculties/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Faculty>> GetFaculty(string id)
        {
          if (_context.Faculty == null)
          {
              return NotFound();
          }
            var faculty = await _context.Faculty.FindAsync(id);

            if (faculty == null)
            {
                return NotFound();
            }

            return faculty;
        }

        [HttpGet("classes/{facultyId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Classroom>>> GetAllClassesInFaculty(string facultyId)
        {
            List<Classroom> classrooms =
                _context.Classroom.Where(classroom => classroom.FacultyId == facultyId).ToList();
            return Ok(classrooms);
        }

       
        [HttpGet("users/{facultyId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByFacultyId(string facultyId)
        {
            List<User> users =
               _context.User.Where(user => user.FacultyId == facultyId).ToList();
            return Ok(users);
        }



        // POST: api/Faculties
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Faculty>> PostFaculty(CreateFacultyDto request)
        {
          if (_context.Faculty == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Faculty'  is null.");
          }

          var faculty = new Faculty
          {
              FacultyId = request.FacultyId,
              Name = request.Name,
          };
            _context.Faculty.Add(faculty);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FacultyExists(faculty.FacultyId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFaculty", new { id = faculty.FacultyId }, faculty);
        }

        

        // DELETE: api/Faculties/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFaculty(string id)
        {
            if (_context.Faculty == null)
            {
                return NotFound();
            }
            var faculty = await _context.Faculty.FindAsync(id);
            if (faculty == null)
            {
                return NotFound();
            }

            _context.Faculty.Remove(faculty);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FacultyExists(string id)
        {
            return (_context.Faculty?.Any(e => e.FacultyId == id)).GetValueOrDefault();
        }
    }
}

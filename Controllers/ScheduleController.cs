using api.Data;
using api.Model.DatabaseModels;
using api.Model.DtoModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/schedules")]
    [EnableCors("Allow CORS")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly StudentContext _context;

        public ScheduleController(StudentContext context)
        {
            _context = context;
        }

        [HttpGet("{courseClassId}")]
        public async Task<ActionResult<List<Schedule>>> Get(string courseClassId)
        {
            var schedules = await _context.Schedule
                .Where(w => w.CourseClassId == courseClassId)
                .ToListAsync();

            if (schedules == null || !schedules.Any())
                return NotFound();

            return Ok(schedules);
        }

        [HttpPut("{scheduleId}")]
        public async Task<ActionResult<Schedule>> Update(CreateScheduleDto request, int scheduleId)
        {
            var schedule = await _context.Schedule.FindAsync(scheduleId);
            if (schedule == null)
                return NotFound();

            var courseClassroom = await _context.CourseClassroom.FindAsync(request.courseClassId);
            if (courseClassroom == null)
                return BadRequest("Course classroom does not exist");

            // Cập nhật từng thuộc tính
            schedule.dateInWeek = request.dateInWeek;
            schedule.startPeriod = request.startPeriod;
            schedule.endPeriod = request.endPeriod;
            schedule.Room = request.Room;
            schedule.CourseClassroom = courseClassroom;
            schedule.CourseClassId = courseClassroom.CourseClassId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return Ok(schedule);
        }

        [HttpPost]
        public async Task<ActionResult<Schedule>> PostSchedule(CreateScheduleDto request)
        {
            // Tìm kiếm CourseClassroom dựa trên courseClassId
            var courseClassroom = await _context.CourseClassroom.FindAsync(request.courseClassId);
            if (courseClassroom == null)
                return BadRequest("Course classroom does not exist");

            // Kiểm tra xung đột lịch
            var existingSchedules = await _context.Schedule
                .Where(s => s.Room == request.Room &&
                            s.dateInWeek == request.dateInWeek &&
                            // Kiểm tra xung đột thời gian
                            (s.startPeriod <= request.endPeriod && s.endPeriod >= request.startPeriod))
                .ToListAsync();

            if (existingSchedules.Count > 0)
            {
                return BadRequest("Conflicting schedule exists in the same room at this time.");
            }

            // Tạo lịch mới
            var newSchedule = new Schedule
            {
                dateInWeek = request.dateInWeek,
                startPeriod = request.startPeriod,
                endPeriod = request.endPeriod,
                Room = request.Room,
                CourseClassroom = courseClassroom,
                CourseClassId = courseClassroom.CourseClassId,
            };

            // Thêm lịch mới vào cơ sở dữ liệu
            await _context.Schedule.AddAsync(newSchedule);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return CreatedAtAction(nameof(PostSchedule), new { id = newSchedule.Id }, newSchedule);
        }
    }
}

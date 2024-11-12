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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using api.Hubs;
namespace api.Controllers
{
    [EnableCors("Allow CORS")]
    [Route("api/messages")]
    [ApiController]
    public class MessagesController : ControllerBase
        
    {
        private readonly StudentContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IHubContext<ChatHub> _chatHub;
        public MessagesController(StudentContext context, IWebHostEnvironment environment, IHubContext<ChatHub> chatHub)
        {
            _context = context;
            _environment = environment;
            _chatHub = chatHub;
        }

        // Phương thức POST: Tạo tin nhắn mới
        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] MessageDto messageDto)
        {
            if (messageDto == null || string.IsNullOrWhiteSpace(messageDto.Content))
            {
                return BadRequest("Nội dung tin nhắn không hợp lệ.");
            }

            var newMessage = new ChatMessages
            {
                CourseClassId = messageDto.CourseClassId,
                SenderId = messageDto.SenderId,
                Content = messageDto.Content,
                Timestamp = DateTime.Now
            };

            await _context.ChatMessages.AddAsync(newMessage);
            await _context.SaveChangesAsync();

           

            return CreatedAtAction(nameof(GetMessageById), new { id = newMessage.MessageId }, newMessage);
        }

        // Phương thức GET: Lấy danh sách tin nhắn theo CourseClassId
        [HttpGet("{courseClassId}")]
        public async Task<IActionResult> GetMessagesByCourseClassId(string courseClassId)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.CourseClassId == courseClassId)
                .OrderBy(m => m.Timestamp)
                .Select(m => new
                {
                    m.MessageId,
                    m.CourseClassId,
                    m.SenderId,
                    m.Content,
                    m.Timestamp
                })
                .ToListAsync();

            if (messages == null || messages.Count == 0)
            {
                return Ok(new { messages = new List<string>(), message = "Không có tin nhắn nào cho lớp học phần này." });
            }

            return Ok(messages);
        }

        // Phương thức GET: Lấy tin nhắn theo MessageId
        [HttpGet("message/{id}")]
        public async Task<IActionResult> GetMessageById(int id)
        {
            var message = await _context.ChatMessages.FindAsync(id);
            if (message == null)
            {
                return NotFound("Tin nhắn không tồn tại.");
            }

            return Ok(message);
        }
    }
}

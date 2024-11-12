using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using api.Data;
using api.Model.DatabaseModels;
using Microsoft.EntityFrameworkCore;
namespace api.Hubs
{
    [EnableCors("Allow CORS")]
    public class ChatHub : Hub
    {
        private readonly StudentContext _context;

        public ChatHub(StudentContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string senderId, string content, string courseClassId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Nội dung tin nhắn không hợp lệ.");
            }
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == senderId);
            if (user == null)
            {
                throw new ArgumentException("Người dùng không tồn tại.");
            }
            // Tạo tin nhắn mới
            var newMessage = new ChatMessages
            {
                CourseClassId = courseClassId,
                SenderId = senderId,
                Content = content,
                Timestamp = DateTime.Now
            };

            try
            {
                // Lưu tin nhắn vào cơ sở dữ liệu
                await _context.ChatMessages.AddAsync(newMessage);
                await _context.SaveChangesAsync();

                // Gửi tin nhắn đến các client khác
                await Clients.All.SendAsync("ReceiveMessage", user.Name, content, newMessage.Timestamp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                throw;
            }
        }
    }
}

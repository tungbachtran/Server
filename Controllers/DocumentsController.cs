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
using OfficeOpenXml;

namespace api.Controllers
{

    [EnableCors("Allow CORS")]
    [Route("api/documents")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly StudentContext _context;
        public static IWebHostEnvironment _enviroment;

        public DocumentsController(StudentContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _enviroment = environment;
        }


        [HttpGet("{courseClassId}")]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocumentsByCourseClassId(string courseClassId)
        {
            var documents = await _context.Documents
                                           .Where(d => d.CourseClassId == courseClassId)
                                           .Select(d => new DocumentDto // Sử dụng DTO để chỉ lấy thông tin cần thiết
                                           {
                                               DocumentId = d.documentId,
                                               Content = d.Content,
                                               Details = d.Details,
                                               MimeType = d.MimeType
                                               // Chỉ lưu thông tin chi tiết mà bạn muốn hiển thị
                                           })
                                           .ToListAsync();

            if (documents == null || documents.Count == 0)
            {
                return NotFound("Không tìm thấy tài liệu cho CourseClassId này.");
            }

            return Ok(documents);
        }




        [HttpPost("{courseClassId}")]
        public async Task<ActionResult<Documents>> AddDocument(string courseClassId, [FromForm] string Content, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Tệp không hợp lệ.");
            }

            using (var memoryStream = new MemoryStream())
            {
                // Sao chép nội dung file vào memoryStream
                await file.CopyToAsync(memoryStream);

                // Tạo document mới và lưu thông tin
                var document = new Documents
                {
                    CourseClassId = courseClassId,
                    Content = Content, // Tên hoặc mô tả tài liệu
                    Details = memoryStream.ToArray(), // Lưu nội dung tệp dưới dạng byte[]
                    MimeType = file.ContentType // Lưu kiểu MIME của tệp
                };

                // Thêm tài liệu vào ngữ cảnh và lưu vào cơ sở dữ liệu
                _context.Documents.Add(document);
                await _context.SaveChangesAsync();

                // Trả về tài liệu đã được tạo, bao gồm MimeType
                return CreatedAtAction(nameof(GetDocumentsByCourseClassId), new { courseClassId = courseClassId }, new
                {
                    document.documentId,
                    document.Content,
                    document.MimeType
                });
            }
        }









        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound("Tài liệu không tồn tại.");
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool DocumentExists(int id)
        {
            return _context.Documents.Any(e => e.documentId == id);
        }


    }
}
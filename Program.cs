using Microsoft.EntityFrameworkCore;
using api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using api.Hubs;
var builder = WebApplication.CreateBuilder(args);
var key = builder.Configuration.GetSection("AppSettings:Token").Value;
var keyBytes = Encoding.UTF8.GetBytes(key);

// Thêm dịch vụ DbContext với chuỗi kết nối từ appsettings.json
builder.Services.AddDbContext<StudentContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm dịch vụ cho Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Student Management API", Version = "v1" });

    // Thêm định nghĩa bảo mật cho JWT
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Thêm dịch vụ controller
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;  // Cho phép khi chạy trên HTTP
    options.SaveToken = true;  // Lưu token trong HttpContext
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Allow CORS", builder =>
    {
        builder.WithOrigins("http://localhost:3000") // Chỉ định rõ các nguồn được phép
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("Access-Control-Allow-Headers")
               .AllowCredentials(); // Cho phép cookie và xác thực
    });
});

var app = builder.Build();

// Middleware CORS để thêm tiêu đề CORS


// Sử dụng Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Management API v1");
        c.RoutePrefix = "swagger"; // Đặt Swagger UI ở route /swagger
    });
}

app.UseHttpsRedirection();
app.UseRouting();

// Áp dụng CORS sau UseRouting và trước các middleware khác
app.UseCors("Allow CORS");

app.UseStaticFiles();
app.UseDefaultFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseEndpoints(endpoints =>
{

    endpoints.MapHub<ChatHub>("/chatHub"); // Định tuyến cho SignalR hub
});
app.Run();

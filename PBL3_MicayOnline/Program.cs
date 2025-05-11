using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Services.Implementations;
using PBL3_MicayOnline.Services.Interfaces;
using System.Security.Claims;


//using PBL3_MicayOnline.Services.Implementations;
//using PBL3_MicayOnline.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔐 Cấu hình JWT Settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Đăng ký Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Kết nối DB
builder.Services.AddDbContext<Pbl3Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PBL3Db")));

// 🧾 Thêm Controller + Swagger + JWT hỗ trợ
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PBL3_MicayOnline API", Version = "v1" });

    // ✅ Swagger hỗ trợ JWT Authorize
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập chuỗi token theo định dạng: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// 🔒 Cấu hình JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
            RoleClaimType = ClaimTypes.Role
        };
        // log neeys lỗi
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("❌ JWT lỗi: " + context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// ⚙️ Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();



// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// 🛡 Bảo mật
app.UseAuthentication();
app.UseAuthorization();

// Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

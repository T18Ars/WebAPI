using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyWebAPI.Data;
using MyWebAPI.Models;
using MyWebAPI.Services.Loai;
using System;
using System.Text;

namespace MyWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            #region dbcontext
            /*
             * Singleton
             * Khi project, webapi của mình start thì đã khai báo DB Context và lấy chuỗi kết nối
            */
            services.AddDbContext<MyDbContext>(option =>
            {
                option.UseSqlServer(Configuration.GetConnectionString("MyDB"));
            });
            #endregion dbcontext

            services.AddScoped<ILoaiRepository, LoaiRepository>();

            /*
             * sử dụng config này để tự động map dữ liệu tương ứng từ file appsetting.json sang file AppSetting.cs
             */
            services.Configure<AppSetting>(Configuration.GetSection("AppSettings"));

            #region authentication
            var secretKey = Configuration["AppSettings:SecretKey"];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt => {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false, // Có xác thực nhà phát hành của token ko
                        ValidateAudience = false, // Có xác thực người nhận của token ko
                        ValidateIssuerSigningKey = true, // Có xác thực khóa ký của nhà phát hành token ko
                        ValidateLifetime = true, // Có kiểm tra token đã hết hạn hay ko

                        ValidIssuer = "nhannv", // Nhà phát hành hợp lệ của token
                        ValidAudience = "user", // Người nhận hợp lệ của token
                        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes), // Khóa bí mật được sử dụng để ký token
                        ClockSkew = TimeSpan.Zero
                    };
                });
            #endregion authentication

            #region Swagger
            // Net5 trở đi mặc định khai báo sẵn swagger
            // Từ bản Net6 trở đi 2 file Program và Startup sẽ gộp làm 1
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyWebAPI", Version = "v1" });
            });
            #endregion Swagger
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyWebAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

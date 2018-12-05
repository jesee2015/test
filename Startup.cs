using System.Text;
using JwtBearerAuthentication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace JwtBearerAuthentication
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
            var jwtSettings = new JwtSettings();
            this.Configuration.Bind("JwtSettings",jwtSettings);

            services.AddAuthentication(options=>{
                //认证middleware配置
                options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o=>{
                //主要是jwt  token参数设置
                o.TokenValidationParameters=new Microsoft.IdentityModel.Tokens.TokenValidationParameters{
　　　　　　　　　　　　//Token颁发机构
                    ValidIssuer =jwtSettings.Issuer,
　　　　　　　　　　　　//颁发给谁
                    ValidAudience =jwtSettings.Audience,
                    //这里的key要进行加密，需要引用Microsoft.IdentityModel.Tokens
                    IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
　　　　　　　　　　　　//ValidateIssuerSigningKey=true,
　　　　　　　　　　　　////是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
　　　　　　　　　　　　//ValidateLifetime=true,
　　　　　　　　　　　　////允许的服务器时间偏移量
　　　　　　　　　　　　//ClockSkew=TimeSpan.Zero
                    };
                });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}

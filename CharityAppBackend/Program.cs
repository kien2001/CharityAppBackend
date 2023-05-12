using Base;
using CharityAppBL.Charity;
using CharityAppBL.Login;
using CharityAppBL.Setting;
using CharityAppBL.Upload;
using CharityAppBL.Users;
using CharityAppDL.Charity;
using CharityAppDL.Setting;
using CharityAppDL.User;
using CharityAppDL.Users;
using CharityBackendDL;
using Login;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Validation;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                      });
});


builder.Services.AddRazorPages();
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Charity WEB APP API",
        Description = "An ASP.NET Core Web API for managing charity activities",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBLLogin, BLLogin>();
builder.Services.AddScoped<IDLLogin, DLLogin>();
builder.Services.AddScoped<IDLSetting, DLSetting>();
builder.Services.AddScoped<IBLSetting, BLSetting>();
builder.Services.AddScoped<IBLAccount, BLAccount>();
builder.Services.AddScoped<IDLAccount, DLAccount>();
builder.Services.AddScoped<IBLUpload, BLUpload>();
builder.Services.AddScoped<IDLBase, DLBase>();
builder.Services.AddScoped<IBLCharity, BLCharity>();
builder.Services.AddScoped<IDLCharity, DLCharity>();
builder.Services.AddScoped<IBLUser, BLUser>();
builder.Services.AddScoped<IDLUser, DLUser>();

builder.Services.AddScoped<JwtTokenValidator>();

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Keys
                                .SelectMany(key => context.ModelState[key].Errors.Select(x => $"{key}: {x.ErrorMessage}"))
                                .ToArray();

        var apiError = new
        {
            Message = "Validation Error",
            Result = errors,
            StatusCode = 400
        };

        var result = new ObjectResult(apiError);
        result.StatusCode = 400;
        result.ContentTypes.Add(MediaTypeNames.Application.Json);

        return result;
    };
});

var jwtSettings = builder.Configuration.GetSection("Jwt");
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    RequireExpirationTime = false,
    ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
    ValidAudience = jwtSettings.GetValue<string>("Audience"),
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.GetValue<string>("Key"))),

    // Allow to use seconds for expiration of token
    // Required only when token lifetime less than 5 minutes
    // THIS ONE
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(jwt =>
    {
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = tokenValidationParameters;
        jwt.SecurityTokenValidators.Clear(); // Clear default token validators
        jwt.SecurityTokenValidators.Add(new JwtTokenValidator(
            builder.Services.BuildServiceProvider().GetRequiredService<IDistributedCache>(),
            builder.Services.BuildServiceProvider().GetRequiredService<IDLLogin>()
        ));
    });
builder.Services.AddAuthorization(options =>
{
    var AdminRole = Enum.GetName(typeof(RoleUser), (int)RoleUser.Admin);
    var UserCharityRole = Enum.GetName(typeof(RoleUser), (int)RoleUser.UserCharity);
    var UserNormalRole = Enum.GetName(typeof(RoleUser), (int)RoleUser.UserNormal);
    if(AdminRole != null && UserCharityRole != null && UserNormalRole != null)
    {
        options.AddPolicy(AdminRole, policy => policy.RequireRole(AdminRole));
        options.AddPolicy(UserCharityRole, policy => policy.RequireRole(UserCharityRole));
        options.AddPolicy(UserNormalRole, policy => policy.RequireRole(UserNormalRole));
    }
});
builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("CacheSettings").GetValue<string>("ConnectionString");
});

DatabaseContext.ConnectionString = builder.Configuration.GetConnectionString("MySQLConnection");
DatabaseContext.ConfigJwt = builder.Configuration.GetSection("Jwt");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(MyAllowSpecificOrigins);

//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//    endpoints.MapRazorPages();
//}

//);
//app.UseCorsMiddleware();

app.MapControllers();

app.Run();

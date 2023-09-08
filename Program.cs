global using AutoMapper;
global using griffined_api.Data;
global using griffined_api.Dtos.AddressDtos;
global using griffined_api.Dtos.AttendanceDtos;
global using griffined_api.Dtos.AvailableScheduleDtos;
global using griffined_api.Dtos.CourseDtos;
global using griffined_api.Dtos.General;
global using griffined_api.Dtos.ParentDtos;
global using griffined_api.Dtos.PreferredDayDtos;
global using griffined_api.Dtos.ProfilePictureDto;
global using griffined_api.Dtos.StaffDtos;
global using griffined_api.Dtos.StudentDtos;
global using griffined_api.Dtos.StudentAddtionalFilesDtos;
global using griffined_api.Dtos.TeacherDtos;
global using griffined_api.Dtos.UserDtos;
global using griffined_api.Dtos.WorkTimeDtos;
global using griffined_api.Enums;
global using griffined_api.Exceptions;
global using griffined_api.Integrations.Firebase;
global using griffined_api.Jobs;
global using griffined_api.Middlewares;
global using griffined_api.Models;
global using griffined_api.Services.AttendanceService;
global using griffined_api.Services.ClassCancellationRequestService;
global using griffined_api.Services.CheckAvailableService;
global using griffined_api.Services.CourseService;
global using griffined_api.Services.StaffService;
global using griffined_api.Services.StudentService;
global using griffined_api.Services.StudentReportService;
global using griffined_api.Services.TeacherService;
global using griffined_api.Services.RegistrationRequestService;
global using griffined_api.Services.StudyCourseService;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using System.ComponentModel.DataAnnotations;

//Authen
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.AspNetCore.Authorization;
global using System.Security.Claims;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.Extensions.Options;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using System.IdentityModel.Tokens.Jwt;
global using Google.Cloud.Firestore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using FirebaseAdmin;

// Background Tasks
using Quartz;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(Environment.GetEnvironmentVariable("REMOTE_DB")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLogging();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IClassCancellationRequestService, ClassCancellationRequestService>();
builder.Services.AddScoped<ICheckAvailableService, CheckAvailableService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IFirebaseService, FirebaseService>();
builder.Services.AddScoped<IRegistrationRequestService, RegistrationRequestService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentReportService, StudentReportService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IStudyCourseService, StudyCourseService>();
builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"Bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddSingleton(FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"))
}));
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, (o) => { });

builder.Services.AddSingleton(_ => UrlSigner.FromCredential(GoogleCredential.FromFile(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"))));

var storageClient = StorageClient.Create();
builder.Services.AddSingleton(_ => StorageClient.Create());

builder.Services.AddQuartzQuartzInfrastructure();

builder.Services.AddSignalR();

builder.Services.ConfigureSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "House of Griffin",
        Version = "v1.0.2"
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();

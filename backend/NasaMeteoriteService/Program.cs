using Microsoft.EntityFrameworkCore;
using NasaMeteoriteService.Handlers;
using NasaMeteoriteSomeServices.Services.Implementations;
using NasaMeteoriteSomeServices.Services.Interfaces;
using Quartz;
using Shared.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMeteoriteRepository, MeteoriteRepository>();
builder.Services.AddScoped<IMeteoriteSyncService, MeteoriteSyncService>();
builder.Services.AddScoped<IMeteoriteService, MeteoriteService>();
builder.Services.AddScoped<MeteoriteDomainService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<INasaApiClient, NasaApiClient>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot";
});

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("DataSyncJob");

    q.AddJob<DataSyncJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("DataSyncJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(5)
            .RepeatForever()));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddTransient<DataSyncJob>();

var app = builder.Build();

app.UseCors("AllowAll");

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "wwwroot";
});

app.MapControllers();

await app.RunAsync();

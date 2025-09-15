using Calendar.Data;
using Calendar.Services;
using Calendar.Repositories;
using Calendar.GetRecurrence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<EventTaskService>();

builder.Services.AddScoped<AppointmentsService>();

builder.Services.AddScoped<ITasksRepo, TasksRepo>();
builder.Services.AddScoped<IEventsRepo, EventsRepo>();

builder.Services.AddScoped<IRecurrence, Recurrence>();

// sample
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
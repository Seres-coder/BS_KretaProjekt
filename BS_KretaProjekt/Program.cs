using BS_KretaProjekt.Model;
using BS_KretaProjekt.Persistence;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContextPool<KretaDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("KreateDb")));
builder.Services.AddTransient<DataModel>();
builder.Services.AddTransient<GradeModel>();
builder.Services.AddTransient<MessageModel>();
builder.Services.AddTransient<TimeTableModel>();
builder.Services.AddTransient<UserModel>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using Autenticacao.DBContext;
using Autenticacao.Services.IRepository;
using Autenticacao.Services.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IAutenticacaoRepository, AutenticacaoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

var jwtKey = builder.Configuration["Jwt:Key"];
builder.Services.AddJwtAuthentication(jwtKey);

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

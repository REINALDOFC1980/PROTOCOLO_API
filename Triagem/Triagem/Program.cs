using Triagem.DBContext;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Triagem.Service.IRepository;
using Triagem.Service.Repositoty;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();

// Configura��o do contexto de banco de dados da primeira API
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IProcessoServices, ProcessoServices>();
builder.Services.AddScoped<IRedistribuicaoRepository, RedistribuicaoRepository>();

/// JWT
var jwtKey = builder.Configuration["Jwt:Key"];
builder.Services.AddJwtAuthentication(jwtKey);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //c.IncludeXmlComments(xmlPath);

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Protocolo API",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "Reinaldo Cordeiro",
            Email = "reinaldo@gruporecursos.com.br",
        }
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
app.UseMiddleware<TryCatchMiddleware>();

app.MapControllers();

app.Run();

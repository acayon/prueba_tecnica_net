using Microsoft.OpenApi.Models;
using NetTechnicalTest.Clients;
using NetTechnicalTest.DataAccess;
using NetTechnicalTest.IClients;
using NetTechnicalTest.IDataAccess;
using NetTechnicalTest.IServices;
using NetTechnicalTest.Services;
using OpenAPIClient;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

//Establece la forma de resolver la inyecci�n de dependencias de los distintos servicios que usa la aplicaci�n
builder.Services.AddSingleton(provider => new HttpClient { BaseAddress = new Uri("http://localhost:5245/") });
builder.Services.AddSingleton<DemoClient>();
builder.Services.AddSingleton<IPricesClient, PricesClient>();
builder.Services.AddSingleton<IMarketPartiesClient, MarketPartiesClient>();
builder.Services.AddSingleton<IDBAccess, MongoDBAccess>();
builder.Services.AddSingleton<IDataService, DataService>();

//A�ade los servicios para los controladores
builder.Services.AddControllers();

//A�ade Mvc para que se pueda auto exponer los servicios API Rest del controlador
builder.Services.AddMvc();
//Configura la auto generaci�n del contenido de los servicios API Rest con Swagger
builder.Services.AddSwaggerGen(options =>
{
    //Establece la informaci�n de la cabecera
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "NetTechnicalTest API",
        Description = "API generada para la prueba t�nica .NET",
        TermsOfService = new Uri("https://localhost:5245/terms"),
        Contact = new OpenApiContact
        {
            Name = "Ejemplo de Contacto",
            Url = new Uri("https://localhost:5245/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Ejemplo de Licencia",
            Url = new Uri("https://localhost:5245/license")
        }
    });

    //Incluye los comentarios de documentaci�n de los m�todos como informaci�n para los m�todos del servicio API Rest
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

//Despliega los controladores en el servidor para hacerlos disponibles a los clientes
app.MapControllers();

//Despliega los servicios API Rest configurados a partir de los controladores
app.MapSwagger();

//Configura la informaci�n del midelware de Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

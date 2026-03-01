var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<AddProcedureToPlanCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddUserToPlanProcedureCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RemoveUserFromPlanProcedureCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddSqlite<RLContext>(builder.Configuration.GetConnectionString(StaticKeys.DefaultConnectionString) ?? throw new InvalidOperationException("Default connection string not configured"));
builder.Services.AddControllers()
    .AddOData(options => options.Select().Filter().Expand().OrderBy())
    .AddJsonOptions(options => options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase);

// api versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;

    // Url-based versioning: api/v1/..
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    //Filter detects endpoints with [EnableQuery] and adds OData query parameters to the Swagger UI
    c.OperationFilter<EnableQueryFilter>();
});
string corsPolicy = StaticKeys.AllowLocal;
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy,
    policy =>
    {
        policy.WithOrigins(builder.Configuration.GetValue<string>(StaticKeys.ReactAppBaseUrl) ?? throw new InvalidOperationException("React App Base Url is not configured")) //Allows React app to access the API
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RL v1");
        c.RoutePrefix = string.Empty;
    });
}
app.UseHttpsRedirection();

app.UseCors(corsPolicy);
// Global exception middleware registered here (before authorization)
app.UseGlobalExceptionalHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();
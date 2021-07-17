### Union Architecture using ASP.Net Core & NHibernate

#### Entity Layer

IEntity interface
```c#
public interface IEntity
{
    int Id { get; set; }
}
```

`Samurai` Class

```c#
using FluentNHibernate.Mapping;

public class Samurai : IEntity
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; }

    public virtual ISet<Quote> Quotes { get; set; } = new HashSet<Quote>();
}


public class SamuraiMap : ClassMap<Samurai>
{
    public SamuraiMap()
    {
        Table("samurais");
        Schema("public");

        Id(x => x.Id).Column("id").GeneratedBy.Increment();
        Map(x => x.Name).Column("name");

        // One-to-Many
        HasMany(x => x.Quotes).Inverse().Cascade.All(); 
    }
}
```

`Quote` Class

```c#
using FluentNHibernate.Mapping;

public class Quote : IEntity
{
    public virtual int Id { get; set; }

    public virtual string Text { get; set; }

    public virtual Samurai Samurai { get; set; }
}

public class QuoteMap : ClassMap<Quote>
{
    public QuoteMap()
    {
        Table("quotes");
        Schema("public");

        Id(x => x.Id).Column("id").GeneratedBy.Increment();
        Map(x => x.Text).Column("text");

        References(x => x.Samurai).Column("samurai_id");
    }
}
```

#### Repository Layer

`IGenericRepository` interface

```c#
using System.Linq;
using System.Threading.Tasks;
using NHibernateDemo.Entity.Models;

public interface IGenericRepository<TEntity> where TEntity : class, IEntity
{
    IQueryable<TEntity> GetAll();

    Task<TEntity> GetById(int id);

    Task Create(TEntity entity);

    Task Update(int id, TEntity entity);

    Task Delete(TEntity entity);

    void BeginTransaction();
    Task Commit();
    Task Rollback();
    void CloseTransaction();
}
```

`GenericRepository` repository class
Note: DI for `ISession` is done via Startup class of the WebApi Project.

```c#
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernateDemo.Entity.Models;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly ISession _session;
        private ITransaction _transaction;

        public GenericRepository(ISession session)
        {
            _session = session;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _session.Query<TEntity>();
        }

        public Task<TEntity> GetById(int id)
        {
            return _session.GetAsync<TEntity>(id);
        }

        public Task Create(TEntity entity)
        {
           return _session.SaveAsync(entity);
        }

        public Task Update(int id, TEntity entity)
        {
            return _session.UpdateAsync(entity, id);
        }

        public Task Delete(TEntity entity)
        {
            return _session.DeleteAsync(entity);
        }

        public void BeginTransaction()
        {
            _transaction = _session.BeginTransaction();
        }

        public async Task Commit()
        {
            await _transaction.CommitAsync();
        }

        public async Task Rollback()
        {
            await _transaction.RollbackAsync();
        }

        public void CloseTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }
```

#### Service Layer

`ISamuraiService` interface

```c#
using System.Linq;
using System.Threading.Tasks;
using NHibernateDemo.Entity.Models;

public interface ISamuraiService
{
    IQueryable<Samurai> GetAll();

    Task<Samurai> GetById(int id);

    Task Create(Samurai entity);

    Task Update(int id, Samurai entity);

    Task Delete(Samurai entity);
}
```

`SamuraiService` service class


```c#
using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernateDemo.Data;
using NHibernateDemo.Entity.Models;

public class SamuraiService : ISamuraiService
{
    private readonly IGenericRepository<Samurai> _repository;
    public SamuraiService(ISession session, IGenericRepository<Samurai> repository)
    {
        _repository = repository;
    }
    public IQueryable<Samurai> GetAll()
    {
        return _repository.GetAll();
    }

    public Task<Samurai> GetById(int id)
    {
        return _repository.GetById(id);
    }

    public async Task Create(Samurai samurai)
    {
        try
        {
            _repository.BeginTransaction();
            await _repository.Create(samurai);
            await _repository.Commit();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await _repository.Rollback();
            throw;
        }
        finally
        {
            _repository.CloseTransaction();
        }
    }

    public async Task Update(int id, Samurai samurai)
    {
        try
        {
            _repository.BeginTransaction();
            await _repository.Update(id, samurai);
            await _repository.Commit();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await _repository.Rollback();
            throw;
        }
        finally
        {
            _repository.CloseTransaction();
        }
    }

    public async Task Delete(Samurai entity)
    {
        try
        {
            _repository.BeginTransaction();
            await _repository.Delete(entity);
            await _repository.Commit();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await _repository.Rollback();
            throw;
        }
        finally
        {
            _repository.CloseTransaction();
        }
    }
}
```

#### Presentation Layer

`Samurai` controller class

```c#
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate.Linq;
using NHibernateDemo.Api.Dtos;
using NHibernateDemo.Entity.Models;
using NHibernateDemo.Services;

namespace NHibernateDemo.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SamuraiController : ControllerBase
    {
        private readonly ILogger<SamuraiController> _logger;
        private readonly ISamuraiService _samuraiService;

        public SamuraiController(ILogger<SamuraiController> logger, ISamuraiService samuraiService)
        {
            _logger = logger;
            _samuraiService = samuraiService;
        }

        [HttpGet]
        public async Task<IEnumerable<Samurai>> Get(int? id)
        {
            if (!id.HasValue) return await _samuraiService.GetAll().ToListAsync();

            var samurai = await _samuraiService.GetById(id.Value);
            return new List<Samurai>() { samurai };
        }


        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] SamuraiCreateInput input)
        {
            var newSamurai = new Samurai { Name = input.Name };

            foreach (var quote in input.Quotes)
                newSamurai.Quotes.Add(new Quote() { Text = quote.Text, Samurai = newSamurai });

            await _samuraiService.Create(newSamurai);

            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult<int>> Put([FromBody] SamuraiUpdateInput input)
        {
            var oldSamurai = await _samuraiService.GetById(input.Id);

            if (oldSamurai == null) return NotFound();

            oldSamurai.Name = input.Name;
            oldSamurai.Quotes.Clear();
            foreach (var quote in input.Quotes)
                oldSamurai.Quotes.Add(new Quote() { Text = quote.Text, Samurai = oldSamurai });

            await _samuraiService.Update(oldSamurai.Id, oldSamurai);

            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult<int>> Delete([FromBody] int id)
        {
            var oldSamurai = await _samuraiService.GetById(id);
            if (oldSamurai == null) return NotFound();
            await _samuraiService.Delete(oldSamurai);
            return NoContent();
        }
    }
}

```

`Dto` classes

```c#
public class SamuraiUpdateInput
{
    public int Id { get; set; }

    public string Name { get; set; }

    public IList<QuoteCreateInput> Quotes { get; set; } = new List<QuoteCreateInput>();
}
```

```c#
public class SamuraiUpdateInput
{
    public int Id { get; set; }

    public string Name { get; set; }

    public IList<QuoteCreateInput> Quotes { get; set; } = new List<QuoteCreateInput>();
}
```

```c#
public class QuoteCreateInput
{
    public string Text { get; set; }
}
```



#### Setup NHibernate

Step 1: Create a NHibernateExtensions method for IServiceCollection services

```c#
using FluentNHibernate;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NHibernateDemo.Entity.Models;

namespace NHibernateDemo.Api
{
    public static class NHibernateExtensions
    {
        public static IServiceCollection AddNHibernate(this IServiceCollection services, string connectionString)
        {
            var mapper = new ModelMapper();
            var assembly = typeof(IEntity).Assembly; // All entities implements IEntity interface, and saved in Entity Assembly
            mapper.AddMappings(assembly.ExportedTypes);

            HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            var configuration = new Configuration();
            configuration.DataBaseIntegration(c =>
            {
                c.Dialect<PostgreSQLDialect>();
                c.Driver<NpgsqlDriver>();
                c.ConnectionString = connectionString;
                c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                c.LogFormattedSql = true;
                c.LogSqlInConsole = true;
            });
            configuration.AddMapping(domainMapping);

            var persistenceModel = new PersistenceModel();
            persistenceModel.AddMappingsFromAssembly(assembly);
            persistenceModel.Configure(configuration);

            var sessionFactory = configuration.BuildSessionFactory();

            services.AddSingleton(sessionFactory);
            services.AddScoped(factory => sessionFactory.OpenSession());
            services.AddScoped(factory => sessionFactory.OpenStatelessSession());

            return services;
        }
    }
}

```



Note:  Below code will locate our entity classes and their ClassMap

```c#
var mapper = new ModelMapper();
var assembly = typeof(IEntity).Assembly; // All entities implements IEntity interface, and saved in Entity Assembly
mapper.AddMappings(assembly.ExportedTypes);
```



Step 2: Use this extension method

`Startup.cs` class

```c#
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NHibernateDemo.Data;
using NHibernateDemo.Entity.Models;
using NHibernateDemo.Services;

namespace NHibernateDemo.Api
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

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            ); ;

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NHibernateDemo.Api", Version = "v1" });
            });

            var connStr = Configuration.GetConnectionString("DefaultConnection");

            // add NHibernate configuration
            services.AddNHibernate(connStr);

            // dependencies for the project
            services.AddScoped<ISamuraiAppDbContext, SamuraiAppDbContext>();
            services.AddScoped<IGenericRepository<Samurai>, GenericRepository<Samurai>>();
            services.AddScoped<ISamuraiService, SamuraiService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NHibernateDemo.Api v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

```



Note:

1. `services.AddNHibernate(connStr)` This line will invoke the extension we created just now.

2.  Dependencies injection for the entire solution:

   ```c#
   services.AddScoped<ISamuraiAppDbContext, SamuraiAppDbContext>();
   services.AddScoped<IGenericRepository<Samurai>, GenericRepository<Samurai>>();
   services.AddScoped<ISamuraiService, SamuraiService>();
   ```


   Connection string:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=nuber-eats;Username=root;Password=root"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft": "Warning",
         "Microsoft.Hosting.Lifetime": "Information"
       }
     },
     "AllowedHosts": "*"
   }
   
   ```

   ​            

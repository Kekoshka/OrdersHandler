using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.DataAccess.Postgres.Context;
using PaymentService.DataAccess.Postgres.Models;
using PaymentService.WebApi.Common.Behaviors;
using PaymentService.WebApi.Common.Extensions;
using PaymentService.WebApi.Common.Options;
using PaymentService.WebApi.Common.CustomExceptions;
using System.Data;
using System.Reflection;

namespace PaymentService.WebApi.Common.Extensions
{
    /// <summary>
    /// Содержит методы расширения для регистрации сервисов.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Название поля в файле appsettings, соответствующее строке подключения для postgre
        /// </summary>
        static string ConfigNameConnectionStringPostgre = "PostgreSql";

        /// <summary>
        /// Регистрирует сервисы в текущей сборке
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        /// <remarks>
        /// Регистрирует все сервисы в текущей сборке, названия которых заканчиваются на "Service",
        /// а также которые имеют интерфейс с соответствующим названием и префиксом "I"
        /// </remarks>
        public static void RegisterExecutingAsseblyServices(this IServiceCollection services)
        {
            var serviceTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(st => st.IsClass && !st.IsAbstract && st.Name.EndsWith("Service"));
            foreach (var serviceType in serviceTypes)
            {
                var interfaceType = serviceType.GetInterfaces()
                    .FirstOrDefault(it => it.Name == $"I{serviceType.Name}");
                if (interfaceType is not null)
                    services.AddScoped(interfaceType, serviceType);
            }
        }

        /// <summary>
        /// Регистрирует все мапперы из текущей сборки, имена которых заканчиваются на "Mapper".
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        public static void RegisterMappers(this IServiceCollection services)
        {
            var mappers = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(st => st.IsClass && st.Name.EndsWith("Mapper"));
            foreach (var mapper in mappers)
            {
                if (mapper is not null)
                    services.AddScoped(mapper);
            }
        }

        /// <summary>
        /// Настраивает базу данных для работы с PostgreSQL 
        /// и заполняет таблицу Statuses начальными данными.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="configuration">Конфигурация приложения, содержащая строки подключения.</param>
        public static void UsePostgreSql(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(config =>
            {
                var connectionString = configuration.GetConnectionString(ConfigNameConnectionStringPostgre);
                if (connectionString is null)
                    throw new NotFoundException($"Connection string with name {ConfigNameConnectionStringPostgre} not found");
                config.UseNpgsql(connectionString);
                config.UseSeeding((context, _) =>
                {
                    var rowsCount = context.Set<Status>().Count();
                    if (rowsCount == 0)
                    {
                        List<Status> statusesSeed = new()
                        {
                            new(){ Id = 1, Name = "Complete" },
                            new(){ Id = 2, Name = "In progress" }
                        };
                        context.Set<Status>().AddRange(statusesSeed);
                        context.SaveChanges();
                    }
                });
            });
        }
        public static void UseSeeding(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(config =>
            {
                config.UseSeeding((context, _) =>
                {
                    var rowsCount = context.Set<Status>().Count();
                    if (rowsCount == 0)
                    {
                        var statuses = configuration
                            .GetSection("SeedStatusesData")
                            .Get<List<Status>>();
                        if (statuses is null)
                            throw new DirectoryNotFoundException("Section \"SeedStatusesData\" in appsetings not found");
                        context.Set<Status>().AddRange(statuses);
                        context.SaveChanges();
                    }
                });
            });
        }

        /// <summary>
        /// Выполняет привязку параметров конфигурации к классам опций.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ContentTypesOptions>(configuration.GetSection(nameof(ContentTypesOptions)));
            services.Configure<KafkaOptions>(configuration.GetSection(nameof(KafkaOptions)));
            services.Configure<StatusesOptions>(options =>
            {
                var statuses = configuration.GetSection("SeedStatusesData").Get<List<Status>>();
                if (statuses is null)
                    throw new DirectoryNotFoundException("Section \"SeedStatusesData\" in appsetings not found");
                options.CompletedId = statuses.First(s => s.Name == "Completed").Id;
                options.InProgressId = statuses.First(s => s.Name == "In progress").Id;

            });
        }

        /// <summary>
        /// Регистрирует MediatR с автоматическим обнаружением обработчиков из сборки текущего приложения.
        /// Добавляет в пайплайн поведение валидации <see cref="ValidationBehavior{TRequest, TResponse}"/>.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        public static void AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });
        }
    }
}

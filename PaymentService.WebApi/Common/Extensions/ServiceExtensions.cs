using ExceptionHandler.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentService.DataAccess.Postgres.Context;
using PaymentService.WebApi.Common.Behaviors;
using PaymentService.WebApi.Common.Extensions;
using PaymentService.WebApi.Common.Options;
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
            services.Configure<ExternalServicesOptions>(configuration.GetSection(nameof(ExternalServicesOptions)));
            services.Configure<StatusesOptions>(configuration.GetSection(nameof(StatusesOptions)));
            services.Configure<DataTypesOptions>(configuration.GetSection(nameof(DataTypesOptions)));
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

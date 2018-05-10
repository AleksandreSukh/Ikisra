using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigNet;
using ProjectAPI.DataSource;

namespace ProjectAPI
{
    public class Configuration : DbMigrationsConfiguration<SourceDbContextMssql>
    {
        protected override void Seed(SourceDbContextMssql context)
        {
            //  This method will be called after migrating to the latest version.
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Sb sb" },
            //      new Person { FullName = "Giorgi Beridze" }
            //    );
            //
        }
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }
    }
    public class Initializer : MigrateDatabaseToLatestVersion<SourceDbContextMssql, Configuration>
    {
    }
    public sealed class DependencyRepository
    {
        public const string ConnectionStringName = "ConnectionStringMSSQL";
        public const string ServiceName = "RitmaRestApi";


        private static readonly Lazy<DependencyRepository> lazy =
            new Lazy<DependencyRepository>(() =>
            {
                //TODO:Change this with aspnet something 
                var apiConfig = ConfigReader.ReadFromSettings<ApiConfig>();
                //TODO:redundant
                var defaultLogger = Debugger.IsAttached ? (ILogger)new Logger() : new Logger();
                Func<ISourceDbContext> contextProvider = () => new SourceDbContextPostgres(ConnectionStringName);
                Func<IIkisraDataRepository> reportRepositoryProvider = () => new IkisraDataRepository(contextProvider);
                return new DependencyRepository(defaultLogger, reportRepositoryProvider, contextProvider, apiConfig);
            });

        public static DependencyRepository Instance { get { return lazy.Value; } }

        private DependencyRepository(ILogger logger, Func<IIkisraDataRepository> reportRepositoryProvider, Func<ISourceDbContext> contextProvider, ApiConfig apiConfig)
        {
            Logger = logger;
            ReportRepositoryProvider = reportRepositoryProvider;
            ContextProvider = contextProvider;
            ApiConfig = apiConfig;
        }
        public Func<ISourceDbContext> ContextProvider { get; }
        public Func<IIkisraDataRepository> ReportRepositoryProvider { get; }
        public ILogger Logger { get; }
        public ApiConfig ApiConfig { get; }
    }
    //TODO:
    public interface ILogger
    {
    }

    public class Logger : ILogger
    {
    }

    public sealed class ApiConfig
    {
        public readonly string AdminPassword;
        public readonly string BaseUrl;
        public readonly string Issuer;
        public readonly string Secret;
        public readonly string TokenEndpointPath;
        public readonly bool AllowInsecureHttp;
        public readonly int AccessTokenExpireTimeMinutes;
        public readonly int EvalTopNSimiilarities;

        public ApiConfig(string baseUrl, string issuer, string secret, string tokenEndpointPath, bool allowInsecureHttp, int accessTokenExpireTimeMinutes, string adminPassword, int evalTopNSimiilarities)
        {
            BaseUrl = baseUrl;
            Issuer = issuer;
            Secret = secret;
            TokenEndpointPath = tokenEndpointPath;
            AllowInsecureHttp = allowInsecureHttp;
            AccessTokenExpireTimeMinutes = accessTokenExpireTimeMinutes;
            AdminPassword = adminPassword;
            EvalTopNSimiilarities = evalTopNSimiilarities;
        }

    }
}

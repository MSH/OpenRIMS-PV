using Autofac;
using OpenRIMS.PV.Main.API.Application.Queries.AppointmentAggregate;
using OpenRIMS.PV.Main.API.Application.Queries.EncounterAggregate;
using OpenRIMS.PV.Main.API.Application.Queries.PatientAggregate;
using OpenRIMS.PV.Main.API.Application.Queries.ReportInstanceAggregate;
using OpenRIMS.PV.Main.API.Application.Queries.WorkFlowAggregate;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Infrastructure.Repositories;
using OpenRIMS.PV.Main.Services;
using System.Reflection;

namespace OpenRIMS.PV.Main.API.Infrastructure.AutofacModules
{
    public class ApplicationModule
        : Autofac.Module
    {

        public string QueriesConnectionString { get; }

        public ApplicationModule(string queriesConnectionString)
        {
            QueriesConnectionString = queriesConnectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TypeExtensionHandler>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.Register(c => new AppointmentQueries(QueriesConnectionString))
                .As<IAppointmentQueries>()
                .InstancePerLifetimeScope();

            builder.Register(c => new EncounterQueries(QueriesConnectionString))
                .As<IEncounterQueries>()
                .InstancePerLifetimeScope();

            builder.Register(c => new PatientQueries(QueriesConnectionString))
                .As<IPatientQueries>()
                .InstancePerLifetimeScope();

            builder.Register(c => new ReportInstanceQueries(QueriesConnectionString))
                .As<IReportInstanceQueries>()
                .InstancePerLifetimeScope();

            builder.Register(c => new WorkFlowQueries(QueriesConnectionString))
                .As<IWorkFlowQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<EntityFrameworkUnitOfWork>()
                .AsImplementedInterfaces()
                .As<EntityFrameworkUnitOfWork>() // for internal factories.
                .InstancePerLifetimeScope()
                .OnActivating(u => u.Instance.Start());

            builder.RegisterGeneric(typeof(EntityFrameworkRepository<>)).As(typeof(IRepositoryInt<>));
        }
    }
}

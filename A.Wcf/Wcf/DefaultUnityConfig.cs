using EgoalTech.DB;
using A.DBExtension;
using EgoalTech.UserAccessManage;
using Microsoft.Practices.Unity;
using System;
using System.Configuration;

namespace AOPIOC.Wcf
{
    public class DefaultUnityConfig : IObjectContainerConfig
    {
        public virtual void Config(IObjectContainer container)
        {
            this.RegisterDbObjectOperator(container);
            this.RegisterDbContext(container);
            this.RegisterUserAccessManager(container);
        }

        private void RegisterDbContext(IObjectContainer container)
        {
            IDbRuleContext instance = new DbRuleContext();
            container.RegisterInstance<IDbRuleContext>(instance);
            if (!container.IsRegistered(typeof(ISqlFormatter)))
            {
                container.RegisterType<ISqlFormatter, SqlServerFormatter>(new object[0]);
            }
            InjectionProperty property = new InjectionProperty("DbObjectOperator", new ResolvedParameter<DbObjectOperator>());
            InjectionProperty property2 = new InjectionProperty("DbRuleContext", new ResolvedParameter<IDbRuleContext>());
            container.RegisterType<IDbContext, DbContext>(new object[] { property, property2 });
        }

        private void RegisterDbObjectOperator(IObjectContainer container)
        {
            string nameOfDefaultConnectionString = WcfUtils.GetNameOfDefaultConnectionString();
            if (string.IsNullOrEmpty(nameOfDefaultConnectionString))
            {
                throw new ArgumentNullException("Missing connection string setting.");
            }
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[nameOfDefaultConnectionString];
            string connectionString = settings.ConnectionString;
            container.RegisterTypeWithConstructor<DbObjectOperator>(3, new object[] { new InjectionConstructor(new object[] { connectionString }) });
        }

        private void RegisterUserAccessManager(IObjectContainer container)
        {
            bool flag = string.Equals(ConfigurationManager.AppSettings["unique_login"], "true", StringComparison.OrdinalIgnoreCase);
            int result = 0;
            if (!int.TryParse(ConfigurationManager.AppSettings["session_timeout"], out result))
            {
                result = -1;
            }
            if (result <= 0)
            {
                result = 0x493e0;
            }
            InjectionProperty property = new InjectionProperty("EnableUniqueAccess", flag);
            InjectionProperty property2 = new InjectionProperty("Timeout", result);
            if (!container.IsRegistered(typeof(IPasswordEncryptor)))
            {
                container.RegisterType<IPasswordEncryptor, SimplePasswordEncryptor>(new object[0]);
            }
            InjectionConstructor constructor = new InjectionConstructor(new object[] { new ResolvedParameter<DbObjectOperator>(), new ResolvedParameter<IPasswordEncryptor>() });
            container.RegisterType<UserAccessManager, EGUserAccessManager>(new object[] { constructor, property, property2 });
        }
    }
}


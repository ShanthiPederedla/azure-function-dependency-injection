using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Extensions.Bindings;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InjectConfigurationTriggerAttribute : Attribute
    {
        
    }

    public class InjectConfigurationTriggerBindingProvider : ITriggerBindingProvider
    {
        private readonly InjectConfiguration _configuration;

        public InjectConfigurationTriggerBindingProvider(InjectConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var parameter = context.Parameter;
            var attribute =
                parameter.GetCustomAttribute<InjectConfigurationTriggerAttribute>(false);
            
            if (attribute == null)
                return Task.FromResult<ITriggerBinding>(null);
            
            if (!IsSupportBindingType(parameter.ParameterType))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Can't bind InjectConfigurationTriggerAttribute to type '{0}'.", parameter.ParameterType));
            return
                Task.FromResult<ITriggerBinding>(new InjectConfigurationTriggerBinding(context.Parameter, _configuration));
        }
        
        public bool IsSupportBindingType(Type t)
        {
            return t == typeof(IServiceCollection);
        }
    }

    public class InjectConfigurationTriggerBinding : ITriggerBinding
    {
        private readonly InjectConfiguration _configuration;
        private readonly Dictionary<string, Type> _bindingContract;
        private readonly ParameterInfo _parameter;

        public InjectConfigurationTriggerBinding(ParameterInfo parameter, InjectConfiguration configuration)
        {
            _parameter = parameter;
            _configuration = configuration;
            _bindingContract = new Dictionary<string, Type>(StringComparer.CurrentCultureIgnoreCase)
            {
                {"data", typeof(IServiceCollection)}
            };
        }
        
        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            if (value is IServiceCollection)
            {
                IValueBinder binder =
                    new InjectConfigurationTriggerValueBinder(value as IServiceCollection, _parameter.ParameterType);

                return Task.FromResult((ITriggerData) new TriggerData(binder,
                    new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase)
                    {
                        {"data", value}
                    }));
            }
            throw new NotSupportedException();
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            _configuration.AddConfigExecutor(context.Descriptor.ShortName, context.Executor);
            return Task.FromResult((IListener) new InjectConfigurationTriggerListener());
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new InjectConfigurationTriggerParameterDescriptor();
        }

        public Type TriggerValueType => typeof(IServiceCollection);

        public IReadOnlyDictionary<string, Type> BindingDataContract => _bindingContract;
        
        private class InjectConfigurationTriggerValueBinder : ValueBinder, IDisposable
        {
            private readonly IServiceCollection _serviceCollection;
            
            public InjectConfigurationTriggerValueBinder(IServiceCollection serviceCollection, Type type, BindStepOrder bindStepOrder = BindStepOrder.Default) : base(type, bindStepOrder)
            {
                _serviceCollection = serviceCollection;
            }

            public void Dispose()
            {

            }

            public override Task<object> GetValueAsync()
            {
                return Task.FromResult((object)_serviceCollection);
            }

            public override string ToInvokeString()
            {
                // TODO:Executor
                return "nono";
            }

            
        }
        
        private class InjectConfigurationTriggerParameterDescriptor : TriggerParameterDescriptor
        {
            public override string GetTriggerReason(IDictionary<string, string> arguments)
            {
                // TODO: Customize your Dashboard display string
                return string.Format("InjectConfigurationTrigger trigger fired at {0}", DateTime.Now.ToString("o"));
            }
        }
    }
    
    public class InjectConfigurationTriggerListener : IListener
    {
        public void Dispose()
        {
            
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Cancel()
        {
            
        }
    }
}
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection
{
    public static class GreeterFunction
    {
        [FunctionName("GreeterSingleton1")]
        public static async Task<HttpResponseMessage> Run1(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequestMessage req,
            [Inject("SingletonConfig")]IGreeter greeter)
        {
            return req.CreateResponse(greeter.Greet());
        }

        [FunctionName("GreeterSingleton2")]
        public static async Task<HttpResponseMessage> Run2(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequestMessage req,
           [Inject("SingletonConfig")]IGreeter greeter)
        {
            return req.CreateResponse(greeter.Greet());
        }

        [FunctionName("GreeterScope1")]
        public static async Task<HttpResponseMessage> Run3(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequestMessage req,
            [Inject("ScopeConfig")]IGreeter greeter)
        {
            return req.CreateResponse(greeter.Greet());
        }

        [FunctionName("GreeterScope2")]
        public static async Task<HttpResponseMessage> Run4(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequestMessage req,
           [Inject("ScopeConfig")]IGreeter greeter)
        {
            return req.CreateResponse(greeter.Greet());
        }

        [FunctionName("SingletonConfig")]
        public static void Config1(
            [InjectConfigurationTrigger] IServiceCollection serviceCollection
            )
        {
            serviceCollection.AddSingleton<IGreeter, CountUpGreeter>();
        }

        [FunctionName("ScopeConfig")]
        public static void Config2(
            [InjectConfigurationTrigger] IServiceCollection serviceCollection
            )
        {
            serviceCollection.AddScoped<IGreeter, CountUpGreeter>();
        }
    }
}

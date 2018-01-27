using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection
{
    public static class GreeterFunction
    {
        [FunctionName("Greeter")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequestMessage req,
            [Inject("FunctionConfig")]IGreeter greeter)
        {
            return req.CreateResponse(greeter.Greet());
        }

        [FunctionName("FunctionConfig")]
        public static void Config(
            [InjectConfigurationTrigger] IServiceCollection serviceCollection
            )
        {
            serviceCollection.AddSingleton<IGreeter, Greeter>();
        }
    }
}

using Orleans;
using OrleansGettingStarted.GrainInterfaces;
using System.Threading.Tasks;

namespace OrleansGettingStarted.Grains
{
    public class HelloWorld : Grain, IHelloWorld
    {
        public Task<string> SayHello(string name)
        {
            return Task.FromResult($"Hello World! Orleans is neato torpedo, eh {name}?");
        }
    }
}
using System.Threading.Tasks;
using Orleans;

namespace OrleansGettingStarted.GrainInterfaces
{
    public interface IHelloWorld : IGrainWithGuidKey
    {
         Task<string> SayHello(string name);
    }
}
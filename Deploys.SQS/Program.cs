using System;
using System.Threading.Tasks;
using Deploys.SQS.Configuration;

namespace Deploys.SQS
{
    class Program
    {
        static int Main(string[] args)
        {
            return new Startup(args).Execute();
        }
    }
}
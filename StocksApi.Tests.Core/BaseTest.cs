using Microsoft.Extensions.Logging;

using NSubstitute;

namespace StocksApi.Service.Tests
{
    public abstract class BaseTest<T>
    {
        protected ILogger<T> Logger { get; set; }

        public virtual void Setup()
        {
            Logger = Substitute.For<ILogger<T>>();
        }
    }
}

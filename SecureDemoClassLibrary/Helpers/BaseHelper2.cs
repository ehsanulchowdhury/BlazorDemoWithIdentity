using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureDemoClassLibrary.Helpers
{
    public class BaseHelper2 : IServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private static BaseHelper2 _instance;

        public BaseHelper2(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _instance = this;
        }

        public static BaseHelper2 Instance
        {
            get => _instance ?? throw new ApplicationException("Not Initialized");
            private set { }
        }

        public object? GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}

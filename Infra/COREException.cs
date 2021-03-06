using Microsoft.Extensions.Hosting;
using System;

namespace RithV.Services.CORE.API.Infra
{
    public class COREDomainException : Exception
    {
        public COREDomainException()
        { }

        public COREDomainException(string message)
            : base(message)
        { }

        public COREDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}

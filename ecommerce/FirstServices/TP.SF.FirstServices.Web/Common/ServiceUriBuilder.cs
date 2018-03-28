using System;
using System.Fabric;

namespace TP.SF.FirstServices.Web.Common
{
    public class ServiceUriBuilder
    {
        public string ApplicationInstance { get; set; }
        public string ServiceInstance { get; set; }
        public ICodePackageActivationContext ActivationContext { get; set; }


        public ServiceUriBuilder(string serviceInstance)
        {
            this.ActivationContext = FabricRuntime.GetActivationContext();
            this.ServiceInstance = serviceInstance;
        }

        public ServiceUriBuilder(ICodePackageActivationContext context, string serviceInstance)
        {
            this.ActivationContext = context;
            this.ServiceInstance = serviceInstance;
        }

        public ServiceUriBuilder(ICodePackageActivationContext context, string applicationInstance, string serviceInstance)
        {
            this.ActivationContext = context;
            this.ApplicationInstance = applicationInstance;
            this.ServiceInstance = serviceInstance;
        }

        public Uri ToUri()
        {
            string applicationInstance = this.ApplicationInstance;

            if (String.IsNullOrEmpty(applicationInstance))
            {
                // the ApplicationName property here automatically prepends "fabric:/" for us
                applicationInstance = this.ActivationContext.ApplicationName.Replace("fabric:/", String.Empty);
            }

            return new Uri("fabric:/" + applicationInstance + "/" + this.ServiceInstance);
        }
    }
}

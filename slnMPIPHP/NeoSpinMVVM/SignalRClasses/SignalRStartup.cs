using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Compilation;
using System.Linq;
[assembly: OwinStartup(typeof(NeoSpinWebClient.SignalRStartup))]
namespace NeoSpinWebClient
{
    public class SignalRStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            //GlobalHost.DependencyResolver.Register(typeof(Microsoft.AspNet.SignalR.Hubs.IAssemblyLocator), () => new AssemblyLocator());
        }
    }

    public class AssemblyLocator : IAssemblyLocator
    {
        public IList<Assembly> GetAssemblies()
        {
            List<Assembly> allAsms = new List<Assembly>();
            ICollection assemblies = BuildManager.GetReferencedAssemblies();
            foreach (Assembly asm in assemblies)
            {
                if (asm.FullName.ToLower().Contains("sagiteccommon"))
                    allAsms.Add(asm);
            }
            return allAsms;
        }
    }
}
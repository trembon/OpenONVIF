using OpenONVIF.Attributes;
using OpenONVIF.Security.Soap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace OpenONVIF.Services
{
    internal abstract class BaseService : IService
    {
        public Camera Camera { get; }

        internal BaseService(Camera camera)
        {
            this.Camera = camera;
        }

        static BaseService()
        {
            ServicePointManager.Expect100Continue = false;
        }

        protected TValue CallCamera<TValue>(Func<TValue> task, [CallerMemberName]string callerMethod = null)
        {
            Type currentType = this.GetType();
            MethodInfo callingMethod = currentType.GetInterfaces().Select(t => t.GetMethod(callerMethod)).FirstOrDefault(m => m != null);
            if (currentType == null)
                callingMethod = currentType.GetMethod(callerMethod);

            var callingMethodAttributes = callingMethod.GetCustomAttributes();

            if (callingMethodAttributes.Any(attr => attr is AuthenticationNeededAttribute) && Camera.PasswordDigestBehavior == null)
                throw new System.Exception("blah");
            
            return task();
        }
    }
}

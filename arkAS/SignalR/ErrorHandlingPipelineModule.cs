using System.Diagnostics;
using Microsoft.AspNet.SignalR.Hubs;

namespace arkAS.SignalR
{
    public class ErrorHandlingPipelineModule : HubPipelineModule
    {
        protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
        {
            RDL.Debug.LogError(exceptionContext.Error);
            if (exceptionContext.Error.InnerException != null)
            {
                RDL.Debug.LogError(exceptionContext.Error.InnerException);                
            }
            base.OnIncomingError(exceptionContext, invokerContext);
        }
    }
}
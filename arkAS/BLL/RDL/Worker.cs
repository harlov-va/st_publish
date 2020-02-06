using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Security.Principal;
using System.Threading;


namespace RDL
{
    public abstract class Worker
    {
        public delegate void WorkerMethod();

        public static void StartShadowTask( WorkerMethod method){
            Thread newThread = new Thread(new ThreadStart(method));
            newThread.Start();
        }         
    } 
}
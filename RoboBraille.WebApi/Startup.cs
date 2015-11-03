﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Owin;
using RoboBraille.WebApi.Models;

namespace RoboBraille.WebApi
{
    public class Startup
    {
        public static void Configuration(IAppBuilder builder)
        {
            var config = new HttpConfiguration();
            config.Formatters.Add(new MultipartFormDataFormatter());
            config.MapHttpAttributeRoutes();
            

            
            //enable HMAC authentication 
            //HawkAuthenticator.EnableHawkAuthentication(config);

            builder.UseWebApi(config);
        }
    }
}
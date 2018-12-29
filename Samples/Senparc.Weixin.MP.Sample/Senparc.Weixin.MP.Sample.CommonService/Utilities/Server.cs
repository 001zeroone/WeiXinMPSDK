﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#if NET45
using System.Web;
#else
using Microsoft.AspNetCore.Http;
using Senparc.CO2NET.Utilities;
#endif

namespace Senparc.Weixin.MP.Sample.CommonService.Utilities
{
    public static class Server
    {
        public static string GetMapPath(string virtualPath)
        {
            //if (virtualPath == null)
            //{
            //    return "";
            //}
            //else if (virtualPath.StartsWith("~/"))
            //{
            //    return virtualPath.Replace("~/", AppDomainAppPath);
            //}
            //else
            //{
            //    return Path.Combine(AppDomainAppPath, virtualPath);
            //}
            return ServerUtility.ContentRootMapPath(virtualPath);
        }

        public static HttpContext HttpContext
        {
            get
            {
#if NET45
                HttpContext context = HttpContext.Current;
                if (context == null)
                {
                    HttpRequest request = new HttpRequest("Default.aspx", "http://sdk.weixin.senparc.com/default.aspx", null);
                    StringWriter sw = new StringWriter();
                    HttpResponse response = new HttpResponse(sw);
                    context = new HttpContext(request, response);
                }
#else
                HttpContext context = new DefaultHttpContext();
#endif
                return context;
            }
        }
    }
}

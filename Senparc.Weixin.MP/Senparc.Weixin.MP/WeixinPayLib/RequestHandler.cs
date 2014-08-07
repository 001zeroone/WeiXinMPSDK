using System;
using System.Collections;
using System.Text;
using System.Web;
using System.Xml;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace Senparc.Weixin.MP.WeixinPayLib
{
    /**
    'ǩ��������
     ============================================================================/// <summary>
    'api˵����
    'init();
    '��ʼ��������Ĭ�ϸ�һЩ������ֵ��
    'setKey(key_)'�����̻���Կ
    'createMd5Sign(signParams);�ֵ�����Md5ǩ��
    'genPackage(packageParams);��ȡpackage��
    'createSHA1Sign(signParams);����ǩ��SHA1
    'parseXML();���xml
    'getDebugInfo(),��ȡdebug��Ϣ
     * 
     * ============================================================================
     */
    public class RequestHandler
	{
    
        public RequestHandler(HttpContext httpContext)
        {
            parameters = new Hashtable();

            this.httpContext = httpContext;
           
        }
        /// <summary>
        /// ��Կ
        /// </summary>
        private string key;

        protected HttpContext httpContext;

		/// <summary>
        /// ����Ĳ���
		/// </summary>
		protected Hashtable parameters;
		
		/// <summary>
        /// debug��Ϣ
		/// </summary>
		private string debugInfo;
		
		/// <summary>
        /// ��ʼ������
		/// </summary>
		public virtual void Init() 
		{
		}
        /// <summary>
        /// ��ȡdebug��Ϣ
        /// </summary>
        /// <returns></returns>
		public String GetDebugInfo() 
		{
			return debugInfo;
		}
		/// <summary>
        /// ��ȡ��Կ
		/// </summary>
		/// <returns></returns>
		public string GetKey() 
		{
			return key;
		}
        /// <summary>
        /// ������Կ
        /// </summary>
        /// <param name="key"></param>
		public void SetKey(string key) 
		{
			this.key = key;
		}
        
        /// <summary>
        /// ���ò���ֵ
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterValue"></param>
        public void SetParameter(string parameter, string parameterValue)
        {
            if (parameter != null && parameter != "")
            {
                if (parameters.Contains(parameter))
                {
                    parameters.Remove(parameter);
                }

                parameters.Add(parameter, parameterValue);
            }
        }


        /// <summary>
        /// ��ȡpackage��������ǩ����
        /// </summary>
        /// <returns></returns>
        public string GetRequestURL()
        {
            this.CreateSign();
            StringBuilder sb = new StringBuilder();
            ArrayList akeys=new ArrayList(parameters.Keys); 
            akeys.Sort();
            foreach(string k in akeys)
            {
                string v = (string)parameters[k];
                if(null != v && "key".CompareTo(k) != 0) 
                {
                    sb.Append(k + "=" + WeixinPayUtil.UrlEncode(v, getCharset()) + "&");
                }
            }

            //ȥ�����һ��&
            if(sb.Length > 0)
            {
                sb.Remove(sb.Length-1, 1);
            }

         
           return sb.ToString();
           
        }
       
		/// <summary>
        /// ����md5ժҪ,������:����������a-z����,������ֵ�Ĳ������μ�ǩ��
		/// </summary>
        protected virtual void  CreateSign() 
        {
            StringBuilder sb = new StringBuilder();

            ArrayList akeys=new ArrayList(parameters.Keys); 
            akeys.Sort();

            foreach(string k in akeys)
            {
                string v = (string)parameters[k];
                if(null != v && "".CompareTo(v) != 0
                    && "sign".CompareTo(k) != 0 && "key".CompareTo(k) != 0) 
                {
                    sb.Append(k + "=" + v + "&");
                }
            }

            sb.Append("key=" + this.GetKey());
            string sign = MD5Util.GetMD5(sb.ToString(), getCharset()).ToUpper();

            this.SetParameter("sign", sign);
		
            //debug��Ϣ
            this.SetDebugInfo(sb.ToString() + " => sign:" + sign);		
        }
     

       /// <summary>
        /// ����packageǩ��
       /// </summary>
       /// <returns></returns>
        public virtual string CreateMd5Sign()
        {
            StringBuilder sb = new StringBuilder();
            ArrayList akeys=new ArrayList(parameters.Keys); 
            akeys.Sort();

            foreach(string k in akeys)
            {
                string v = (string)parameters[k];
                if(null != v && "".CompareTo(v) != 0
                    && "sign".CompareTo(k) != 0 && "".CompareTo(v) != 0) 
                {
                    sb.Append(k + "=" + v + "&");
                }
            }
            string sign = MD5Util.GetMD5(sb.ToString(), getCharset()).ToLower();

            this.SetParameter("sign", sign);
            return sign;
    }


        /// <summary>
        /// ����sha1ǩ��
        /// </summary>
        /// <returns></returns>
        public string CreateSHA1Sign()
        {
            StringBuilder sb = new StringBuilder();
            ArrayList akeys = new ArrayList(parameters.Keys);
            akeys.Sort();

            foreach (string k in akeys)
            {
                string v = (string)parameters[k];
              if (null != v && "".CompareTo(v) != 0
                     && "sign".CompareTo(k) != 0 && "key".CompareTo(k) != 0)
                {
                    if(sb.Length==0)
                    {
                    sb.Append(k + "=" + v);
                    }
                    else{
                     sb.Append("&" + k + "=" + v);
                    }
                }
            }
            string paySign = SHA1Util.GetSha1(sb.ToString()).ToString().ToLower();
       
			//debug��Ϣ
            this.SetDebugInfo(sb.ToString() + " => sign:" + paySign);
            return paySign;
        }


         /// <summary>
        /// ���XML
         /// </summary>
         /// <returns></returns>
        public string ParseXML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<xml>");
            foreach (string k in parameters.Keys)
            {
                string v = (string)parameters[k];
                if (Regex.IsMatch(v, @"^[0-9.]$"))
                {

                    sb.Append("<" + k + ">" + v + "</" + k + ">");
                }
                else
                {
                    sb.Append("<" + k + "><![CDATA[" + v + "]]></" + k + ">");
                }

           }
            sb.Append("</xml>");
            return sb.ToString();
        }

       

        /// <summary>
        /// ����debug��Ϣ
        /// </summary>
        /// <param name="debugInfo"></param>
		public void SetDebugInfo(String debugInfo) 
		{
			this.debugInfo = debugInfo;
		}

		public Hashtable getAllParameters()
		{
			return this.parameters;
		}

         protected virtual string getCharset()
      {
          return this.httpContext.Request.ContentEncoding.BodyName;
      } 
    }
}

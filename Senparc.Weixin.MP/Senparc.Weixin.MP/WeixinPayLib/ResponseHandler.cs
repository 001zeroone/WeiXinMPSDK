using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Xml;

namespace Senparc.Weixin.MP.WeixinPayLib
{

    /** 
    '============================================================================
    'api˵����
    'getKey()/setKey(),��ȡ/������Կ
    'getParameter()/setParameter(),��ȡ/���ò���ֵ
    'getAllParameters(),��ȡ���в���
    'isTenpaySign(),�Ƿ���ȷ��ǩ��,true:�� false:��
    'isWXsign(),�Ƿ���ȷ��ǩ��,true:�� false:��
    ' * isWXsignfeedback�ж�΢��άȨǩ��
    ' *getDebugInfo(),��ȡdebug��Ϣ
    '============================================================================
    */

    public class ResponseHandler
	{
		/// <summary>
        /// ��Կ 
		/// </summary>
		private string key;

        /// <summary>
        /// appkey
        /// </summary>
        private string appkey;

        /// <summary>
        /// xmlMap
        /// </summary>
        private Hashtable xmlMap;

		/// <summary>
        /// Ӧ��Ĳ���
		/// </summary>
		protected Hashtable parameters;
		
		/// <summary>
        /// debug��Ϣ
		/// </summary>
		private string debugInfo;
        /// <summary>
        /// ԭʼ����
        /// </summary>
        protected string content;

        private string charset = "gb2312";

        /// <summary>
        /// ����ǩ���Ĳ����б�
        /// </summary>
        private static string SignField = "appid,appkey,timestamp,openid,noncestr,issubscribe";

		protected HttpContext httpContext;

        /// <summary>
        /// ��ʼ������
        /// </summary>
        public virtual void Init()
        {
        }

        /// <summary>
        /// ��ȡҳ���ύ��get��post����
        /// </summary>
        /// <param name="httpContext"></param>
        public ResponseHandler(HttpContext httpContext)
        {
            parameters = new Hashtable();
            xmlMap = new Hashtable();

            this.httpContext = httpContext;
            NameValueCollection collection;
            //post data
            if (this.httpContext.Request.HttpMethod == "POST")
            {
                collection = this.httpContext.Request.Form;
                foreach (string k in collection)
                {
                    string v = (string)collection[k];
                    this.SetParameter(k, v);
                }
            }
            //query string
            collection = this.httpContext.Request.QueryString;
            foreach (string k in collection)
            {
                string v = (string)collection[k];
                this.SetParameter(k, v);
            }
            if (this.httpContext.Request.InputStream.Length > 0)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(this.httpContext.Request.InputStream);
                XmlNode root = xmlDoc.SelectSingleNode("xml");
                XmlNodeList xnl = root.ChildNodes;

                foreach (XmlNode xnf in xnl)
                {
                    xmlMap.Add(xnf.Name, xnf.InnerText);
                }
            }
        }
    

		/// <summary>
        /// ��ȡ��Կ
		/// </summary>
		/// <returns></returns>
		public string GetKey() 
		{ return key;}

		/// <summary>
        /// ������Կ
		/// </summary>
		/// <param name="key"></param>
		/// <param name="appkey"></param>
		public void SetKey(string key, string appkey) 
		{
            this.key = key;
            this.appkey = appkey;
        }

		/// <summary>
        /// ��ȡ����ֵ
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public string GetParameter(string parameter) 
		{
			string s = (string)parameters[parameter];
			return (null == s) ? "" : s;
		}

		/// <summary>
        /// ���ò���ֵ
		/// </summary>
		/// <param name="parameter"></param>
		/// <param name="parameterValue"></param>
		public void SetParameter(string parameter,string parameterValue) 
		{
			if(parameter != null && parameter != "")
			{
				if(parameters.Contains(parameter))
				{
					parameters.Remove(parameter);
				}
	
				parameters.Add(parameter,parameterValue);		
			}
		}

		/// <summary>
		/// �Ƿ�Ƹ�ͨǩ��,������:����������a-z����,������ֵ�Ĳ������μ�ǩ����return boolean
		/// </summary>
		/// <returns></returns>
        public virtual Boolean IsTenpaySign() 
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
            string sign = MD5Util.GetMD5(sb.ToString(), GetCharset()).ToLower();
            this.SetDebugInfo(sb.ToString() + " => sign:" + sign);
			//debug��Ϣ
			return GetParameter("sign").ToLower().Equals(sign); 
		}

        /// <summary>
        /// �ж�΢��ǩ��
        /// </summary>
        /// <returns></returns>
        public virtual Boolean IsWXsign()
        {
            StringBuilder sb = new StringBuilder();
            Hashtable signMap = new Hashtable();

            foreach (string k in xmlMap.Keys)
            {
                if (k != "SignMethod" && k != "AppSignature")
                {
                    signMap.Add(k.ToLower(), xmlMap[k]);
                }
            }
            signMap.Add("appkey", this.appkey);


            ArrayList akeys = new ArrayList(signMap.Keys);
            akeys.Sort();

            foreach (string k in akeys)
            {
                string v = (string)signMap[k];
                if (sb.Length == 0)
                {
                    sb.Append(k + "=" + v);
                }
                else
                {
                    sb.Append("&" + k + "=" + v);
                }
            }

            string sign = SHA1Util.GetSha1(sb.ToString()).ToString().ToLower();

            this.SetDebugInfo(sb.ToString() + " => SHA1 sign:" + sign);

            return sign.Equals(xmlMap["AppSignature"]);

        }

        /// <summary>
        /// �ж�΢��άȨǩ��
        /// </summary>
        /// <returns></returns>
        public virtual Boolean IsWXsignfeedback()
        {
            StringBuilder sb = new StringBuilder();
            Hashtable signMap = new Hashtable();
       
            foreach (string k in xmlMap.Keys)
            {
                if (SignField.IndexOf(k.ToLower()) != -1)
                {
                    signMap.Add(k.ToLower(), xmlMap[k]);
                }
            }
            signMap.Add("appkey", this.appkey);
          

            ArrayList akeys = new ArrayList(signMap.Keys);
            akeys.Sort();

            foreach (string k in akeys)
            {
                string v = (string)signMap[k];
                if ( sb.Length == 0 )
                {
                    sb.Append(k + "=" + v);
                }
                else
                {
                    sb.Append("&" + k + "=" + v);
                }
            }
            
            string sign = SHA1Util.GetSha1(sb.ToString()).ToString().ToLower();
            
            this.SetDebugInfo(sb.ToString() + " => SHA1 sign:" + sign);

            return sign.Equals( xmlMap["AppSignature"] );

        }
   
		/// <summary>
        /// ��ȡdebug��Ϣ
		/// </summary>
		/// <returns></returns>
		public string GetDebugInfo() 
		{ return debugInfo;}
				
		/// <summary>
        /// ����debug��Ϣ
		/// </summary>
		/// <param name="debugInfo"></param>
		protected void SetDebugInfo(String debugInfo)
		{ this.debugInfo = debugInfo;}

		protected virtual string GetCharset()
		{
			return this.httpContext.Request.ContentEncoding.BodyName;
			
		}

		
	}
}

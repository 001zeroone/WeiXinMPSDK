/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc
    
    �ļ�����MD5UtilHelper.cs
    �ļ�������������ȡ��д��MD5ǩ�����
    
    
    ������ʶ��Senparc - 20150211
    
    �޸ı�ʶ��Senparc - 20150303
    �޸�����������ӿ�
    
    �޸ı�ʶ��Senparc - 20161015
    �޸��������޸�GB2312����Ϊ936

    �޸ı�ʶ��Senparc - 20170203
    �޸�������v14.3.123  �ϳ�MD5UtilHelper�������ϲ���
       Senparc.Weixin.Helpers.EncryptHelper�£�Senparc.Weixin.dll�У�

----------------------------------------------------------------*/

using System;
using System.Security.Cryptography;
using System.Text;

namespace Senparc.Weixin.MP.Helpers
{
	/// <summary>
    /// MD5UtilHelper ��ժҪ˵����
	/// </summary>
	public class MD5UtilHelper
	{
        /// <summary>
        /// ��ȡ��д��MD5ǩ�����
		/// </summary>
		/// <param name="encypStr"></param>
		/// <param name="charset"></param>
		/// <returns></returns>
		public static string GetMD5(string encypStr, string charset)
		{
			string retStr;
			var m5 = MD5.Create();

			//����md5����
			byte[] inputBye;
			byte[] outputBye;

			//ʹ��GB2312���뷽ʽ���ַ���ת��Ϊ�ֽ����飮
			try
			{
				inputBye = Encoding.GetEncoding(charset).GetBytes(encypStr);
			}
			catch (Exception ex)
			{
                //inputBye = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
                inputBye = Encoding.GetEncoding(936).GetBytes(encypStr);
            }
            outputBye = m5.ComputeHash(inputBye);

			retStr = BitConverter.ToString(outputBye);
			retStr = retStr.Replace("-", "").ToUpper();
			return retStr;
		}
	}
}

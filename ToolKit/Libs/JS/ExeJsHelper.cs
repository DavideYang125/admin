using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;
namespace DotNet.Utilities.JS
{
    /// <summary>
    /// ����ִ��JS����
    /// </summary>
    public class ExeJsHelper
    {

        public object GetMainResult(string js, string mainname)
        {
            CodeDomProvider _provider = new Microsoft.JScript.JScriptCodeProvider();
            Type _evaluateType;
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            CompilerResults results = _provider.CompileAssemblyFromSource(parameters, js);
            Assembly assembly = results.CompiledAssembly;
            string time = "";

            _evaluateType = assembly.GetType("aa.JScript");
            object[] w = new object[] { "123", time };

            object ww = _evaluateType.InvokeMember("getm32str", BindingFlags.InvokeMethod,
            null, null, w);
            return js;
        }
        ///// <summary>
        ///// �������
        ///// </summary>
        ///// <param name="pass"></param>
        ///// <returns></returns>
        //public string EncodePass(string pass)
        //{
        //    ScriptControlClass sc = new ScriptControlClass();
        //    sc.UseSafeSubset = true;
        //    sc.Language = "JScript";
        //    sc.AddCode(Properties.Resources.QQRsa);  //����Դ�ж�ȡjs����,Ҳ����д��Js�ļ������.
        //    string str = sc.Run("rsaEncrypt", new object[] { pass }).ToString();
        //    return str;
        //}
    }
}

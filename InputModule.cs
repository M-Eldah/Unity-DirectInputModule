using System;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class InputModule : MonoBehaviour
{
    public TMP_InputField inputField;
    public string ObjectName;
    public string TargetName;
    public enum TargetType
    {
        Field,
        Property,
        Method
    }
    public TargetType targetType;
    public string InputRegex;

    
    // Update is called once per frame
    public void InsertValue()
    {
        GameObject obj = GameObject.Find(ObjectName);
        Regex regex= new Regex(InputRegex);
        Debug.Log(regex.IsMatch(inputField.text));
        if(regex.IsMatch(inputField.text)||InputRegex==string.Empty)
        {
            switch (targetType)
            {
                case TargetType.Field:
                    var FInfo = GetField(obj, TargetName);
                    FieldInfo f = FInfo.Item2;
                    var value = Convert.ChangeType(inputField.text, f.FieldType);
                    f.SetValue(FInfo.Item1, value);
                    break;

                case TargetType.Property:
                    var PInfo = GetProperty(obj, TargetName);
                    PropertyInfo p = PInfo.Item2;
                    var Pvalue = Convert.ChangeType(inputField.text, p.PropertyType);
                    p.SetValue(PInfo.Item1, Pvalue);
                    break;

                case TargetType.Method:
                    var Info = GetMethod(obj, TargetName);
                    MethodInfo m = Info.Item2 as MethodInfo;
                    MonoBehaviour mono = Info.Item1 as MonoBehaviour;
                    ParameterInfo[] ps = m.GetParameters();
                    var paramter = Convert.ChangeType(inputField.text, ps[0].ParameterType);
                    m.Invoke(mono, new object[] { paramter });

                    break;
            }
        }else
        {
            Debug.LogError("Regex Failed");
        }
       
    }
    private (MonoBehaviour, FieldInfo) GetField(GameObject obj, string TargetName)
    {
        var mbs = obj.GetComponents<MonoBehaviour>();
        var publicFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
        foreach (MonoBehaviour mb in mbs)
        {
            foreach (var m in mb.GetType().GetFields(publicFlags))
            {
                if (m.Name == TargetName)
                {
                    return (mb, m);
                }
            }
        }
        return (null, null);
    }
    private (MonoBehaviour, PropertyInfo) GetProperty(GameObject obj, string TargetName)
    {
        var mbs = obj.GetComponents<MonoBehaviour>();
        var publicFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
        foreach (MonoBehaviour mb in mbs)
        {
            foreach (var m in mb.GetType().GetProperties(publicFlags))
            {
                if (m.Name == TargetName)
                {
                    return (mb, m);
                }
            }
        }
        return (null, null);
    }
    private (MonoBehaviour,MethodInfo) GetMethod(GameObject obj, string TargetName) 
    {
        var mbs = obj.GetComponents<MonoBehaviour>();
        var publicFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
        foreach (MonoBehaviour mb in mbs)
        {
            foreach (var m in mb.GetType().GetMethods(publicFlags))
            {
                if (m.Name == TargetName)
                {
                    return (mb,m);
                }
            }
        }
        return (null,null);
    }
}

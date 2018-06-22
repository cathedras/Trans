using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace myzy.Util
{
    public abstract class IntrinsicCfg
    {
        protected  string _section = "CFG";
        /// <summary>
        /// INI 配置文档
        /// </summary>
        private  IniFiles _ini = null;
        /// <summary>
        /// 反射加载配置信息
        /// </summary>
        public IntrinsicCfg LoadGbl<T>(string fil)
            where T : IntrinsicCfg, new()
        {
            _ini = new IniFiles(fil);
            var typ = this.GetType();

            var pros = typ.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in pros)
            {
                try
                {
                    var attr2 = propertyInfo.GetCustomAttributes(typeof(SectionNameAttribute), true);
                    var sec = _section;
                    if (attr2.Any())
                    {
                        sec = ((SectionNameAttribute)attr2[0]).SectionName;
                    }

                    if (!_ini.ValueExists(sec, propertyInfo.Name))
                        continue;

                    Object obj = null;
                    var proTyp = propertyInfo.PropertyType;
                    if (proTyp == typeof(string))
                    {
                        obj = _ini.ReadString(sec, propertyInfo.Name, "");
                    }
                    else if (proTyp == typeof(int))
                    {
                        obj = _ini.ReadInteger(sec, propertyInfo.Name, 0);
                    }
                    else if (proTyp == typeof(ushort))
                    {
                        obj = _ini.ReadInteger(sec, propertyInfo.Name, 0);
                    }
                    else if (proTyp == typeof(double))
                    {
                        var str = _ini.ReadString(sec, propertyInfo.Name, "");
                        var val = 0d;
                        double.TryParse(str, out val);
                        obj = val;
                    }
                    else if (proTyp == typeof(float))
                    {
                        var str = _ini.ReadString(sec, propertyInfo.Name, "");
                        float val = 0f;
                        float.TryParse(str, out val);
                        obj = val;
                    }
                    else if (proTyp == typeof(bool))
                    {
                        obj = _ini.ReadBool(sec, propertyInfo.Name, false);
                    }
                    if (obj != null)
                    {
                        propertyInfo.SetValue(this, obj);
                    }
                }
                catch (Exception)
                {
                    //
                }
            }
            return this;
        }

        public void Save(string fil, Type typ)
        {
            IniFiles ini = new IniFiles(fil);

            var pros = typ.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in pros)
            {
                try
                {
                    Object obj = null;
                    var proTyp = propertyInfo.PropertyType;
                    if (proTyp == typeof(string))
                    {
                        obj = propertyInfo.GetValue(this);
                    }
                    else if (proTyp == typeof(int))
                    {
                        obj = propertyInfo.GetValue(this);
                    }
                    else if (proTyp == typeof(ushort))
                    {
                        obj = propertyInfo.GetValue(this);
                    }
                    else if (proTyp == typeof(float))
                    {
                        var obj2 = (float)propertyInfo.GetValue(this);
                        obj = obj2.ToString();
                    }
                    else if (proTyp == typeof(double))
                    {
                        var obj2 = (double)propertyInfo.GetValue(this);
                        obj = obj2.ToString();
                    }
                    else if (proTyp == typeof(bool))
                    {
                        obj = (bool)propertyInfo.GetValue(this);
                    }
                    if (obj != null)
                    {
                        var attr = propertyInfo.GetCustomAttributes(typeof(IgnoreSerializeAttribute), true);

                        var attr2 = propertyInfo.GetCustomAttributes(typeof(SectionNameAttribute), true);

                        var attr3 = propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);

                        var sec = _section;
                        if (attr2.Any())
                        {
                            sec = ((SectionNameAttribute) attr2[0]).SectionName;
                        }
                        
                        if (!attr.Any())
                        {
                            ini.WriteString(sec, propertyInfo.Name, obj.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    //
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SectionNameAttribute : Attribute
    {
        public SectionNameAttribute(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
                throw new NoNullAllowedException("SECTION NAME CAN'T BE NULL.");

            SectionName = sectionName;
        }
        public string SectionName { get; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IgnoreSerializeAttribute : Attribute
    {
        
    }
}
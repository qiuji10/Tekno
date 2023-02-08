using System;
using System.Reflection;

namespace Kurisu.AkiBT.Editor
{
    public class FloatResolver : FieldResolver<UnityEngine.UIElements.FloatField,float>
    {
        public FloatResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override UnityEngine.UIElements.FloatField CreateEditorField(FieldInfo fieldInfo)
        {
            return new UnityEngine.UIElements.FloatField(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType == typeof(float);
    }
}
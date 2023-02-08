using System;
using System.Reflection;

namespace Kurisu.AkiBT.Editor
{
    public class IntResolver : FieldResolver<UnityEngine.UIElements.IntegerField,int>
    {
        public IntResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override UnityEngine.UIElements.IntegerField CreateEditorField(FieldInfo fieldInfo)
        {
            return new UnityEngine.UIElements.IntegerField(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType == typeof(int);
    }
}
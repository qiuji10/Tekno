using System;
using System.Reflection;

using UnityEngine;

namespace Kurisu.AkiBT.Editor
{
    public class RectIntResolver : FieldResolver<UnityEngine.UIElements.RectIntField,RectInt>
    {
        public RectIntResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override UnityEngine.UIElements.RectIntField CreateEditorField(FieldInfo fieldInfo)
        {
            return new UnityEngine.UIElements.RectIntField(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType == typeof(RectInt);
    }
}
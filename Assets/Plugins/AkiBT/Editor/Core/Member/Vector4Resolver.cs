using System;
using System.Reflection;

using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class Vector4Resolver : FieldResolver<UnityEngine.UIElements.Vector4Field,Vector4>
    {
        public Vector4Resolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override UnityEngine.UIElements.Vector4Field CreateEditorField(FieldInfo fieldInfo)
        {
            return new UnityEngine.UIElements.Vector4Field(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType == typeof(Vector4);

    }
}
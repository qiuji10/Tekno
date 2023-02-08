using System;
using System.Reflection;

using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class Vector2Resolver : FieldResolver<UnityEngine.UIElements.Vector2Field, Vector2>
    {
        public Vector2Resolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override UnityEngine.UIElements.Vector2Field CreateEditorField(FieldInfo fieldInfo)
        {
            return new UnityEngine.UIElements.Vector2Field(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType == typeof(Vector2);
    }
}
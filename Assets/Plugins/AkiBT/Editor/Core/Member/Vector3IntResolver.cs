using System;
using System.Reflection;

using UnityEngine;
namespace Kurisu.AkiBT.Editor
{
    public class Vector3IntResolver : FieldResolver<UnityEngine.UIElements.Vector3IntField,Vector3Int>
    {
        public Vector3IntResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override UnityEngine.UIElements.Vector3IntField CreateEditorField(FieldInfo fieldInfo)
        {
            return new UnityEngine.UIElements.Vector3IntField(fieldInfo.Name);
        }
        public static bool IsAcceptable(Type infoType,FieldInfo info)=>infoType == typeof(Vector3Int);

    }
}
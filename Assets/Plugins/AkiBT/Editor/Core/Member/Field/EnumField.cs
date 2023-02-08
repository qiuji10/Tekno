using System;
using System.Collections.Generic;


namespace Kurisu.AkiBT.Editor
{
    public class EnumField : UnityEngine.UIElements.PopupField<Enum>
    {
        public EnumField(string label, List<Enum> choices, Enum defaultValue = null)
            : base(label, choices,  defaultValue, null, null) {
        }
    }
}
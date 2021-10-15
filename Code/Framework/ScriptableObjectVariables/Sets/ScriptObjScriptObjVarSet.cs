// Primary Author : Viktor Dahlberg - vida6631

using System.Linq;

namespace Framework.ScriptableObjectVariables.Sets
{
    /// <summary>
    ///     Override implementation for Scriptable Object Sets to allow ScriptObjVar sets to be serialized.
    /// </summary>
    /// <typeparam name="T">Any type that inherits from <c>ScriptObjVar</c></typeparam>
    public class ScriptObjScriptObjVarSet<T> : ScriptObjSet<T> where T : ScriptObjVar
    {
        public override object GetValue()
        {
            return items.Select(t => t.GetValue()).ToArray();
        }

        public override void SetValue(object value)
        {
            var itemValues = (object[]) value;
            for (var i = 0; i < items.Count; i++)
            {
                items[i].SetValue(itemValues[i]);
            }
        }

        public override string ToString()
        {
            return string.Concat(items);
        }
    }
}
// Primary Author : Viktor Dahlberg - vida6631

using System;
using System.Collections.Generic;
using Framework.ScriptableObjectVariables;

namespace Framework.SaveSystem
{
	/// <summary>
	///     Takes an array of Scriptable Object Variables, parses their values and stores them.
	/// </summary>
	[Serializable]
    public class Save
    {
        public Save(IReadOnlyList<ScriptObjVar> saveData)
        {
            SaveData = new object[saveData.Count];
            for (var i = 0; i < saveData.Count; i++)
            {
                SaveData[i] = saveData[i].GetValue();
            }
        }

        public object[] SaveData { get; private set; }
    }
}
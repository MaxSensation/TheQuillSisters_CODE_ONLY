// Primary Author : Viktor Dahlberg - vida6631

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace DebugModes
{
	[ExecuteInEditMode]
	public class DebugPassController : MonoBehaviour
	{
		private enum PassMode
		{
			NONE,
			UV_LIT,
			UV_UNLIT
		}
		
		[Header("Debug Pass Settings"), SerializeField]
		private PassMode debugPassMode = default;

		[Header("Internal"), SerializeField]
		private CustomPassVolume customPassVolume = default;

		private PassMode _old;

#if UNITY_EDITOR
		private void OnEnable()
		{
			EditorApplication.update += SelfUpdate;
		}

		private void OnDisable()
		{
			EditorApplication.update -= SelfUpdate;
		}

		private void SelfUpdate()
		{
			if (debugPassMode != _old)
			{
				SetPassMode(debugPassMode);
			}
		}
#endif

		private void SetPassMode(PassMode passMode)
		{
			_old = passMode;
			DisableAll();
			switch (passMode)
			{
				case PassMode.UV_LIT:
					customPassVolume.customPasses.Where(p => p.name.Equals("UV_Lit")).FirstOrDefault().enabled = true;
					break;
				case PassMode.UV_UNLIT:
					customPassVolume.customPasses.Where(p => p.name.Equals("UV_Unlit")).FirstOrDefault().enabled = true;
					break;
			}
		}

		private void DisableAll()
		{
			customPassVolume.customPasses.ForEach(p => p.enabled = false);
		}
	}
}
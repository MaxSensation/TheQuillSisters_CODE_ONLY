// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace UI
{
	/// <summary>
	///     Used for quitting the game through the UI. Sends exit code 0.
	/// </summary>
	public class Quitter : MonoBehaviour
    {
        public void Quit()
        {
            Application.Quit(0);
        }
    }
}
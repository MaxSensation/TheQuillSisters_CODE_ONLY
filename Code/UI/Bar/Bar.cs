// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;
using UnityEngine.UI;

namespace UI.Bar
{
    public class Bar : MonoBehaviour
    {
        [Header("Main Slider")] 
        
        [SerializeField]
        protected Slider mainSlider = default;
        
        [Header("Echo Slider")] 
        
        [SerializeField]
        protected Slider echoSlider = default;
        [SerializeField]
        private float delay = default;
        [SerializeField]
        private float speed = default;

        private float _timeUntilMove;

        private void Update()
        {
            _timeUntilMove -= Time.deltaTime;
            if (_timeUntilMove <= 0.001f)
            {
                echoSlider.value = Mathf.MoveTowards(echoSlider.value, mainSlider.value, speed * Time.deltaTime);
            }
        }

        public virtual void Refresh()
        {
        }

        protected void ResetWait()
        {
            _timeUntilMove = delay;
        }
    }
}
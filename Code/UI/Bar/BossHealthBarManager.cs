// Primary Author : Viktor Dahlberg - vida6631

using Entity.HealthSystem;
using UnityEngine;

namespace UI.Bar
{
    public class BossHealthBarManager : MonoBehaviour
    {
        [SerializeField] 
        private BossHealthBar[] bossHealthBars = default;

        public static BossHealthBarManager Instance;

        private void Awake()
        {
            Instance = Instance == null ? this : Instance;
        }

        public void Add(string name, Health healthComponent, int index)
        {
            bossHealthBars[index].SetName(name);
            bossHealthBars[index].SetHealthComponentTarget(healthComponent);
            bossHealthBars[index].gameObject.SetActive(true);
        }

        public void Refresh(int index, string name = null)
        {
            bossHealthBars[index].Refresh();
            if (name != null)
            {
                bossHealthBars[index].SetName(name);
            }
        }

        public void Remove(int index)
        {
            bossHealthBars[index].SetName(string.Empty);
            bossHealthBars[index].SetHealthComponentTarget(null);
            bossHealthBars[index].gameObject.SetActive(false);
        }
    }
}
    using UnityEngine;

    public class ConfigsRepository : MonoBehaviour
    {
        public CommandsConfig CommandsConfig { get; set; }

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }

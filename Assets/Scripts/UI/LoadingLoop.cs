namespace Incru.UI
{
    using Incru.Structure;
    using UnityEngine;
    using UnityEngine.UI;

    public class LoadingLoop : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;
        [SerializeField] private Image ownImage;

        public MyList<string> StackedLoaders = new MyList<string>();
        public static LoadingLoop Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                DontDestroyOnLoad(gameObject);
                gameObject.SetActive(false);
            }

            StackedLoaders.Changed += OnChanged;
        }

        private void OnDestroy() => StackedLoaders.Changed -= OnChanged;

        private void OnChanged(MyList<string> list) => gameObject.SetActive(list.Count > 0);

        private void Update()
        {
            ownImage.fillAmount += ownImage.fillClockwise ? Time.deltaTime * speed : -Time.deltaTime * speed;
            ownImage.fillClockwise = ownImage.fillClockwise ? ownImage.fillAmount != 1 : ownImage.fillAmount == 0;
        }
    }
}
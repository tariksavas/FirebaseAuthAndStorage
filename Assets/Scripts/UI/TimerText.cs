namespace Incru.UI
{
    using System.Collections;
    using TMPro;
    using UnityEngine;

    public class TimerText : TextMeshProUGUI
    {
        private float _closeTime = 5f;
        public float CloseTime { get => _closeTime; set => _closeTime = value; }

        public override string text
        {
            get => base.text;
            set
            {
                base.text = value;
                gameObject.SetActive(true);
                StopAllCoroutines();
                StartCoroutine(CloseTimer());
            }
        }

        private IEnumerator CloseTimer()
        {
            yield return new WaitForSeconds(CloseTime);
            gameObject.SetActive(false);
        }
    }
}
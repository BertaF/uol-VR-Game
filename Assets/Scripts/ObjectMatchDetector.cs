using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts
{
    public class ObjectMatchDetector : MonoBehaviour
    {
        [SerializeField] private float _maxDistToMatchObjects;
        [SerializeField] private UIManager _uiMgr;
        private bool _objectsMatched;

        private void OnTriggerEnter(Collider other)
        {
            _objectsMatched = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other || !other.CompareTag("Pickup")) return;

            float distance = Vector3.Distance(transform.position, other.transform.position);

            _objectsMatched = distance < _maxDistToMatchObjects;

            // Disable the ghostly object now that objects have been matched
            if (_objectsMatched && gameObject.activeSelf)
            {
                gameObject.SetActive(false);

                if (_uiMgr)
                {
                    _uiMgr.UpdateScore(1);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            gameObject.SetActive(true);
            _objectsMatched = false;
        }
    }
}

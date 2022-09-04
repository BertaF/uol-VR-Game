using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioSelector : MonoBehaviour
    {
        #region Member Variables
        [SerializeField] private AudioClip _ambient1;
        [SerializeField] private AudioClip _ambient2;
        [SerializeField] private AudioClip _ambient3;
        [SerializeField] private AudioSource _audioSource;
        #endregion

        private void Start()
        {
            StartCoroutine(Amb1());
        }

        private IEnumerator Amb1()
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.clip = _ambient1;
                _audioSource.Play();

                yield return new WaitForSeconds(_audioSource.clip.length);
                StartCoroutine(Amb2());
            }
        }

        private IEnumerator Amb2()
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.clip = _ambient2;
                _audioSource.Play();

                yield return new WaitForSeconds(_audioSource.clip.length);
                StartCoroutine(Amb3());
            }
        }

        private IEnumerator Amb3()
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.clip = _ambient3;
                _audioSource.Play();

                yield return new WaitForSeconds(_audioSource.clip.length);
                StartCoroutine(Amb1());
            }
        }
    }
}

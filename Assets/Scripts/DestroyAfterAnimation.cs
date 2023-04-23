using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    void Start() {
        this.GetComponent<AudioSource>().Play();
        Destroy(gameObject, this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
    }
}
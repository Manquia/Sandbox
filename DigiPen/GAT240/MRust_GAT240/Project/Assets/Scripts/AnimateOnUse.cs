using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateOnUse : MonoBehaviour {

    public bool singleUse = true;
    bool used = false;
    public AudioClip animateSound;
    void Start()
    {
        FFMessageBoard<PlayerInteract.Use>.Connect(OnUse, gameObject);
    }
    private void OnDestroy()
    {
        FFMessageBoard<PlayerInteract.Use>.Disconnect(OnUse, gameObject);
    }

    private void OnUse(PlayerInteract.Use e)
    {
        if (singleUse && used)
            return;

        if (singleUse)
            used = true;
        
        Animate();
    }
    

    public void Animate()
    {
        GetComponent<AudioSource>().PlayOneShot(animateSound);
        GetComponent<Animation>().Play();
    }
}

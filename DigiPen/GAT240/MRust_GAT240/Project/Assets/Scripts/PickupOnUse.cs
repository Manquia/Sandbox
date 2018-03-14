using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct PickupEvent
{
    public string name;

}

public class PickupOnUse : FFComponent {

    public bool singleUse = true;
    bool used = false;
    public AudioClip pickupSound;


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

        Pickup(e);
    }



    private void Pickup(PlayerInteract.Use e)
    {
        // Play effects
        if(GetComponent<AudioSource>() != null) GetComponent<AudioSource>().PlayOneShot(pickupSound);
        if(GetComponent<Animation>() != null) GetComponent<Animation>().Play();

        // Send out event to player
        PickupEvent pe;
        pe.name = gameObject.name;
        FFMessageBoard<PickupEvent>.SendToLocalToAllConnected(pe, e.playerCamera.gameObject);

        // Destroy self, @TODO pickup Animation
        DestroyAfterDelay(pickupSound.length);
    }



    void DestroyAfterDelay(float delay)
    {
        var seq = action.Sequence();
        seq.Delay(delay);
        seq.Sync();
        seq.Call(DestroySelf);
    }
    void DestroySelf()
    {
        Destroy(gameObject);
    }
}

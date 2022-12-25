using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour
{
    private PlayerMovement pm;
    public ParticleSystem jumpParticle;

    private void OnEnable()
    {
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        pm.onGroundJump += spawnJP;
    }
    private void OnDestroy()
    {
        pm.onGroundJump -= spawnJP;
    }

    private void spawnJP(int index)
    {
        Instantiate(jumpParticle, new Vector2(pm.transform.position.x, pm.transform.position.y), jumpParticle.transform.rotation);
    }
}

using System.Collections;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 2f;
    public MovementInput moveScript;
    public float speedBoost = 6;
    public Animator animator;
    public float animSpeedBoost = 1.5f;

    [Header("Mesh Releted")]
    public float meshRefreshRate = 1f;
    public float meshDestroyDelay = 3f;
    public Transform positionToSpawn;

    [Header("Shader Releted")]
    public Material material;
    public string shaderVerRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private bool isTrailActive;

    private float nomalSpeed;
    private float nomalAnimSpeed;

    IEnumerator AnimateMaterialFloat(Material m,float valueGoal, float rate, float refreshRate)
    {
        float valueToAnimate = m.GetFloat(shaderVerRef);

        while(valueToAnimate>valueGoal)
        {
            valueToAnimate -= rate;
            m.SetFloat(shaderVerRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }

    IEnumerator ActivateTrail(float timeActivated)
    {
        nomalSpeed = moveScript.movementSpeed;
        moveScript.movementSpeed=speedBoost;

        nomalAnimSpeed = animator.GetFloat("animSpeed");
        animator.SetFloat("animSpeed", animSpeedBoost);

        while(timeActivated>0)
        {
            if(skinnedMeshRenderers==null)
            {
                skinnedMeshRenderers = positionToSpawn.GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                GameObject gameObject = new GameObject();
                gameObject.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr =gameObject.AddComponent<MeshRenderer>();
                MeshFilter mf = gameObject.AddComponent<MeshFilter>();

                Mesh m = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(m);
                mf.mesh = m;
                mr.material = material;

                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

                Destroy(gameObject, meshDestroyDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }

        moveScript.movementSpeed=nomalSpeed;
        animator.SetFloat("animSpeed", nomalAnimSpeed);
        isTrailActive = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTrailActive)
        {
            StartCoroutine(ActivateTrail(activeTime));
            isTrailActive = true;
        }
    }
}

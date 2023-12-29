using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    Vector3 currentRotation;
    Vector3 targetRotation;

    public float snappiness;
    public float returnSpeed;

    private void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire(float recoilX, float recoilY, float recoilZ, float returnSpeed)
    {
        targetRotation = new Vector3(0, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        targetRotation.x += recoilX;
        this.returnSpeed = returnSpeed;
    }
}

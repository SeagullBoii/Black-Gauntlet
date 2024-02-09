using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    Vector3 currentRotation;
    Vector3 targetRotation;

    Vector3 currentPosition;
    Vector3 targetPosition;

    public float snappiness;
    public float returnSpeed;

    float currentPositionalRecoil = 0;
    float currentYRotationalRecoil = 0;

    private void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        
        targetPosition = Vector3.Lerp(targetPosition, Vector3.zero, returnSpeed * Time.deltaTime);
        currentPosition = Vector3.Slerp(targetPosition, Vector3.zero, snappiness * Time.fixedDeltaTime);

        currentPositionalRecoil = Mathf.Lerp(currentPositionalRecoil, 0, returnSpeed * Time.deltaTime);
        currentYRotationalRecoil = Mathf.Lerp(currentPositionalRecoil, 0, returnSpeed * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(currentRotation);
        transform.localPosition = currentPosition;
    }

    public void RecoilFire(float recoilX, float recoilY, float recoilZ, float returnSpeed, float positionalRecoilZ, float shotRecoilRatio)
    {
        if (currentPositionalRecoil < positionalRecoilZ)
        {
            targetPosition.z -= positionalRecoilZ * shotRecoilRatio;
            currentPositionalRecoil += positionalRecoilZ * shotRecoilRatio;
        }

        targetRotation = new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        targetRotation.x += recoilX*shotRecoilRatio;
        this.returnSpeed = returnSpeed;
    }
}

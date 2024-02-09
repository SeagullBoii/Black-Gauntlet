using System.Collections;
using UnityEngine;

public class Scythe : MonoBehaviour
{
    [Header("Charge")]
    public float maxCharge;
    public GameObject blade;

    [Header("Animation")]
    public int animationCount;

    [Header("Audio")]
    public AudioSource[] audioSources;
    public float[] pitchRandoms;
    public float[] startingPitches;

    [Header("Input")]
    public KeyCode meleeKey = KeyCode.F;

    [Header("References")]
    public ScytheDamage scytheDMGScript;

    public GameObject melee;
    public GameObject guns;

    public Transform model;

    [HideInInspector]
    public bool isSwinging;
    [HideInInspector]
    public bool canSwing = true;

    [HideInInspector]
    public float charge;

    int previousAnimation;

    Vector3 startPosition;
    Vector3 startRotation;
    PlayerInput playerInput;

    private void Awake()
    {
        playerInput = new PlayerInput();
        startPosition = model.localPosition;
        startRotation = model.localEulerAngles;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Update()
    {
        if (Time.timeScale != 0)
        {
            scytheDMGScript.charged = charge >= maxCharge;

            blade.SetActive(isSwinging && charge >= maxCharge);

            if (playerInput.Combat.Melee.triggered)
            {

                if (canSwing)
                    Swing();
            }

            Vector3 gunTargetPos = new Vector3(guns.transform.localPosition.x, -0.75F, guns.transform.localPosition.z);
            if (isSwinging)
                guns.transform.localPosition = Vector3.Lerp(guns.transform.localPosition, gunTargetPos, 12.5001f * Time.deltaTime);
            else if (guns.transform.localPosition != Vector3.zero)
                guns.transform.localPosition = Vector3.Lerp(guns.transform.localPosition, Vector3.zero, 12.5f * Time.deltaTime);
        }
    }

    private void Swing()
    {
        canSwing = false;
        isSwinging = true;

        //Randomization
        int randomInt = Random.Range(1, animationCount + 1);

        if (previousAnimation == randomInt)
        {
            if (randomInt == animationCount)
                randomInt = Random.Range(1, animationCount + 1);
            else
                randomInt += Random.Range(1, animationCount - randomInt + 1);
        }

        if (charge >= maxCharge)
        {
            CameraShake.Invoke(0.25f, 1, 4f, 65, transform, 50);
            PlayAudio.PlaySound(audioSources[1], startingPitches[1], pitchRandoms[1]);
        }
        else
        {
            CameraShake.Invoke(0.25f, 1, 3, 15, transform, 50);
            PlayAudio.PlaySound(audioSources[0], startingPitches[0], pitchRandoms[0]);
        }
        Animator animator = melee.GetComponent<Animator>();

        switch (randomInt)
        {
            case 1: animator.Play("ScytheSwing"); break;
            case 2: animator.Play("ScytheSwing1"); break;
            case 3: animator.Play("ScytheSwing2"); break;
            default: animator.Play("ScytheSwing3"); break;
        }

            //animator.SetTrigger("Swing");

            //Reset
            previousAnimation = randomInt;

            Invoke(nameof(ResetGunPos), 0.3f);
            Invoke(nameof(ResetSwing), 0.4f);
        }

        private void ResetGunPos()
        {
            model.localPosition = startPosition;
            model.localEulerAngles = startRotation;
            isSwinging = false;
        }

        private void ResetSwing()
        {
            Animator animator = melee.GetComponent<Animator>();
            // animator.Play("Idle");
            canSwing = true;
        }

    }

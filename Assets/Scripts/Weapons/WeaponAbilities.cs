using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Weapons))]
public class WeaponAbilities : MonoBehaviour
{
    [Header("Conditions")]
    public bool hasFirstWeaponMod;
    public bool hasThirdWeaponMod;

    [Header("Pistol")]
    public GameObject revolverGrenade;

    [Header("Assault Rifle")]
    public Vector3 aimingPos;
    public TrailRenderer bulletTrail;
    public GameObject electricHitFX;

    [Header("Audio")]
    public AudioSource sfx;
    public AudioMixerGroup audioMixerGroup;

    [Header("References")]
    public PlayerCameraMovement playerCameraMovement;


    [HideInInspector] public bool aiming;
    [HideInInspector] public float firstWeaponModCooldown;
    [HideInInspector] public float thirdWeaponModCooldown;

    float currentWeaponID;
    GameObject currentWeapon;

    Transform cam;

    Weapons weapons;
    PlayerMovement pm;
    PlayerInput playerInput;

    private void Awake()
    {
        playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Start()
    {
        weapons = GetComponent<Weapons>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        TryGetComponent<Scythe>(out Scythe scythe);

        if (Time.timeScale != 0)
        {

            //Handle Cooldowns
            if (firstWeaponModCooldown > 0) firstWeaponModCooldown -= Time.deltaTime;
            if (thirdWeaponModCooldown > 0) thirdWeaponModCooldown -= Time.deltaTime;

            if (aiming)
            {
                weapons.currentWeapon.transform.localPosition = Vector3.Lerp(weapons.currentWeapon.transform.localPosition, aimingPos * 2, 10 * Time.deltaTime);
            }
            else
            {
                weapons.currentWeapon.transform.localPosition = Vector3.Lerp(weapons.currentWeapon.transform.localPosition, Vector3.zero, 10 * Time.deltaTime);
                if (!pm.dashing)
                    playerCameraMovement.DoFOV(PlayerPrefs.GetFloat("FOV"));
            }

            if (playerInput.Combat.WeaponAbility.ReadValue<float>() != 1 || (scythe != null && scythe.isSwinging)) aiming = false;

            //Ability
            if (playerInput.Combat.WeaponAbility.triggered && weapons.currentWeapon != null)
            {
                if (scythe != null && scythe.canSwing)
                    WeaponAbility();
                else if (scythe == null)
                    WeaponAbility();
            }

            if (aiming && playerInput.Combat.Fire.triggered)
                PowerShot();

            currentWeaponID = weapons.currentWeaponID;
            currentWeapon = weapons.currentWeapon;
            cam = weapons.cam;
        }
    }

    private void WeaponAbility()
    {
        Gun gun = weapons.loadout[weapons.currentWeaponID];

        if (weapons.currentWeaponID == 0 && firstWeaponModCooldown <= 0 && hasFirstWeaponMod)
        {
            PistolGrenade();
            firstWeaponModCooldown = weapons.loadout[weapons.currentWeaponID].weaponModCooldown;
        }

        else if (weapons.currentWeaponID == 2 && thirdWeaponModCooldown <= 0 && hasThirdWeaponMod)
        {
            aiming = true;

            if (PlayerPrefs.GetFloat("FOV") - 60 > 0.00001f)
                playerCameraMovement.DoFOV(PlayerPrefs.GetFloat("FOV") - 60);
            else
                playerCameraMovement.DoFOV(5);
        }
    }

    private void PistolGrenade()
    {
        Gun gun = weapons.loadout[weapons.currentWeaponID];
        Transform muzzleFlashTransform = gun.prefab.GetComponent<GunObjects>().muzzleFlash;

        //Spawning
        GameObject newGrenade = Instantiate(revolverGrenade, muzzleFlashTransform.position, weapons.weaponParent.rotation, weapons.weaponParent) as GameObject;
        Vector3 grenadePosition = muzzleFlashTransform.localPosition;
        newGrenade.transform.localPosition = grenadePosition;
        grenadePosition.y += 0.25f;
        newGrenade.transform.localEulerAngles = muzzleFlashTransform.localEulerAngles;
        newGrenade.transform.localScale = new Vector3(.7f, .7f, .7f);
        newGrenade.transform.parent = null;
        newGrenade.GetComponent<RevolverGrenade>().scythe = GetComponent<Scythe>();


        newGrenade.GetComponent<Rigidbody>().velocity = cam.forward * 50;

        //FX
        float pitch = 1 + Random.Range(-gun.pitchRandomization, gun.pitchRandomization);
        PlaySound(gun.abilitySound, pitch, 1.2f, audioMixerGroup);

        weapons.recoilScript.RecoilFire(gun.recoilX * 2.5f, gun.recoilY * 2.5f, gun.recoilZ * 2.5f, weapons.recoilScript.returnSpeed);
        weapons.gunRecoilScript.RecoilFire(gun.gunRecoilX, gun.gunRecoilY * 2.5f, gun.gunRecoilZ * 2.5f, gun.returnSpeed);
    }

    private void PowerShot()
    {
        Transform spawn = cam;
        Gun gun = weapons.loadout[weapons.currentWeaponID];
        if (currentWeapon.GetComponent<Animator>())
            currentWeapon.GetComponent<Animator>().Play("AbilityShoot", 0, 0);

        //Audio
        float pitch = 1 + Random.Range(-gun.pitchRandomization, gun.pitchRandomization);
        PlaySound(gun.abilitySound, pitch, 1, audioMixerGroup);

        //Recoil
        weapons.recoilScript.RecoilFire(gun.recoilX * 2.5f, gun.recoilY * 5f, gun.recoilZ * 5f, gun.returnSpeed / 2);
        weapons.gunRecoilScript.RecoilFire(gun.gunRecoilX * 2.5f, gun.gunRecoilY * 2.5f, gun.gunRecoilZ * 2.5f, gun.returnSpeed / 2);

        //Bloom
        Vector3 bloom = spawn.forward;

        //Raycast
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(spawn.position, bloom, out hit, 1000f, weapons.canBeShot))
        {
            GameObject collisionObject = hit.collider.gameObject;

            //Impact FX
            weapons.SpawnBulletHole(hit, collisionObject);
            SpawnElectricityFX(hit, hit.collider.gameObject);
            if (collisionObject.TryGetComponent<HitEffect>(out HitEffect hitEffects)) hitEffects.SpawnParticles(collisionObject, hit);

            //Deal Damage 
            if (collisionObject.TryGetComponent<HurtBox>(out HurtBox hurtBox) && hit.collider == hurtBox.hitCollider)
            {
                hurtBox.DealDamage(12.5f, true, DamageType.DamageTypes.normal, gun.criticalMultiplier, hit);
                hurtBox.scythe = GetComponent<Scythe>();
            }
            else if (collisionObject.TryGetComponent<Health>(out Health health))
            {
                health.DealDamage(12.5f);
                health.scythe = GetComponent<Scythe>();
                weapons.didHit = true;
                weapons.hitTime = 0.25f;
                health.SpawnDamagePopup(hit);
            }

            if (collisionObject.TryGetComponent<RevolverGrenade>(out RevolverGrenade grenade))
            {
                if (!grenade.hasCollided)
                    grenade.Explode(RevolverGrenade.ExplosionType.Red);
                else
                    grenade.Explode(RevolverGrenade.ExplosionType.Normal);
                weapons.didHit = true;
                weapons.hitTime = 0.15f;
            }

            if (collisionObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForce((hit.point - transform.position) * gun.knockbackForce * 10, ForceMode.Impulse);
            }

            SpawnElectricTrail(spawn.forward, (hit.point - spawn.position).magnitude);

        }
        else
        {
            SpawnElectricTrail(spawn.forward, 100);
        }

        weapons.shotCooldown = 0.5f;
        thirdWeaponModCooldown = gun.weaponModCooldown;
        aiming = false;
    }

    private void SpawnElectricityFX(RaycastHit hit, GameObject collisionObject)
    {
        GameObject electricityFX = Instantiate(electricHitFX, hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
        Destroy(electricityFX, 2);
    }

    //Trails
    private void SpawnElectricTrail(Vector3 direction, float distance)
    {
        Transform spawn = cam;
        Ray ray = new Ray(spawn.position, direction);
        TrailRenderer trail = Instantiate(bulletTrail, weapons.currentWeapon.GetComponent<GunObjects>().muzzleFlash.transform.position, Quaternion.identity);

        StartCoroutine(SpawnTrail(trail, ray.GetPoint(distance)));
    }

    IEnumerator SpawnTrail(TrailRenderer trail, Vector3 targetPos)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, targetPos, time);
            time += Time.deltaTime * 5;

            yield return null;
        }

        trail.transform.position = targetPos;

        Destroy(trail.gameObject, trail.time);
    }

    //Sound
    private void PlaySound(AudioClip clip, float pitch, float volume, AudioMixerGroup mixerGroup)
    {
        //Play Gunshot Sound
        sfx.clip = clip;
        sfx.pitch = pitch;
        sfx.volume = volume;
        sfx.outputAudioMixerGroup = mixerGroup;
        sfx.PlayOneShot(clip);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;

[RequireComponent(typeof(WeaponAbilities))]
public class Weapons : MonoBehaviour, IDataPersistence
{
    [Header("Guns")]
    public GameObject weapons;
    public Gun[] loadout;

    [Header("Input")]
    public KeyCode firstWeapon = KeyCode.Alpha1;
    public KeyCode secondWeapon = KeyCode.Alpha2;
    public KeyCode thirdWeapon = KeyCode.Alpha3;

    [Header("Effects")]
    public GameObject bulletHole;
    public GameObject bulletHoleSmoke;
    public AudioSource sfx;
    public AudioMixerGroup audioGroup;
    public AudioClip holster;

    [Header("References")]
    public Transform weaponParent;

    public Transform cam;
    public Transform camRecoil;

    public LayerMask canBeShot;

    public PlayerCameraMovement playerCameraMovement;

    public GameObject crosshair;

    [HideInInspector] public bool didHit;
    [HideInInspector] public float hitTime;

    [HideInInspector] public int currentWeaponID;
    [HideInInspector] public GameObject currentWeapon;

    [HideInInspector] public float shotCooldown;

    public Recoil recoilScript { get; private set; }
    public Recoil gunRecoilScript { get; private set; }

    Health health;
    PlayerMovement pm;
    WeaponAbilities weaponAbilities;

    PlayerInput playerInput;

    public bool[] unlockedWeapons;

    public void SaveData(GameData gameData)
    {
        gameData.weapons = unlockedWeapons;
    }

    public void LoadData(GameData gameData) {
        unlockedWeapons = gameData.weapons;
    }

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
        health = GetComponent<Health>();
        recoilScript = camRecoil.GetComponent<Recoil>();
        gunRecoilScript = weapons.GetComponent<Recoil>();
        pm = GetComponent<PlayerMovement>();
        weaponAbilities = GetComponent<WeaponAbilities>();
        Equip(0);
    }

    private void Update()
    {
        gameObject.TryGetComponent<Scythe>(out Scythe scytheScript);

        if (Time.timeScale != 0)
        {
            if (scytheScript != null)
            {
                if (scytheScript.canSwing)
                    HandleInput();

            }
            else
            {
                HandleInput();

            }

            if (shotCooldown > 0) shotCooldown -= Time.deltaTime;
            if (hitTime > 0)
                hitTime -= Time.deltaTime;
            else
                didHit = false;
        }
    }

    public void HandleInput()
    {

        //Equipping
        if (playerInput.Combat.FirstWeapon.triggered) Equip(0);
        if (playerInput.Combat.SecondWeapon.triggered) Equip(1);
        if (playerInput.Combat.ThirdWeapon.triggered) Equip(2);

        if (playerInput.UI.ScrollWheel.ReadValue<Vector2>().y > 0.5f || playerInput.Combat.ChangeWeaponsUp.ReadValue<float>() > 0.5f)
        {
            if (currentWeaponID == 0) Equip(loadout.Length - 1);
            else Equip(currentWeaponID - 1);
        }
        else if (playerInput.UI.ScrollWheel.ReadValue<Vector2>().y < -0.5f || playerInput.Combat.ChangeWeaponsDown.ReadValue<float>() > 0.5f)
        {
            if (currentWeaponID == loadout.Length - 1) Equip(0);
            else Equip(currentWeaponID + 1);
        }

        //Shooting
        if (!health.isDead && !weaponAbilities.aiming)
        {
            if (loadout[currentWeaponID].firingState == Gun.FiringState.semi)
            {
                if (playerInput.Combat.Fire.triggered && currentWeapon != null && shotCooldown <= 0)
                {
                    Shoot();
                }
            }
            else if (loadout[currentWeaponID].firingState == Gun.FiringState.auto)
            {
                if (playerInput.Combat.Fire.ReadValue<float>() > 0 && currentWeapon != null && shotCooldown <= 0)
                {
                    Shoot();
                }
            }
        }

    }


    private void Equip(int id)
    {


        if (currentWeapon == null)
        {
            SpawnWeapon(id);
        }
        else if (currentWeaponID != id)
        {
            float pitch = 1 + Random.Range(0.1f, 0.25f);
            PlaySound(holster, pitch, 0.5f, audioGroup);
            Destroy(currentWeapon);
            SpawnWeapon(id);

            weaponAbilities.aiming = false;
            shotCooldown = 0.15f;
        }

        currentWeapon.GetComponent<GunObjects>().weapons = this;
        currentWeapon.GetComponent<GunObjects>().weaponAbilities = GetComponent<WeaponAbilities>();

    }

    private void SpawnWeapon(int id)
    {
        GameObject newEquipment = Instantiate(loadout[id].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;

        newEquipment.transform.localPosition = Vector3.zero;
        newEquipment.transform.localEulerAngles = Vector3.zero;

        currentWeaponID = id;
        currentWeapon = newEquipment;
    }

    private void Shoot()
    {
        Transform spawn = cam;
        Gun gun = loadout[currentWeaponID];

        //Bloom
        Vector3 bloom = spawn.position + spawn.forward * 1000f;
        bloom += Random.Range(-gun.bloom, gun.bloom) * spawn.up;
        bloom += Random.Range(-gun.bloom, gun.bloom) * spawn.right;
        bloom -= spawn.position;
        bloom.Normalize();

        Effects(gun);

        //Raycast
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(spawn.position, bloom, out hit, 1000f, canBeShot))
        {
            GameObject collisionObject = hit.collider.gameObject;

            //Impact FX
            SpawnBulletHole(hit, collisionObject);
            SpawnBulletFX(hit, hit.collider.gameObject);

            //Deal Damage
            if (collisionObject.TryGetComponent<HurtBox>(out HurtBox hurtBox) && hit.collider == hurtBox.hitCollider)
            {
                hurtBox.DealDamage(gun.damage, true, DamageType.DamageTypes.normal, gun.criticalMultiplier, hit);
                hurtBox.scythe = GetComponent<Scythe>();
                didHit = true;
                hitTime = 0.25f;
            }
            else if (collisionObject.TryGetComponent<Health>(out Health health))
            {
                health.DealDamage(gun.damage, true);
                health.scythe = GetComponent<Scythe>();
                didHit = true;
                hitTime = 0.25f;
                health.SpawnDamagePopup(hit.point + hit.normal * 0.001f);
            }
            if (collisionObject.TryGetComponent<RevolverGrenade>(out RevolverGrenade grenade))
            {
                if (!grenade.hasCollided)
                    grenade.Explode(RevolverGrenade.ExplosionType.Red);
                else
                    grenade.Explode(RevolverGrenade.ExplosionType.Normal);
                didHit = true;
                hitTime = 0.25f;
            }

            if (collisionObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForce((hit.point - transform.position) * gun.knockbackForce, ForceMode.Impulse);
            }
            StartTrail(spawn.forward, (hit.point - spawn.position).magnitude);

        }
        else
        {
            StartTrail(spawn.forward, 100);
        }

        //Cooldown
        shotCooldown = gun.fireRate;
    }

    //Trails
    private void StartTrail(Vector3 direction, float distance)
    {
        Transform spawn = cam;
        Ray ray = new Ray(spawn.position, direction);
        GameObject trail = Instantiate(loadout[currentWeaponID].bulletTrail, currentWeapon.GetComponent<GunObjects>().muzzleFlash.transform.position, Quaternion.identity);

        StartCoroutine(SpawnTrail(trail, ray.GetPoint(distance)));
    }

    IEnumerator SpawnTrail(GameObject trail, Vector3 targetPos)
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

        Destroy(trail.gameObject, .5f);
    }

    public void SpawnBulletHole(RaycastHit hit, GameObject collisionObject)
    {
        if (collisionObject.TryGetComponent<HitEffect>(out HitEffect hitFX))
        {
            if (hitFX.bulletHole)
            {
                GameObject newBulletHole = Instantiate(bulletHole, hit.point + hit.normal * 0.001f, Quaternion.identity, hit.collider.gameObject.transform) as GameObject;
                newBulletHole.transform.LookAt(hit.point + hit.normal);
                newBulletHole.transform.localScale = new Vector3(1 / collisionObject.transform.localScale.x, 1 / collisionObject.transform.localScale.y, 1 / collisionObject.transform.localScale.z);

                Destroy(newBulletHole, 5f);
            }
        }
        else
        {
            GameObject newBulletHole = Instantiate(bulletHole, hit.point + hit.normal * 0.001f, Quaternion.identity, hit.collider.gameObject.transform) as GameObject;
            newBulletHole.transform.LookAt(hit.point + hit.normal);
            newBulletHole.transform.localScale = new Vector3(1 / collisionObject.transform.localScale.x, 1 / collisionObject.transform.localScale.y, 1 / collisionObject.transform.localScale.z);
            Destroy(newBulletHole, 5f);
        }
    }

    public void SpawnBulletFX(RaycastHit hit, GameObject collisionObject)
    {
        GameObject hitObject = collisionObject.GetComponent<HurtBox>() ? collisionObject.GetComponent<HurtBox>().health.gameObject : collisionObject;

        if (hitObject.TryGetComponent<HitEffect>(out HitEffect hitEffects))
        {
            hitEffects.SpawnParticles(hitObject, hit);
        }
        else
        {
            GameObject newBulletHoleSmoke = Instantiate(bulletHoleSmoke, hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
            Destroy(newBulletHoleSmoke, 0.90f);
        }
    }

    private void PlaySound(AudioClip clip, float pitch, float volume, AudioMixerGroup mixerGroup)
    {
        //Play Gunshot Sound
        sfx.clip = clip;
        sfx.pitch = pitch;
        sfx.volume = volume;
        sfx.outputAudioMixerGroup = mixerGroup;
        sfx.PlayOneShot(clip);
    }

    private void Effects(Gun gun)
    {
        //Animation
        if (currentWeapon.GetComponent<Animator>() && !weaponAbilities.aiming)
        {
            currentWeapon.GetComponent<Animator>().Play("Shoot", 0, 0);
        }

        //Audio
        float pitch = 1 + Random.Range(-gun.pitchRandomization, gun.pitchRandomization);
        PlaySound(gun.gunShotSound, pitch, 1, audioGroup);

        //Recoil
        recoilScript.RecoilFire(gun.recoilX, gun.recoilY, gun.recoilZ, gun.returnSpeed, 0, gun.shotRecoilRatio);
        gunRecoilScript.RecoilFire(gun.gunRecoilX, gun.gunRecoilY, gun.gunRecoilZ, gun.returnSpeed, 0, gun.shotRecoilRatio);

        //Crosshair
        if (crosshair.GetComponent<DynamicCrosshair>())
        {
            crosshair.GetComponent<DynamicCrosshair>().MoveParts();
        }
    }
}

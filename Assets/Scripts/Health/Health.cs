using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class Health : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth;

    [Header("Effects")]
    public bool bleed = true;
    public bool damagePopup = true;

    public float pitch;
    public float pitchChange;
    public AudioClip audioClip;
    public AudioMixerGroup mixer;

    [Header("VFX")]
    public GameObject damagePopupPrefab;

    [Header("References")]
    public AudioSource sfx;
    public Scythe scythe;

    bool charge;
    [HideInInspector] public bool isDead;
    [HideInInspector] public float health;
    [HideInInspector] public float realDamage;

    HitEffect hitEffects;
    DamageType.DamageTypes damageType;

    // Start is called before the first frame update
    private void Start()
    {
        health = maxHealth;
        hitEffects = GetComponent<HitEffect>();
    }

    private void Update()
    {
        Mathf.Clamp(health, 0, maxHealth);
        if (health <= 0 && gameObject.tag != "Player")
        {
            if (scythe != null && scythe.charge < scythe.maxCharge && tag == "Enemy" && charge) scythe.charge++;
            Destroy(gameObject);
        }

        if (health > maxHealth) health = maxHealth;

    }

    public void DealDamage(float damage)
    {
        realDamage = damage;
        health -= realDamage;

        //Audio
        if (audioClip != null && sfx != null)
        {
            sfx.clip = audioClip;
            sfx.pitch = pitch + Random.Range(-pitchChange, pitchChange);
            sfx.outputAudioMixerGroup = mixer;
            sfx.Play();
        }

        //Charge Scythe
        charge = true;
    }

    public void DealDamage(float damage, bool chargeScythe)
    {
        realDamage = damage;
        health -= realDamage;

        //Audio
        if (audioClip != null && sfx != null)
        {
            sfx.clip = audioClip;
            sfx.pitch = pitch + Random.Range(-pitchChange, pitchChange);
            sfx.outputAudioMixerGroup = mixer;
            sfx.Play();
        }

        //Charge Scythe
        charge = chargeScythe;
    }

    public void DealDamage(float damage, bool chargeScythe, DamageType.DamageTypes damageType)
    {
        realDamage = damage;
        health -= realDamage;

        //Audio
        if (audioClip != null && sfx != null)
        {
            sfx.clip = audioClip;
            sfx.pitch = pitch + Random.Range(-pitchChange, pitchChange);
            sfx.outputAudioMixerGroup = mixer;
            sfx.Play();
        }

        //Charge Scythe
        charge = chargeScythe;
    }

    public void SpawnDamagePopup(Vector3 position)
    {
        if (!damagePopup) return;

        GameObject damagePopupObject = Instantiate(damagePopupPrefab, position, Quaternion.identity);
        damagePopupObject.GetComponent<DamagePopup>().damage = realDamage;

        float xDistance = Mathf.Abs(position.x - Camera.main.transform.position.x);
        float yDistance = Mathf.Abs(position.y - Camera.main.transform.position.y);
        float zDistance = Mathf.Abs(position.z - Camera.main.transform.position.z);
        Vector3 distance = new Vector3(xDistance, yDistance, zDistance);

        damagePopupObject.transform.localScale = new Vector3(distance.magnitude, distance.magnitude, distance.magnitude);

        float fontSize = damagePopupObject.transform.localScale.magnitude;
        if (fontSize < 10) fontSize = 10;
        if (damagePopupObject.TryGetComponent<TMP_Text>(out TMP_Text tmpText)) tmpText.fontSize = 2.5f * fontSize;
    }
    public void SpawnDamagePopup(Vector3 position, DamageType.DamageTypes type)
    {
        if (!damagePopup) return;

        GameObject damagePopupObject = Instantiate(damagePopupPrefab, position, Quaternion.identity);
        damagePopupObject.GetComponent<DamagePopup>().damage = realDamage;

        float xDistance = Mathf.Abs(position.x - Camera.main.transform.position.x);
        float yDistance = Mathf.Abs(position.y - Camera.main.transform.position.y);
        float zDistance = Mathf.Abs(position.z - Camera.main.transform.position.z);
        Vector3 distance = new Vector3(xDistance, yDistance, zDistance);

        damagePopupObject.transform.localScale = new Vector3(distance.magnitude, distance.magnitude, distance.magnitude);

        float fontSize = damagePopupObject.transform.localScale.magnitude;
        if (fontSize < 10) fontSize = 10;

        if (damagePopupObject.TryGetComponent<TMP_Text>(out TMP_Text tmpText))
        {
            tmpText.fontSize = 2.5f * fontSize;
            tmpText.color = DamageType.DamageColor(damageType);
        }
    }

    public void SpawnDamagePopup(RaycastHit hit)
    {
        if (!damagePopup) return;

        Vector3 position = hit.point + hit.normal * 0.001f;

        GameObject collisionObject = hit.collider.gameObject;
        GameObject damagePopupObject = Instantiate(damagePopupPrefab, position, Quaternion.identity);
        damagePopupObject.GetComponent<DamagePopup>().damage = realDamage;

        float xDistance = Mathf.Abs(position.x - Camera.main.transform.position.x);
        float yDistance = Mathf.Abs(position.y - Camera.main.transform.position.y);
        float zDistance = Mathf.Abs(position.z - Camera.main.transform.position.z);
        Vector3 distance = new Vector3(xDistance, yDistance, zDistance);

        damagePopupObject.transform.localScale = new Vector3(distance.magnitude, distance.magnitude, distance.magnitude);

        float fontSize = damagePopupObject.transform.localScale.magnitude;
        if (fontSize < 10) fontSize = 10;


        if (damagePopupObject.TryGetComponent<TMP_Text>(out TMP_Text tmpText))
        {
            tmpText.fontSize = 2.5f * fontSize;
        }
    }

    public void SpawnDamagePopup(RaycastHit hit, DamageType.DamageTypes type)
    {
        if (!damagePopup) return;

        damageType = type;
        Vector3 position = hit.point + hit.normal * 0.001f;

        GameObject collisionObject = hit.collider.gameObject;
        GameObject damagePopupObject = Instantiate(damagePopupPrefab, position, Quaternion.identity);
        damagePopupObject.GetComponent<DamagePopup>().damage = realDamage;

        float xDistance = Mathf.Abs(position.x - Camera.main.transform.position.x);
        float yDistance = Mathf.Abs(position.y - Camera.main.transform.position.y);
        float zDistance = Mathf.Abs(position.z - Camera.main.transform.position.z);
        Vector3 distance = new Vector3(xDistance, yDistance, zDistance);

        damagePopupObject.transform.localScale = new Vector3(distance.magnitude, distance.magnitude, distance.magnitude);

        float fontSize = damagePopupObject.transform.localScale.magnitude;
        if (fontSize < 10) fontSize = 10;


        if (damagePopupObject.TryGetComponent<TMP_Text>(out TMP_Text tmpText))
        {
            tmpText.fontSize = 2.5f * fontSize;
            tmpText.color = DamageType.DamageColor(damageType);
        }
    }
}

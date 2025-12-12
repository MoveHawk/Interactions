using UnityEngine;
using TMPro;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public TextMeshProUGUI ammoText;

    [Header("Weapon Models (3D Viewmodels)")]
    public GameObject pistolModel;
    public GameObject rifleModel;

    [Header("Weapon Holder Movement")]
    public Transform weaponHolder;
    public float lowerAmount = 0.3f;
    public float transitionSpeed = 8f;
    public float weaponSwitchDelay = 0.3f;

    [Header("Weapons (ScriptableObjects)")]
    public WeaponData pistol;
    public WeaponData rifle;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip pistolFire;
    public AudioClip rifleFire;
    public AudioClip reloadSound;

    [Header("Recoil Settings")]
    public float recoilReturnSpeed = 6f;
    public float recoilSnapSpeed = 12f;

    private Vector2 recoil;
    private Vector2 recoilTarget;

    private WeaponData currentWeapon;
    private GameObject currentModel;

    private float nextFireTime = 0f;
    private bool isSwitching = false;
    private bool isReloading = false;

    Vector3 holderDefaultPos;

    void Start()
    {
        holderDefaultPos = weaponHolder.localPosition;
        EquipWeapon(pistol, pistolModel);
    }

    void Update()
    {
        if (isSwitching || isReloading) return;

        HandleWeaponSwitch();
        HandleShooting();
        HandleReload();
        HandleRecoil();
    }

    // ---------------------------------------------------------
    //  WEAPON SWITCHING
    // ---------------------------------------------------------
    void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            StartCoroutine(SwitchWeaponRoutine(pistol, pistolModel));

        if (Input.GetKeyDown(KeyCode.Alpha2))
            StartCoroutine(SwitchWeaponRoutine(rifle, rifleModel));
    }

    IEnumerator SwitchWeaponRoutine(WeaponData newWeapon, GameObject newModel)
    {
        if (currentWeapon == newWeapon) yield break;
        isSwitching = true;

        yield return StartCoroutine(LowerWeapon());

        if (currentModel != null)
            currentModel.SetActive(false);

        EquipWeapon(newWeapon, newModel);

        yield return new WaitForSeconds(weaponSwitchDelay);
        yield return StartCoroutine(RaiseWeapon());

        isSwitching = false;
    }

    void EquipWeapon(WeaponData weapon, GameObject model)
    {
        currentWeapon = weapon;
        currentModel = model;
        currentModel.SetActive(true);
        UpdateAmmoUI();
    }

    // ---------------------------------------------------------
    //  SHOOTING
    // ---------------------------------------------------------
    void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            if (currentWeapon.currentAmmo > 0)
            {
                nextFireTime = Time.time + currentWeapon.fireRate;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        currentWeapon.currentAmmo--;
        UpdateAmmoUI();

        if (currentWeapon == pistol && pistolFire) audioSource.PlayOneShot(pistolFire);
        else if (currentWeapon == rifle && rifleFire) audioSource.PlayOneShot(rifleFire);

        // RAYCAST DAMAGE
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, currentWeapon.range))
        {
            if (hit.transform.TryGetComponent<GroundedEnemy>(out var enemy))
                enemy.TakeDamage(currentWeapon.damage);
        }

        ApplyRecoil();
    }

    // ---------------------------------------------------------
    //  RELOAD
    // ---------------------------------------------------------
    void HandleReload()
    {
        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        if (isReloading) yield break;
        if (currentWeapon.currentAmmo == currentWeapon.maxAmmo) yield break;
        if (currentWeapon.reserveAmmo <= 0) yield break;

        isReloading = true;
        yield return StartCoroutine(LowerWeapon());

        if (reloadSound)
            audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(weaponSwitchDelay);

        int need = currentWeapon.maxAmmo - currentWeapon.currentAmmo;
        int load = Mathf.Min(need, currentWeapon.reserveAmmo);

        currentWeapon.currentAmmo += load;
        currentWeapon.reserveAmmo -= load;

        UpdateAmmoUI();
        yield return StartCoroutine(RaiseWeapon());

        isReloading = false;
    }

    // ---------------------------------------------------------
    //  LOWER / RAISE
    // ---------------------------------------------------------
    IEnumerator LowerWeapon()
    {
        Vector3 lowered = holderDefaultPos + Vector3.down * lowerAmount;

        while (Vector3.Distance(weaponHolder.localPosition, lowered) > 0.01f)
        {
            weaponHolder.localPosition = Vector3.Lerp(
                weaponHolder.localPosition, lowered,
                Time.deltaTime * transitionSpeed
            );
            yield return null;
        }
    }

    IEnumerator RaiseWeapon()
    {
        while (Vector3.Distance(weaponHolder.localPosition, holderDefaultPos) > 0.01f)
        {
            weaponHolder.localPosition = Vector3.Lerp(
                weaponHolder.localPosition, holderDefaultPos,
                Time.deltaTime * transitionSpeed
            );
            yield return null;
        }
    }

    // ---------------------------------------------------------
    //  RECOIL
    // ---------------------------------------------------------
    void ApplyRecoil()
    {
        float x = Random.Range(-currentWeapon.recoilX, currentWeapon.recoilX);
        float y = currentWeapon.recoilY;
        recoilTarget += new Vector2(x, y);
    }

    void HandleRecoil()
    {
        recoil = Vector2.Lerp(recoil, recoilTarget, Time.deltaTime * recoilSnapSpeed);
        recoilTarget = Vector2.Lerp(recoilTarget, Vector2.zero, Time.deltaTime * recoilReturnSpeed);
        cam.transform.localRotation *= Quaternion.Euler(-recoil.y, recoil.x, 0);
    }

    // ---------------------------------------------------------
    //  UI
    // ---------------------------------------------------------
    void UpdateAmmoUI()
    {
        ammoText.text = $"{currentWeapon.currentAmmo} / {currentWeapon.reserveAmmo}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Component
{
    [SerializeField]
    private int maxAmmo;
    private int ammo;
    public enum wepType { manual, immobile, missiles };
    private Vector3 lastTargetDir;
    public wepType targetType;
    [SerializeField]
    private float cooldownMax;
    private float cooldown;
    [SerializeField]
    private float maxRange;
    [SerializeField]
    private Projectile projectile;
    [SerializeField]
    private Transform projectileSpawnPos;
    public bool isPlayer = false;
    private Generator generator;

    private void Awake()
    {
        ammo = maxAmmo;
        generator = FindObjectOfType<Generator>();
    }

    private void FixedUpdate()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.fixedDeltaTime * (ship.health / ship.maxHealth);
        }
    }

    private void Update()
    {
        // follow mouse
        if (targetType == wepType.manual)
        {
            if (isPlayer)
            {
                Vector2 mousePos = Input.mousePosition;
                Camera cam = Camera.main;
                Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.transform.position.y));
                Vector3 temp = mouseWorldPos - transform.position;
                lastTargetDir = temp;
                temp.y = 0;
                transform.localRotation = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(transform.parent.right, temp, Vector3.up) + 90f);
            } else
            {
                //look at target ship
                Ship parent = GetComponentInParent<Ship>();
                Vector3 targetPos = parent.moveTarget;
                if (parent.target != null)
                {
                    targetPos = parent.target.command.transform.position;
                }
                Vector3 temp = targetPos - transform.position;
                lastTargetDir = temp;
                temp.y = 0;
                transform.localRotation = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(transform.parent.right, temp, Vector3.up) + 90f);
            }
            return;
        }
        // follow nearest missile
        if (targetType == wepType.missiles)
        {
            //get closest missile to the turret
            Missile closestMissile = null;
            float closestDistance = float.MaxValue;
            foreach (Missile m in generator.missiles)
            {
                if (m == null || m.player == isPlayer)
                {
                    continue;
                }
                float tempDist;
                if ((tempDist = (m.transform.position - transform.position).magnitude) < closestDistance)
                {
                    closestDistance = tempDist;
                    closestMissile = m;
                }
            }
            if (closestDistance > maxRange)
            {
                return;
            }
            Vector3 missileVel = closestMissile.GetComponent<Rigidbody>().velocity;
            float impactTime = (closestMissile.transform.position - transform.position).magnitude / missileVel.magnitude;
            Vector3 direction = ((closestMissile.transform.position - transform.position) / (impactTime / 2) + missileVel) / projectile.speed;
            lastTargetDir = direction;
            direction.y = 0;
            transform.localRotation = Quaternion.Euler(0f, 0f, Vector3.SignedAngle(transform.parent.right, direction, Vector3.up) + 90f);
            shoot(GetComponentInParent<Ship>().GetComponent<Rigidbody>());
            return;
        }
    }

    public void shoot(Rigidbody rigid)
    {
        if (cooldown > 0 || ammo <= 0)
        {
            return;
        }
        if (targetType == wepType.manual || targetType == wepType.missiles)
        {
            GameObject g = Instantiate(projectile.gameObject, transform.position, Quaternion.LookRotation(lastTargetDir));
            g.GetComponent<Projectile>().player = isPlayer;
            rigid.AddForceAtPosition(-1 * lastTargetDir * g.GetComponent<Projectile>().recoilForce, transform.position, ForceMode.Force);
        } else
        {
            GameObject g = Instantiate(projectile.gameObject, transform.position, transform.rotation);
            g.GetComponent<Projectile>().player = isPlayer;
            rigid.AddForceAtPosition(-1 * rigid.transform.forward * g.GetComponent<Projectile>().recoilForce, transform.position, ForceMode.Force);
        }
        cooldown = cooldownMax;
        ammo--;
        
    }
}

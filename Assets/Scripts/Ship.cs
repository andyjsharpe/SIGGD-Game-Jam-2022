using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private Rigidbody rigid;
    private Slot[] slots;
    public Command command;
    private Component[] components;
    public bool isPlayer = false;
    public bool pirate;
    public bool aggressive = false;
    public Ship target;
    public Vector3 moveTarget = Vector3.zero;
    private float aggroDist = 200;
    private ShipMinimap minimap;
    private Generator generator;
    public int maxHealth = 100;
    [HideInInspector]
    public int health = 100;
    private float wanderRange = 400;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        slots = GetComponentsInChildren<Slot>();
        rigid = GetComponent<Rigidbody>();
        components = GetComponentsInChildren<Component>();
        generator = GameObject.FindObjectOfType<Generator>();
        minimap = GetComponentInChildren<ShipMinimap>();
    }

    public float healthRatio()
    {
        return Mathf.Max((float)health / (float)maxHealth, 0.5f);
    }

    private void FixedUpdate()
    {
        if (health <= 0)
        {
            return;
        }
        //AI controll here
        if (!isPlayer)
        {
            //pirate code
            if (pirate)
            {
                // if pirate has no target
                if (target == null) {
                    Ship closestShip = null;
                    float closestDist = float.MaxValue;
                    //get closest non-pirate
                    foreach (Ship s in generator.ships)
                    {
                        if (!s.pirate)
                        {
                            float tempDist;
                            if ((tempDist = (s.transform.position - transform.position).magnitude) < closestDist)
                            {
                                closestDist = tempDist;
                                closestShip = s;
                            }
                        }
                    }
                    if (closestDist < aggroDist)
                    {
                        target = closestShip;
                    } else
                    {
                        //if no target found, continue moving
                        moveToTarget();
                    }
                } else
                {
                    //if the pirate has a target
                    float distance = (target.transform.position - transform.position).magnitude;
                    if (distance > aggroDist * 2 || target.health <= 0)
                    {
                        target = null;
                        return;
                    }
                    // if target is still in range, attack and move
                    moveToShip();
                    if (distance < aggroDist)
                    {
                        shoot();
                    }
                }
            } 
            //non pirate code
            else
            {
                //if not aggro, move to target
                if (!aggressive)
                {
                    moveToTarget();
                }
                //if aggro, attack the ship
                else
                {
                    if (target != null)
                    {
                        shoot();
                        moveToShip();
                    } else
                    {
                        //if ship DNE, stop being aggressive
                        aggressive = false;
                        minimap.setColor(1);
                    }
                }
            }
        }
    }

    //generates a new ship procedurally
    public Ship generate(bool forPlayer, float fillChance, Material[] metals, Material[] windows, Material[] paints, Command[] commands, Engine[] engines, Weapon[] weapons, Inner[] inners, Thruster[] thrusters)
    {
        ShipMinimap minimap = GetComponentInChildren<ShipMinimap>();
        isPlayer = forPlayer;
        if (!forPlayer)
        {
            pirate = Random.Range(0, 2) == 0;
        }
        slots = GetComponentsInChildren<Slot>();
        rigid = GetComponent<Rigidbody>();
        //setup materials
        Material[] matSetup = { metals[Random.Range(0, metals.Length)], paints[Random.Range(0, paints.Length)], windows[Random.Range(0, windows.Length)] };
        GetComponentInChildren<MeshRenderer>().materials = matSetup;
        bool ins = false;
        bool wep = false;
        foreach (Slot slot in slots)
        {
            //setup command
            if (slot.type == Slot.slotType.command)
            {
                GameObject g = Instantiate(commands[Random.Range(0, commands.Length)].gameObject, slot.transform.position, transform.rotation * Quaternion.Euler(-90, 180, 0), slot.transform);
                slot.markComponent(g);
            }
            //setup engines
            else if (slot.type == Slot.slotType.engine)
            {
                GameObject g = Instantiate(engines[Random.Range(0, engines.Length)].gameObject, slot.transform.position, transform.rotation * Quaternion.Euler(-90, 180, 0), slot.transform);
                slot.markComponent(g);
            }
            //setup weapons
            else if (slot.type == Slot.slotType.weapon)
            {
                if (!wep || Random.Range(0f, 1f) < fillChance)
                {
                    GameObject g = Instantiate(weapons[Random.Range(0, weapons.Length)].gameObject, slot.transform.position, transform.rotation * Quaternion.Euler(-90, 180, 0), slot.transform);
                    slot.markComponent(g);
                    g.GetComponent<Weapon>().isPlayer = isPlayer;
                    wep = true;
                }
            }
            //setup inner
            else
            {
                if (!ins)
                {
                    GameObject g = Instantiate(thrusters[Random.Range(0, thrusters.Length)].gameObject, slot.transform.position, transform.rotation * Quaternion.Euler(-90, 180, 0), slot.transform);
                    slot.markComponent(g);
                    ins = true;
                }   
                else if (Random.Range(0f, 1f) < fillChance)
                {
                    GameObject g = Instantiate(inners[Random.Range(0, inners.Length)].gameObject, slot.transform.position, transform.rotation * Quaternion.Euler(-90, 180, 0), slot.transform);
                    slot.markComponent(g);
                }
            }
        }
        components = GetComponentsInChildren<Component>();
        foreach (Component component in components)
        {
            component.ship = this;
        }
        command = GetComponentInChildren<Command>();
        //set colors
        minimap.commandRend = command.transform.GetComponent<MeshRenderer>();
        if (!forPlayer)
        {
            if (pirate)
            {
                minimap.setColor(3);
            }
            else
            {
                minimap.setColor(1);
            }
        }
        else
        {
            minimap.setColor(0);
        }
        return this;
    }

    private void moveToShip()
    {
        float dist = (target.transform.position - transform.position).magnitude;
        //if distance far, move to pos
        if (dist > 30)
        {
            moveToPos(target.transform.position);
        } 
        // if distance to close, slow down
        else if (dist < 15)
        {
            backward();
        }
    }

    private void moveToTarget()
    {
        //if posiiton target DNE or reached, pick a new one
        if (moveTarget == Vector3.zero || (moveTarget - transform.position).magnitude < 20)
        {
            // if close to center, choose random location, otherwise go nearer to center
            Vector2 randPos = Random.insideUnitCircle * wanderRange;
            if (transform.position.magnitude > wanderRange * 4) {
                moveTarget = transform.position + new Vector3(randPos.x, 0, randPos.y);
            } else
            {
                moveTarget = new Vector3(randPos.x * 2, 0, randPos.y * 2);
            }
        }
        else
        //if posiiton good, move to it
        {
            moveToPos(moveTarget);
        }
    }

    //tries to steer/move to the target position
    private void moveToPos(Vector3 Pos)
    {
        float signedAngle = Vector3.SignedAngle(-1 * transform.forward, Pos - transform.position, Vector3.up);
        float angleOff = Mathf.Abs(signedAngle);
        // move forward if target is not behind
        if (angleOff < 80)
        {
            forward();
        }
        //if rotation far, rotate towards
        if (angleOff > 10)
        {
            if (signedAngle > 0)
            {
                right();
            }
            else
            {
                left();
            }
        }
        //if rotation close, but angularVel high, rotate away
        else if (GetComponent<Rigidbody>().angularVelocity.magnitude > Mathf.PI / 8)
        {
            if (signedAngle > 0)
            {
                left();
            }
            else
            {
                right();
            }
        }
    }

    public void forward()
    {
        foreach (Component component in components)
        {
            if (component.type == Component.componentType.engine) {
                ((Engine)component).engineOn(rigid);
            }
        }
    }

    public void backward()
    {
        foreach (Component component in components)
        {
            if (component.type == Component.componentType.inner)
            {
                Thruster thruster = component as Thruster;
                if (thruster)
                {
                    thruster.thrusterBack(rigid);
                }
            }
        }
    }

    public void left()
    {
        foreach (Component component in components)
        {
            if (component.type == Component.componentType.inner)
            {
                Thruster thruster = component as Thruster;
                if (thruster)
                {
                    thruster.thrusterRotate(rigid, false);
                }
            }
        }
    }

    public void right()
    {
        foreach (Component component in components)
        {
            if (component.type == Component.componentType.inner)
            {
                Thruster thruster = component as Thruster;
                if (thruster)
                {
                    thruster.thrusterRotate(rigid, true);
                }
            }
        }
    }

    public void shoot()
    {
        foreach (Component component in components)
        {
            if (component.type == Component.componentType.weapon && ((Weapon)component).targetType != Weapon.wepType.missiles)
            {
                ((Weapon)component).shoot(rigid);
            }
        }
    }

    public void damage(int damage, Ship originator)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0;
            minimap.setColor(4);
            generator.recalcShips();
        }
        target = originator;
        aggressive = true;
    }
}

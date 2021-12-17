using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Rope2_backup : MonoBehaviour
{
    public enum RopeState
    {
        RolledUp,
        RollingDown,
        RollingUp,
        RolledDown
    }

    public JointSetup jointsConfig;
    public JointSetup hookConfig;
    public LineRenderer lr;

    public Transform ropeHandlePlacement;
    public GameObject hookJoint;
    public GameObject joint;

    public float distanceBetweenJoints = 1;
    public float ropeLength = 2;
    public float dropSpeed = 1;
    public bool upchain = true;
    public GameObject hook;

    private Rigidbody rb;
    private GameObject lastJoint;
    private List<GameObject> joints;

    private RopeState state = RopeState.RolledUp;

    private bool holdLastJoint;

    public bool jointsSelfParented;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        joints = new List<GameObject>();
    }

    private void OnValidate()
    {
        if(hook & !lastJoint)
        {
            lastJoint = hook;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(lastJoint.transform.position, 0.2f);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            DropTheLine();
        }

        Debug.DrawLine(lastJoint.transform.position, ropeHandlePlacement.position, Color.red);

        switch (state)
        {
            case RopeState.RollingDown:

                if (Vector3.Distance(ropeHandlePlacement.position, hook.transform.position) < ropeLength)   // ===> refactor to use Lerp
                {
                    //Vector3.MoveTowards()
                    
                    lastJoint.GetComponent<Rigidbody>().MovePosition(lastJoint.transform.position + Vector3.down * dropSpeed * Time.fixedDeltaTime);
                    //lastJoint.GetComponent<Rigidbody>().position = Vector3.MoveTowards(lastJoint.transform.position, ropeHandlePlacement.position + Vector3.down * ropeLength, dropSpeed * Time.deltaTime);

                    if (Vector3.Distance(ropeHandlePlacement.position, lastJoint.transform.position) > (jointsConfig.autoConfigure ? distanceBetweenJoints : jointsConfig.anchor.y))
                    {
                        SpawnNewJoint();
                    }
                }
                else
                {
                    StopRollingAndActivate();
                }
                break;


            case RopeState.RollingUp:
               
                    if (Vector3.Distance(hook.transform.position, ropeHandlePlacement.position) > 0.01)
                    {
                        lastJoint.transform.position = Vector3.MoveTowards(lastJoint.transform.position, ropeHandlePlacement.position, dropSpeed * Time.deltaTime);

                        if ((lastJoint.transform.position - ropeHandlePlacement.position).magnitude < 0.01)
                        {
                            if (lastJoint == hook)
                            {
                                FinishRollingUp();
                                return;
                            }
                            int i = joints.IndexOf(lastJoint);
                            Destroy(lastJoint);
                            lastJoint = joints[i - 1];
                        //Destroy(lastJoint.GetComponent<HingeJoint>());
                        lastJoint.GetComponent<Rigidbody>().isKinematic = true;
                        }
                    }
                break;

            case RopeState.RolledDown:
                 if (holdLastJoint)
                {
                    if (!lastJoint)
                        return;
                    lastJoint.GetComponent<Rigidbody>().MovePosition(ropeHandlePlacement.position * Time.deltaTime);
                    //lastJoint.transform.position = Vector3.MoveTowards(transform.position, ropeHandlePlacement.position, distanceBetweenJoints * Time.deltaTime);
                }
                break;
        }
    }

    private void FinishRollingUp()
    {
        state = RopeState.RolledUp;
    }

    private void SpawnNewJoint()
    {

        GameObject newJoint = Instantiate(joint, ropeHandlePlacement.position, Quaternion.identity);
        newJoint.GetComponent<Rigidbody>().isKinematic = true;
        if (upchain)
        {
            // Last top point becomes interactive and connected to new kinematic joint
            (lastJoint.AddComponent<HingeJoint>() as HingeJoint).connectedBody = newJoint.GetComponent<Rigidbody>();
            if(lastJoint == hook)
            {
                ConfigureRigidbody(hook.GetComponent<Rigidbody>(), hookConfig);

            }else
            {
                ConfigureJoint(lastJoint.GetComponent<HingeJoint>(), jointsConfig);
                ConfigureRigidbody(lastJoint.GetComponent<Rigidbody>(), jointsConfig);
            }
        }
        else
        {
            if(lastJoint != hook)
            {
                (newJoint.AddComponent<HingeJoint>() as HingeJoint).connectedBody = lastJoint.GetComponent<Rigidbody>();
                
                ConfigureJoint(newJoint.GetComponent<HingeJoint>(), jointsConfig);
                ConfigureRigidbody(lastJoint.GetComponent<Rigidbody>(), jointsConfig);
            }
            else
            {
                ConfigureRigidbody(hook.GetComponent<Rigidbody>(), hookConfig);
            }

        }

        if (jointsSelfParented)
        {
            if (upchain)
            {
                lastJoint.transform.parent = newJoint.transform;  
            }
            else
            {
                newJoint.transform.parent = lastJoint.transform.parent;
            }
        }
        else
        {
            newJoint.transform.parent = ropeHandlePlacement;
        }

        joints.Add(lastJoint = newJoint);
    }

    void DropTheLine()
    {
        switch (state)
        {
            case RopeState.RolledUp:
                state = RopeState.RollingDown;

                joints = new List<GameObject>();
                if (!hook)
                {
                    hook = lastJoint = Instantiate(hookJoint,
                                                    ropeHandlePlacement.position,
                                                    Quaternion.identity
                                                    );

                }
                lastJoint = hook;
                joints.Add(hook);
                
                hook.GetComponent<Rigidbody>().isKinematic = true;
                hook.transform.parent = ropeHandlePlacement;
                //if (upchain)
                //    hook.GetComponent<HingeJoint>().connectedBody = rb;
                //else
                //    GetComponent<HingeJoint>().connectedBody = hook.GetComponent<Rigidbody>();

            break;

            case RopeState.RolledDown:
                RollUpTheLine();
            break;
        }

    }

    void RollUpTheLine()
    {
        lastJoint.GetComponent<Rigidbody>().isKinematic = true;
        //Destroy(lastJoint.GetComponent<HingeJoint>());
        //joints[joints.Count - 1].GetComponent<Rigidbody>().isKinematic = true;
        //foreach (GameObject j in joints)
        //{
        //    j.GetComponent<Rigidbody>().isKinematic = true;
        //    j.transform.parent = hook.transform;
        //}

        state = RopeState.RollingUp;
    }

    private void StopRollingAndActivate()
    {
        state = RopeState.RolledDown;

        //hook.transform.position = ropeHandlePlacement.position + (hook.transform.position - ropeHandlePlacement.position).normalized * ropeLength;
        if(!holdLastJoint)
        {
            (lastJoint.AddComponent(typeof(HingeJoint)) as HingeJoint).connectedBody = ropeHandlePlacement.parent.GetComponent<Rigidbody>();
            ConfigureJoint(lastJoint.GetComponent<HingeJoint>(), jointsConfig);
            ConfigureRigidbody(lastJoint.GetComponent<Rigidbody>(), jointsConfig);
        }

        //ConfigureRigidbody(hook.GetComponent<Rigidbody>(), hookConfig);
        //ConfigureJoint(hook.GetComponent<HingeJoint>(), hookConfig);
        //foreach (GameObject joint in joints)
        //{
        //    joint.transform.parent = ropeHandlePlacement;
        //    ConfigureRigidbody(joint.GetComponent<Rigidbody>(), jointsConfig);
        //    //ConfigureJoint(joint.GetComponent<HingeJoint>(), jointsConfig);
        //}
    }
    private void ConfigureRigidbody(Rigidbody rb, JointSetup config)
    {
        rb.isKinematic = config.isKinematic;
        rb.drag = config.drag;
        rb.angularDrag = config.angularDrag;
        rb.constraints = config.constraints;
    }
    private void ConfigureJoint(HingeJoint joint, JointSetup config)
    {
        joint.autoConfigureConnectedAnchor = config.autoConfigure;

        if(!joint.autoConfigureConnectedAnchor)
        {
            joint.anchor = config.anchor / joint.transform.localScale.y;
            joint.axis = config.axis;
            joint.connectedAnchor = config.connectedAnchor;
        }


        joint.useSpring = config.useSpring;
        if (joint.useSpring)
        {

            JointSpring spring = new JointSpring() { spring = config.spring, damper = config.damper, targetPosition = config.targetPosition};
            joint.spring= spring;
        }

        joint.useLimits = config.useLimits;
        if (joint.useLimits)
        {
            JointLimits limits = new JointLimits() { min = config.minLim, max = config.maxLim, bounciness = config.bounciness, bounceMinVelocity = config.minBounceVelocity, contactDistance = config.contactDistance };
            joint.limits = limits;
        }

    }


}

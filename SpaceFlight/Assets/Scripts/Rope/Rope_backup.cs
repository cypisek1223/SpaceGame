using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rope_backup : MonoBehaviour
{
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

    private Rigidbody rb;
    private GameObject hook;
    private GameObject lastJoint;
    private List<GameObject> joints;
    private bool dropped;
    private bool rollingDown;
    private bool rollingUp;
    private bool holdLastJoint;

    private float t;
    private bool rolled = true;
    private bool rolledYet;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        joints = new List<GameObject>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            DropTheLine();
        }


        if (rollingDown)
        {
            if (Vector3.Distance(ropeHandlePlacement.position, hook.transform.position) < ropeLength)   // ===> refactor to use Lerp
            {
                //Vector3.MoveTowards()
                hook.GetComponent<Rigidbody>().MovePosition(hook.transform.position + Vector3.down * dropSpeed);

                if (Vector3.Distance(ropeHandlePlacement.position, lastJoint.transform.position) > (jointsConfig.autoConfigure      ?     distanceBetweenJoints     :       jointsConfig.anchor.y))
                {
                    SpawnNewJoint();
                }
            }
            else if (!dropped)
            {
                    StopRollingAndActivate();


            }
        }
        else if(rollingUp)
        {
            if(Vector3.Distance(hook.transform.position, ropeHandlePlacement.position) > 0.01)
            {
                hook.transform.position = Vector3.MoveTowards(hook.transform.position, ropeHandlePlacement.position, dropSpeed);

                if((lastJoint.transform.position - ropeHandlePlacement.position).y >= 0)
                {
                    if (lastJoint == hook)
                        return;
                     int i = joints.IndexOf(lastJoint);
                    Destroy(lastJoint);
                    lastJoint = joints[i - 1];
                }
            }
            else if(!rolledYet)
            {
                rolled = true;
                rolledYet = true;
                rollingUp = false;
                dropped = false;
            }
        }
        else if(holdLastJoint)
        {
            if (!lastJoint)
                return;
            lastJoint.GetComponent<Rigidbody>().MovePosition(ropeHandlePlacement.position * Time.deltaTime);
            //lastJoint.transform.position = Vector3.MoveTowards(transform.position, ropeHandlePlacement.position, distanceBetweenJoints * Time.deltaTime);
        }
    }

    private void StopRollingAndActivate()
    {
        rolled = false;
        rollingDown = false;
        dropped = true;
        //hook.transform.position = ropeHandlePlacement.position + (hook.transform.position - ropeHandlePlacement.position).normalized * ropeLength;
        if(!holdLastJoint)
            (lastJoint.AddComponent(typeof(HingeJoint)) as HingeJoint).connectedBody = ropeHandlePlacement.parent.GetComponent<Rigidbody>();

        ConfigureRigidbody(hook.GetComponent<Rigidbody>(), hookConfig);
        ConfigureJoint(hook.GetComponent<HingeJoint>(), hookConfig);
        foreach (GameObject joint in joints)
        {
            joint.transform.parent = ropeHandlePlacement;
            ConfigureRigidbody(joint.GetComponent<Rigidbody>(), jointsConfig);
            //ConfigureJoint(joint.GetComponent<HingeJoint>(), jointsConfig);
        }
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

        if(!config.autoConfigure)
        {
            joint.anchor = config.anchor * joint.transform.localScale.y;
            joint.axis = config.axis;
            joint.connectedAnchor = config.connectedAnchor;
        }


        joint.useSpring = config.useSpring;
        if (config.useSpring)
        {

            JointSpring spring = new JointSpring() { spring = config.spring, damper = config.damper, targetPosition = config.targetPosition};
            joint.spring= spring;
        }

        joint.useLimits = config.useLimits;
        if (config.useLimits)
        {
            JointLimits limits = new JointLimits() { min = config.minLim, max = config.maxLim, bounciness = config.bounciness, bounceMinVelocity = config.minBounceVelocity, contactDistance = config.contactDistance };
            joint.limits = limits;
        }

    }

    private void SpawnNewJoint()
    {

            GameObject newJoint = Instantiate(joint, ropeHandlePlacement.position, Quaternion.identity);

            if (upchain)
        {
                (lastJoint.AddComponent<HingeJoint>() as HingeJoint).connectedBody = newJoint.GetComponent<Rigidbody>();
                ConfigureJoint(lastJoint.GetComponent<HingeJoint>(), jointsConfig);

        }
            else
        {
                (newJoint.AddComponent(typeof(HingeJoint)) as HingeJoint).connectedBody = lastJoint.GetComponent<Rigidbody>();
                ConfigureJoint(newJoint.GetComponent<HingeJoint>(), jointsConfig);

        }

            joints.Add(lastJoint = newJoint);
            lastJoint.GetComponent<Rigidbody>().isKinematic = true;
            lastJoint.transform.parent = hook.transform;
    }

    void DropTheLine()
    {
        if (rolled)  // tu jest problem
        {
            if (!hook)
            {
                hook = lastJoint = Instantiate(hookJoint,
                                                ropeHandlePlacement.position,
                                                Quaternion.identity
                                                );
                joints.Add(hook); 
            }
            else
            {
                joints = new List<GameObject>();
                joints.Add(hook);
            }
            hook.GetComponent<Rigidbody>().isKinematic = true;

            hook.transform.parent = ropeHandlePlacement;
            //if (upchain)
            //    hook.GetComponent<HingeJoint>().connectedBody = rb;
            //else
            //    GetComponent<HingeJoint>().connectedBody = hook.GetComponent<Rigidbody>();
            dropped = false;
            rollingDown = true; 
        }
        else
        {
            if(dropped)
            {
                RollUpTheLine();
            }
    

        }
    }

    void RollUpTheLine()
    {
        foreach(GameObject j in joints)
        {
            j.GetComponent<Rigidbody>().isKinematic = true;
            j.transform.parent = hook.transform;
        }

        rollingUp = true;
        rolledYet = false;
    }
}

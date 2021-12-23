using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RopeState
{
    RolledUp,
    RollingDown,
    RollingUp,
    RolledDown
}


public class RopeJoint
{
    public Rigidbody rb;
    public Joint joint;

    public bool hook;
    public RopeJoint previousOne;
    public RopeJoint nextOne;

    
    public Vector3 position { get { return rb.position; } set { rb.position = value; } }
   
    public Transform parent { get { return rb.transform.parent; } set { rb.transform.parent = value; } }

    public static implicit operator bool(RopeJoint j) => j!=null;
    public static implicit operator Transform(RopeJoint j) => j.rb.transform;
    public static bool operator true(RopeJoint j) => j != null;
    public static bool operator false(RopeJoint j) => j == null;

    public RopeJoint(GameObject go, bool isHook = false)
    {
        this.hook = isHook;
        this.rb = go.GetComponent<Rigidbody>();
        this.rb.isKinematic = true;

    }

    internal void AddConnection(RopeJoint connectedJoint, Type jointType)
    {
        //joint =rb.gameObject.AddComponent(joint.GetType()); // zrobiæ test
        joint = rb.gameObject.AddComponent(jointType) as Joint;
        joint.connectedBody = connectedJoint.rb;
        nextOne = connectedJoint;
        connectedJoint.previousOne = this;
    }



    internal void AddConnection(Rigidbody handle, Type jointType)
    {
        joint = rb.gameObject.AddComponent(jointType) as Joint;
        //rb.gameObject.AddComponent(joint.GetType()); // zrobiæ test
        joint.connectedBody = handle;

    }


}

public enum JointType
{
    HingeJoint,
    ConfigurableJoint,
    Spring
}


public class Rope : MonoBehaviour
{
    public JointSetup jointsConfig;
    public JointSetup hookConfig;

    public Transform ropeHandlePlacement;
    public GameObject hookJoint;
    public GameObject joint;
    public JointType typeOfJoint;
    Type jointType;

    public float distanceBetweenJoints = 1;
    public float ropeLength = 2;
    public float dropSpeed = 1;
    public bool upchain = true;
    public RopeJoint hook;

    private Rigidbody rb;
    private RopeJoint lastJoint;
    private List<RopeJoint> joints;
    private Stack<RopeJoint> stack_of_joints;
    private float JointsDistance => jointsConfig.autoConfigure? distanceBetweenJoints : jointsConfig.anchor.y;

    public RopeState state = RopeState.RolledUp;
    public bool holdLastJoint;
    public bool jointsSelfParented;

    public LineRenderer lr;
    private float t = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        joints = new List<RopeJoint>();
        stack_of_joints = new Stack<RopeJoint>();
    }

    private void OnValidate()
    {
        if(hook & !lastJoint)
        {
            lastJoint = hook;
            //hook.rb = 
        }

        switch (typeOfJoint)
        {
            case JointType.HingeJoint:
                jointType = typeof(HingeJoint);
                break;

            case JointType.ConfigurableJoint:
                jointType = typeof(ConfigurableJoint);
                break;

            case JointType.Spring:
                jointType = typeof(SpringJoint);
                break;

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(lastJoint)
            Gizmos.DrawWireSphere(lastJoint.position, 0.2f);
        else
            Gizmos.DrawWireSphere(ropeHandlePlacement.position, 0.2f);
            
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ToggleRope();
        }

        //Debug.DrawLine(lastJoint.position, ropeHandlePlacement.position, Color.red);
        Logic();
      
        RenderLine();
    }


    private void Logic()
    {
        switch (state)
        {
            case RopeState.RollingDown:

                RollingDownLogic();

                break;


            case RopeState.RollingUp:

                RollingUpLogic();

                break;

            case RopeState.RolledDown:
                if (holdLastJoint) // if this field holds true, rope handle does not have joint and must be hold programatically
                {
                    if (!lastJoint)
                        return;
                    lastJoint.rb.MovePosition(ropeHandlePlacement.position);// * Time.deltaTime);
                    //lastJoint.transform.position = Vector3.MoveTowards(transform.position, ropeHandlePlacement.position, distanceBetweenJoints * Time.deltaTime);
                }
                break;
        }
    }
    private void ToggleRope()
    {
        switch (state)
        {
            case RopeState.RolledUp:
                DropTheLine();
                break;

            case RopeState.RolledDown:
                RollUpTheLine();
                break;
        }
    }
    void DropTheLine()
    {
        state = RopeState.RollingDown;
        InitializeRollingDown();

    }
    private void InitializeRollingDown()
    {
        if (!hook)
        {
            hook = lastJoint = new RopeJoint(Instantiate(hookJoint,
                                            ropeHandlePlacement.position,
                                            Quaternion.identity
                                            ));

        }
        lastJoint = hook;
        hook.parent = ropeHandlePlacement;
        
        joints = new List<RopeJoint>();             //to remove
        stack_of_joints = new Stack<RopeJoint>();   //no needed in future


        joints.Add(hook); // joints ought to be working
        stack_of_joints.Push(hook); // with previous - next

        t = 0;
        //if (upchain)
        //    hook.GetComponent<HingeJoint>().connectedBody = rb;
        //else
        //    GetComponent<HingeJoint>().connectedBody = hook.GetComponent<Rigidbody>();

    }

    private void RollingDownLogic()
    {
        if (Vector3.Distance(ropeHandlePlacement.position, hook.position) < ropeLength)   // ===> refactor to use Lerp?
        {
            //Vector3.MoveTowards()
            // here use lerp
            t += Time.deltaTime;
            t /= dropSpeed;
            Vector3 targetPosition = ropeHandlePlacement.position -
                new Vector3(0f, jointsConfig.autoConfigure ? distanceBetweenJoints : jointsConfig.anchor.y, 0f);

            Debug.DrawLine(ropeHandlePlacement.position, targetPosition, Color.red);

            Vector3 position = Vector3.LerpUnclamped(ropeHandlePlacement.position, targetPosition, t);
            lastJoint.rb.MovePosition(position);
            //lastJoint.rb.MovePosition(ropeHandlePlacement.position + Vector3.down * dropSpeed * Time.fixedDeltaTime);
            //lastJoint.GetComponent<Rigidbody>().position = Vector3.MoveTowards(lastJoint.transform.position, ropeHandlePlacement.position + Vector3.down * ropeLength, dropSpeed * Time.deltaTime);

            //if (Vector3.Distance(ropeHandlePlacement.position, lastJoint.position) > (jointsConfig.autoConfigure ? distanceBetweenJoints : jointsConfig.anchor.y))
            if (t > 1)
            {
                SpawnNewJoint();

            }
        }
        else
        {
            StopRollingDownAndActivate();
        }
    }
    private void StopRollingDownAndActivate()
    {
        state = RopeState.RolledDown;

        Vector3 difference = ropeHandlePlacement.position - lastJoint.position;
        int leftToSpawn = (int)(difference.magnitude/JointsDistance + 0.1);
        for(int i=1; i <= leftToSpawn; i++)
        {
            RopeJoint newJoint =new RopeJoint( Instantiate(joint, difference.normalized * JointsDistance * i, Quaternion.identity) );

            if(upchain)
            {
                lastJoint.AddConnection(newJoint, jointType);
                ConfigureJoint(lastJoint.joint, jointsConfig);
                ConfigureRigidbody(newJoint.rb, jointsConfig);
            }
            else
            {
                newJoint.AddConnection(lastJoint, jointType);
                ConfigureJoint(newJoint.joint, jointsConfig);
                ConfigureRigidbody(lastJoint.rb, jointsConfig);
            }

            if (jointsSelfParented)
            {
                if (upchain)
                {
                    lastJoint.parent = newJoint;
                }
                else
                {
                    newJoint.parent = lastJoint;
                }
            }
            else
            {
                newJoint.parent = ropeHandlePlacement;
            }
            lastJoint = newJoint;
        }
        lastJoint.parent = ropeHandlePlacement;
        //hook.transform.position = ropeHandlePlacement.position + (hook.transform.position - ropeHandlePlacement.position).normalized * ropeLength;
        if (!holdLastJoint)
        {
            if (upchain)
            {
                lastJoint.AddConnection(ropeHandlePlacement.parent.GetComponent<Rigidbody>(), jointType);
                ConfigureJoint(lastJoint.joint, jointsConfig);
                ConfigureRigidbody(lastJoint.rb, jointsConfig); 
            }
            //else
            //{
            //    HingeJoint j = ropeHandlePlacement.gameObject.AddComponent<HingeJoint>();
            //    j.connectedBody = lastJoint.rb;
            //    //lastJoint.rb.isKinematic = false;
            //}    
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
    private void RollingUpLogic()
    {
        //if (Vector3.Distance(hook.position, ropeHandlePlacement.position) > 0.01)
        //{

            lastJoint.rb.transform.localPosition = Vector3.MoveTowards(lastJoint.rb.transform.localPosition, Vector3.zero, dropSpeed * Time.deltaTime);
            //lastJoint.position = Vector3.MoveTowards(lastJoint.position, ropeHandlePlacement.position, dropSpeed * Time.deltaTime);

            if ((lastJoint.position - ropeHandlePlacement.position).magnitude < 0.01)
            {
                if (lastJoint == hook)
                {
                    FinishRollingUp();
                    return;
                }
            //int i = joints.IndexOf(lastJoint);
            //Destroy(lastJoint);
            //lastJoint = joints[i - 1];

                RopeJoint temp;
                if (!upchain)
                {
                    temp = lastJoint.nextOne; 
                }
                else
                {
                    temp = lastJoint.previousOne; 

                }
                Destroy(lastJoint.rb.gameObject);
                stack_of_joints.Pop();
                lastJoint = temp;
            //Destroy(stack_of_joints.Pop().rb.gameObject);
            //lastJoint = stack_of_joints.Peek();

            //Destroy(lastJoint.GetComponent<HingeJoint>());

                lastJoint.rb.isKinematic = true; 
            
            }
        //}
    }


    void RollUpTheLine()
    {
        //deparent self parented
        foreach(RopeJoint j  in stack_of_joints)
        {
            j.parent = ropeHandlePlacement;
        }

        lastJoint.rb.isKinematic = true;
        //Destroy(lastJoint.GetComponent<HingeJoint>());
        //joints[joints.Count - 1].GetComponent<Rigidbody>().isKinematic = true;
        //foreach (GameObject j in joints)
        //{
        //    j.GetComponent<Rigidbody>().isKinematic = true;
        //    j.transform.parent = hook.transform;
        //}



        state = RopeState.RollingUp;
    }

    private void SpawnNewJoint()
    {
        // We got 2 joints: lastJoint and newJoint. We need to connect them
        RopeJoint newJoint;
        if (upchain)
        {
            newJoint = new RopeJoint(Instantiate(joint, ropeHandlePlacement.position, Quaternion.identity ));
            // Last top point becomes interactive and connected to new kinematic joint
            lastJoint.AddConnection(newJoint, jointType);
            if(lastJoint == hook)
            {
                ConfigureRigidbody(hook.rb, hookConfig);
                ConfigureJoint(lastJoint.joint, hookConfig);

            }else
            {
                ConfigureJoint(lastJoint.joint, jointsConfig);
                ConfigureRigidbody(lastJoint.rb, jointsConfig);
            }
        }
        else
        {
            newJoint = new RopeJoint(Instantiate(joint, ropeHandlePlacement.position, Quaternion.Euler(180,0,0)));
            newJoint.AddConnection(lastJoint, jointType);
            if(lastJoint == hook)
            {
                ConfigureRigidbody(hook.rb, hookConfig);
                ConfigureJoint(newJoint.joint, hookConfig);
            }
            else
            {
                //New joints spawn at the top of the rope, this case is downchain, so new joint connects to lastjoint
                ConfigureJoint(newJoint.joint, jointsConfig);
                ConfigureRigidbody(lastJoint.rb, jointsConfig);

            }

        }

        t = 0;

        newJoint.rb.name = stack_of_joints.Count.ToString();
            newJoint.parent = ropeHandlePlacement;
        if (jointsSelfParented)
        {
                lastJoint.parent = newJoint;  
            //if (upchain)
            //{

            //    lastJoint.parent = newJoint;  
            //}
            //else
            //{
            //    newJoint.parent = lastJoint;
            //}
        }
        //else
        //{
        //    newJoint.parent = ropeHandlePlacement;
        //}

        lastJoint = newJoint;
        joints.Add(newJoint);
        stack_of_joints.Push(newJoint);
    }
    private void FinishRollingUp()
    {
        state = RopeState.RolledUp;
    }



    private void RenderLine()
    {
        lr.positionCount = stack_of_joints.Count + 1;
        lr.SetPosition(0, ropeHandlePlacement.position);
        int i = 1;
        foreach(RopeJoint j in stack_of_joints)
        {
            lr.SetPosition(i, j.position);
            i++;
        }
    }


    private void ConfigureRigidbody(Rigidbody rb, JointSetup config)
    {
        rb.mass = config.mass;
        rb.isKinematic = config.isKinematic;
        rb.drag = config.drag;
        rb.angularDrag = config.angularDrag;
        rb.constraints = config.constraints;
    }
    private void ConfigureJoint(Joint joint, JointSetup config)
    {
        if(joint.GetType()== typeof(HingeJoint))
        {
                config.ConfigureHinge(joint as HingeJoint);

        }

        if (joint.GetType() == typeof(ConfigurableJoint))
        {
            config.ConfigureConfigurable(joint as ConfigurableJoint);

        }


        if(joint.GetType() == typeof(SpringJoint))
        {
            config.ConfigureSpring(joint as SpringJoint);
        }
    }


}

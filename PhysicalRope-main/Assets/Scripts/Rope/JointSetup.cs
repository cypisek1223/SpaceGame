using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JointConfiguration", menuName = "ScriptableObjects/Static Configurations/Hinge Joint Configuration", order = 1)]
public class JointSetup : ScriptableObject
{
    [Header("Rigidbody Configuration")]
    public bool isKinematic;
    public float mass;
    public float drag;
    public float angularDrag;

    public RigidbodyConstraints constraints;

    [Header("Hinge Joint Configuration")]
    public float massScale;
    public bool autoConfigure;
    public Vector3 anchor;
    public Vector3 axis;
    public Vector3 connectedAnchor;

    public bool useSpring;
    public float spring;
    public float damper;
    public float targetPosition;

    public bool useLimits;
    public float minLim;
    public float maxLim;
    public float bounciness;
    public float minBounceVelocity;
    public float contactDistance;

    [Header("Configurable Joint Specific")]
    public ConfigurableJointMotion Xmotion;
    public ConfigurableJointMotion Ymotion;
    public ConfigurableJointMotion Zmotion;
    public ConfigurableJointMotion XangularMotion;
    public ConfigurableJointMotion YangularMotion;
    public ConfigurableJointMotion ZangularMotion;

    public float linearLimit;
    public float linearLimitBounciness;
    public float linearLimitContactDistance;


    //Cyprian Zmiany: <-----------------------------------------------------------------------
    public bool EnablePreporcessing;
    public ConfigurableJointMotion Anchor;
    public void SetRigidbody(Rigidbody rb)
    {
        rb.isKinematic = this.isKinematic;
    }

    public void ConfigureHinge(HingeJoint j)
    {
        j.autoConfigureConnectedAnchor = this.autoConfigure;
        j.massScale = this.massScale;

        if (!j.autoConfigureConnectedAnchor)
        {
            j.anchor = this.anchor / j.transform.localScale.y;
            j.axis = this.axis;
            j.connectedAnchor = connectedAnchor;
        }


        j.useSpring = this.useSpring;
        if (j.useSpring)
        {

            JointSpring spring = new JointSpring() { spring =  this.spring, damper =  damper, targetPosition =  targetPosition };
            j.spring = spring;
        }

        j.useLimits =  useLimits;
        if (j.useLimits)
        {

             JointLimits limits = new JointLimits() { min = minLim, max = maxLim, bounciness = this.bounciness, bounceMinVelocity = this.minBounceVelocity, contactDistance = this.contactDistance }; 

            j.limits = limits;
        }
    }

    internal void ConfigureConfigurable(ConfigurableJoint j)
    {
        //j.autoConfigureConnectedAnchor = this.autoConfigure;
        j.massScale = this.massScale;
        j.autoConfigureConnectedAnchor = this.autoConfigure;
        j.autoConfigureConnectedAnchor = false;

        if (!j.autoConfigureConnectedAnchor)
        {
            j.anchor = this.anchor / j.transform.localScale.y;
            j.axis = this.axis;
            j.connectedAnchor = connectedAnchor;
        }

        

        j.xMotion = this.Xmotion;
        j.yMotion = this.Ymotion;
        j.zMotion = this.Zmotion;
        j.angularXMotion = this.XangularMotion;
        j.angularYMotion = this.YangularMotion;
        j.angularZMotion = this.ZangularMotion;

        SoftJointLimit linearLimit = new SoftJointLimit();
        linearLimit.limit = this.linearLimit;
        linearLimit.bounciness = this.linearLimitBounciness;
        linearLimit.contactDistance = this.linearLimitContactDistance;

        j.linearLimit = linearLimit;

        SoftJointLimitSpring linearLimitSpring = new SoftJointLimitSpring();
        linearLimitSpring.spring = this.spring;
        linearLimitSpring.damper = this.damper;

        j.linearLimitSpring = linearLimitSpring;

        //Cyprian Zmiany: <-----------------------------------------------------------------------
        //EnablePreporcessing = false;
        
       
        


        // Linear Limit Sprong
        //{  
        //     Spring = 1000
        // damper = 5
        // }
        //         Linear Limit
        // {
        //             Limit = 0
        // Bounciness = 1
        // Contact Distance = 1
        // }
        //
        // Rotation Drive Mode = Slerp



        //j.useSpring = this.useSpring;
        //if (j.useSpring)
        //{

        //    JointSpring spring = new JointSpring() { spring = this.spring, damper = damper, targetPosition = targetPosition };
        //    j.spring = spring;
        //}

        //j.useLimits = useLimits;
        //if (j.useLimits)
        //{

        //    JointLimits limits = new JointLimits() { min = minLim, max = maxLim, bounciness = this.bounciness, bounceMinVelocity = this.minBounceVelocity, contactDistance = this.contactDistance };

        //    j.limits = limits;
        //}
    }

    public void ConfigureSpring(SpringJoint j)
    
    {
        j.spring = spring;

        j.autoConfigureConnectedAnchor = this.autoConfigure;
        j.massScale = this.massScale;

        if (!j.autoConfigureConnectedAnchor)
        {
            j.anchor = this.anchor / j.transform.localScale.y;
            j.axis = this.axis;
            j.connectedAnchor = connectedAnchor;
        }
    }
}

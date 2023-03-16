using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IKSolver{

    public Transform rootBone;
    public Transform middleBone;
    public Transform endBone;

    float _weight;
    Vector3 hintPosition;


    /// <summary>
    /// Manual creation of the bone targets
    /// </summary>
    /// <param name="rootBone"></param>
    /// <param name="middleBone"></param>
    /// <param name="endBone"></param>
    public IKSolver(Transform rootBone, Transform middleBone, Transform endBone){
        this.rootBone = rootBone;
        this.middleBone = middleBone;
        this.endBone = endBone;
    }

    /// <summary>
    /// Auto creation of the bone targets
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="ikGoal"></param>
    public IKSolver(Animator animator, AvatarIKGoal iKGoal){
        if(animator == null) { return; }

        if (animator.isHuman){
            switch (iKGoal){
                case AvatarIKGoal.LeftHand:
                    rootBone = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                    middleBone = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                    endBone = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                    break;
                case AvatarIKGoal.RightHand:
                    rootBone = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                    middleBone = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
                    endBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
                    break;
                case AvatarIKGoal.LeftFoot:
                    rootBone = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
                    middleBone = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
                    endBone = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                    break;
                case AvatarIKGoal.RightFoot:
                    rootBone = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
                    middleBone = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
                    endBone = animator.GetBoneTransform(HumanBodyBones.RightFoot);
                    break;
            }
        }
    }

    /// <summary>
    /// Get IK Weight
    /// </summary>
   
    public virtual float ikWeight { get { return _weight; } }

    /// <summary>
    /// Set IK Weight
    /// </summary>
    /// <param name="weight"></param>
   public virtual void SetIKWeight(float weight){

        _weight = weight;
    }

    /// <summary>
    /// Set IK Position
    /// </summary>
    /// <param name="ikPosition"></param>
    public virtual void SetIKPosition(Vector3 ikPosition){

        if (!(rootBone && middleBone && endBone) || ikWeight <= 0.0f) return;

        Vector3 middleBoneDirection = Vector3.zero;
        if (hintPosition != Vector3.zero)
            middleBoneDirection = hintPosition - rootBone.position;
        else
            middleBoneDirection = Vector3.Cross(endBone.position - rootBone.position, Vector3.Cross(endBone.position - rootBone.position, endBone.position - middleBone.position));

        float rootBoneLenght = (middleBone.position - rootBone.position).magnitude;
        float middleBoneLenght = (endBone.position - middleBone.position).magnitude;

        Vector3 middleBonePos = GetHintPosition(rootBone.position, ikPosition, rootBoneLenght, middleBoneLenght, middleBoneDirection);

        Quaternion upperarmRotation = Quaternion.FromToRotation(middleBone.position - rootBone.position, middleBonePos - rootBone.position) * rootBone.rotation;
        if(!(System.Single.IsNaN(upperarmRotation.x) || System.Single.IsNaN(upperarmRotation.y) || System.Single.IsNaN(upperarmRotation.z))){

            rootBone.rotation = Quaternion.Slerp(rootBone.rotation, upperarmRotation, ikWeight);
            Quaternion middleBoneRotation = Quaternion.FromToRotation(endBone.position - middleBone.position, ikPosition - middleBonePos) * middleBone.rotation;
            middleBone.rotation = Quaternion.Slerp(middleBone.rotation, middleBoneRotation, ikWeight);
        }

        hintPosition = Vector3.zero;
    }


    /// <summary>
    /// Set IK Rotation
    /// </summary>
    /// <param name="rotation"></param>
    public virtual void SetIKRotation(Quaternion rotation)
    {
        if (!(rootBone && middleBone && endBone) || ikWeight <= 0.0f) return;
        endBone.rotation = Quaternion.Slerp(endBone.rotation, rotation, ikWeight);
    }

    /// <summary>
    /// Set IK Hint Position
    /// ps: Call before SetIKPosition
    /// </summary>
    /// <param name="hintPosition"></param>
    public virtual void SetIKHintPosition(Vector3 hintPosition)
    {
        this.hintPosition = hintPosition;
    }

    /// <summary>
    /// Get IK Hint Position
    /// </summary>
    /// <param name="rootPos"></param>
    /// <param name="endPos"></param>
    /// <param name="rootBoneLength"></param>
    /// <param name="middleBoneLength"></param>
    /// <param name="middleBoneDirection"></param>
    protected virtual Vector3 GetHintPosition(Vector3 rootPos, Vector3 endPos, float rootBoneLength, float middleBoneLength, Vector3 middleBoneDirection)
    {
        Vector3 rootToEndDir = endPos - rootPos;
        float rootToEndMag = rootToEndDir.magnitude;

        float maxDist = (rootBoneLength + middleBoneLength) * 0.999f;
        if (rootToEndMag > maxDist)
        {
            endPos = rootPos + (rootToEndDir.normalized * maxDist);
            rootToEndDir = endPos - rootPos;
            rootToEndMag = maxDist;
        }

        float minDist = Mathf.Abs(rootBoneLength - middleBoneLength) * 1.001f;
        if (rootToEndMag < minDist)
        {
            endPos = rootPos + (rootToEndDir.normalized * minDist);
            rootToEndDir = endPos - rootPos;
            rootToEndMag = minDist;
        }

        float aa = ((rootToEndMag * rootToEndMag + rootBoneLength * rootBoneLength - middleBoneLength * middleBoneLength) * 0.5f) / rootToEndMag;
        float bb = Mathf.Sqrt(rootBoneLength * rootBoneLength - aa * aa);
        Vector3 crossElbow = Vector3.Cross(rootToEndDir, Vector3.Cross(middleBoneDirection, rootToEndDir));
        return rootPos + (aa * rootToEndDir.normalized) + (bb * crossElbow.normalized);
    }
}


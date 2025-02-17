﻿using UnityEngine;
using UnityEngine.UI;

public class WristsPositionsDebug : MonoBehaviour
{
    [SerializeField] Text debugTxt;

    void Update()
    {
        nuitrack.SkeletonData skd = NuitrackManager.SkeletonData;
        if (skd != null)
        {
            float lz = skd.Skeletons[0].GetJoint(nuitrack.JointType.LeftWrist).Real.Z;
            float rz = skd.Skeletons[0].GetJoint(nuitrack.JointType.RightWrist).Real.Z;

            float lx = skd.Skeletons[0].GetJoint(nuitrack.JointType.LeftWrist).Real.X;
            float rx = skd.Skeletons[0].GetJoint(nuitrack.JointType.RightWrist).Real.X;

            float deltaZ = lz - rz;
            float deltaX = lx - rx;

            float alpha = Mathf.Atan2(deltaZ, deltaX) * Mathf.Rad2Deg;

            debugTxt.text = string.Format("dz = " + deltaZ.ToString("0") + System.Environment.NewLine + "dx = " + deltaX.ToString("0") + System.Environment.NewLine + "angle = " + alpha.ToString("0"));
        }
    }
}

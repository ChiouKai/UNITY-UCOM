// (c) Copyright 2014 Luke Light&Magic. All rights reserved.

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class EffectScaler : MonoBehaviour {

  #if UNITY_EDITOR

  public float EffectScale = 1;
  public bool ScaleLight = true;
  public bool ScaleTrailEffect = true;

  private float eScale = 1;

	void Awake () {
    if (EffectScale > 0)
      eScale = EffectScale = transform.localScale.x;
	}

  void Update() {
    if (eScale != EffectScale) {
      if (EffectScale <= 0) {
        EffectScale = eScale;
        return;
      }

      float scaleValue = EffectScale / eScale;
      transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
      ScaleParticeSystems(scaleValue);
      if (ScaleLight) ScaleLights(scaleValue);
      if (ScaleTrailEffect) ScaleTrailEffects(scaleValue);
      eScale = EffectScale;
      transform.localScale = new Vector3(eScale, eScale, eScale);
    }
	
	}


  void ScaleParticeSystems(float scaleValue) {
    ParticleSystem[] psSystems = GetComponentsInChildren<ParticleSystem>();
    if (psSystems != null) {
      for (int i = 0; i < psSystems.Length; i++) {
        psSystems[i].startSpeed *= scaleValue;
        psSystems[i].startSize *= scaleValue;
        psSystems[i].gravityModifier *= scaleValue;

        SerializedObject otherProperties = new SerializedObject(psSystems[i]);
        otherProperties.FindProperty("VelocityModule.x.scalar").floatValue *= scaleValue;
        otherProperties.FindProperty("VelocityModule.y.scalar").floatValue *= scaleValue;
        otherProperties.FindProperty("VelocityModule.z.scalar").floatValue *= scaleValue;
        otherProperties.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= scaleValue;
        otherProperties.FindProperty("ClampVelocityModule.x.scalar").floatValue *= scaleValue;
        otherProperties.FindProperty("ClampVelocityModule.y.scalar").floatValue *= scaleValue;
        otherProperties.FindProperty("ClampVelocityModule.z.scalar").floatValue *= scaleValue;
        otherProperties.FindProperty("ForceModule.x.scalar").floatValue *= scaleValue;
        otherProperties.FindProperty("ForceModule.y.scalar").floatValue *= scaleValue;
        otherProperties.FindProperty("ForceModule.z.scalar").floatValue *= scaleValue;
        otherProperties.FindProperty("ColorBySpeedModule.range").vector2Value *= scaleValue;
        otherProperties.FindProperty("SizeBySpeedModule.range").vector2Value *= scaleValue;
        otherProperties.FindProperty("RotationBySpeedModule.range").vector2Value *= scaleValue;
        otherProperties.ApplyModifiedProperties();
      }
    }

  }

  void ScaleLights(float scaleValue) {
    Light[] ligths = GetComponentsInChildren<Light>();
    if (ligths !=null)
      for (int i = 0; i < ligths.Length; i++) 
        ligths[i].range *= scaleValue;
  }

  void ScaleTrailEffects(float scaleValue) {
    TrailRenderer[] trailRer = GetComponentsInChildren<TrailRenderer>();
    if (trailRer != null)
      for (int i = 0; i < trailRer.Length; i++) {
        trailRer[i].startWidth *= scaleValue;
        trailRer[i].endWidth *= scaleValue;
      }
    LineRenderer[] lineRer = GetComponentsInChildren<LineRenderer>();
    if (lineRer != null)
      for (int i = 0; i < lineRer.Length; i++) {
        SerializedObject otherProperties = new SerializedObject(lineRer[i]);
        otherProperties.FindProperty("m_Parameters.startWidth").floatValue *= scaleValue;
        otherProperties.FindProperty("m_Parameters.endWidth").floatValue *= scaleValue;
        otherProperties.ApplyModifiedProperties();
      }
    LineRenderContrl[] lineRerCtrl = GetComponentsInChildren<LineRenderContrl>();
    if (lineRerCtrl != null)
      for (int i = 0; i < lineRerCtrl.Length; i++)
        lineRerCtrl[i].TexDistanceDivValue *= scaleValue;
  }

  #endif
}

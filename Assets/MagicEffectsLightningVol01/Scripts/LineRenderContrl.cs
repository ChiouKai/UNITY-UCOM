// (c) Copyright 2014 Luke Light&Magic. All rights reserved.

using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRenderContrl : MonoBehaviour {
  public Transform Target;
  public Vector3 TargetOffset;
  public float TexMoverTotalTime = 1, TexSheetTotalTime = 1;
  public int TexSheetCount = 1, TexTilingVector = -1;
  public bool TexSheetRandom = false, TexAutoDistanceDiv=false;
  public float TexDistanceDivValue = 4;

  private LineRenderer lineRer;
  private GameObject lineRerTarget;
  private Material mat;
  private Vector2 texOffset;
  private float[] sheetCoordinates;
  private float timerMover, timerSheet;
  private int sheetIndex, texMoverVector = 1, texAutoDistDiv = 1;


  void Awake() {
    lineRer = GetComponent<LineRenderer>();
    lineRer.SetVertexCount(2);
    lineRer.SetPosition(0, Vector3.zero);
    lineRer.useWorldSpace = false;
    mat = lineRer.materials[0];
    if (mat) {
      sheetCoordinates = new float[TexSheetCount];
      for (var i = 0; i < sheetCoordinates.Length; i++)
        sheetCoordinates[i] = (i + 1f) / TexSheetCount;
      mat.SetTextureScale("_MainTex", new Vector2(TexTilingVector, 1f / TexSheetCount));
      lineRerTarget = new GameObject("lineRerTarget");
      lineRerTarget.transform.parent = transform;
    }

  }


  void Update() {
    LineRenderer updateLineRer = lineRer;
    if (Target) {
      Transform trans = lineRerTarget.transform;
      trans.position = Target.position;
      updateLineRer.SetPosition(1, trans.localPosition + TargetOffset);
      if (TexAutoDistanceDiv) {
        float dist = Vector3.Distance(trans.position, transform.position);
        if (dist > TexDistanceDivValue)
          texAutoDistDiv = (int)(dist / TexDistanceDivValue);
        else
          texAutoDistDiv = 1;
      }
    }

    Material updateMat = mat;
    if (updateMat) {
      //Texture mover
      if (timerMover >= TexMoverTotalTime) {
        timerMover = 0;
        texOffset.x = 0;
      }
      else
        texOffset.x += texMoverVector * Time.deltaTime / TexMoverTotalTime;

      //Texture sheet animation 
      if (timerSheet >= TexSheetTotalTime / TexSheetCount) {
        timerSheet = 0;

        // Sheet animation order or random
        if (!TexSheetRandom) {
          if (sheetIndex == sheetCoordinates.Length)
            sheetIndex = 0;
          texOffset.y = sheetCoordinates[sheetIndex];
          sheetIndex++;
        }
        else
          texOffset.y = sheetCoordinates[Random.Range(0, sheetCoordinates.Length)];
        updateMat.SetTextureScale("_MainTex", new Vector2(TexTilingVector * texAutoDistDiv, 1f / TexSheetCount));
      }
      else
        timerSheet += Time.deltaTime;

      updateMat.SetTextureOffset("_MainTex", texOffset);
    }


  }


}

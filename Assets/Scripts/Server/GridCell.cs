using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    private void Awake() => RoundSystem.AddGridPoint(transform);

    private void OnDestroy() => RoundSystem.RemoveGridPoint(transform);
}
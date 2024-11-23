using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerDetector
{
    void Initialize(LayerMask playerLayer, LayerMask hideLayer, float frontLength, float backLength);
    bool DetectPlayer(Vector2 position, Vector2 direction, float frontLength, float backLength);
    Transform GetDetectedPlayer(Vector2 position, Vector2 direction, float frontLength, float backLength);
}
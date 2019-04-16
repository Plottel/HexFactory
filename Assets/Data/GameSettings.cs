using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Laser Destruction")]
    public float DestructionShrinkEndScale;
    public float DestructionShrinkDuration;

    [Header("Player Controls")]
    public float TurnSpeed;
    public InputMethod InputMethod;

    [Header("Spawners")]
    public float SecondsPerSpawn;
    public int MinimumLaserFrequency;

    [Header("Clumps")]
    public int MinClumpSize;
    public int MaxClumpSize;
    public float TimeToReachMagnet;
    public ClumpShape ClumpShape;
    public DG.Tweening.Ease ClumpTweeningType;

    [Header("Pieces")]
    [Tooltip("Sprites 0-4 are basic pieces. 5 is the laser. 6 is the magnet.")]
    public Sprite[] PieceSprites;
}

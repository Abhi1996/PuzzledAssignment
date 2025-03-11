
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PuzzleManager : MonoBehaviour
{
    public List<GameObject> puzzlePieces = new List<GameObject>();
    public GameObject safeZoneForUnlockedPieces;
    public static bool IsPuzzleComplete = false;
    public static int TotalLockedPuzzlePieces = 0;
    [SerializeField] private GameObject completePuzzleVFX;
    
    private float _timer;
    
    void Start()
    {
        LoadPuzzlePieces();
    }

    private void LoadPuzzlePieces()
    {
        foreach (var puzzlePiece in puzzlePieces)
        {
            var randomPosition = new Vector3(Random.Range(safeZoneForUnlockedPieces.transform.localPosition.x - 200f, safeZoneForUnlockedPieces.transform.localPosition.x + 200.0f), 
                Random.Range(safeZoneForUnlockedPieces.transform.localPosition.y - 200f, safeZoneForUnlockedPieces.transform.localPosition.y + 200.0f), 0);

            Vector3 savedPosition = LoadPuzzlePiecesFromPlayerPref(puzzlePiece.name);
            puzzlePiece.transform.localPosition = savedPosition == Vector3.zero ? randomPosition : savedPosition;
            SavePuzzlePiecesInPlayerPref(puzzlePiece.name, puzzlePiece.transform.localPosition);
        }
    }

    public static void SavePuzzlePiecesInPlayerPref(string pieceName, Vector3 localPosition)
    {
        PlayerPrefs.SetFloat(pieceName+"_x", localPosition.x);
        PlayerPrefs.SetFloat(pieceName+"_y", localPosition.y);
        PlayerPrefs.Save();
    }
    
    public Vector3 LoadPuzzlePiecesFromPlayerPref(string pieceName)
    {
        float posX = PlayerPrefs.GetFloat(pieceName + "_x", 0f);
        float posY = PlayerPrefs.GetFloat(pieceName + "_y", 0f);
        return new Vector3(posX, posY, 0);
    }
    
    void Update()
    {
        if (!IsPuzzleComplete && CheckIfPuzzleIsComplete())
        {
            MarkAsComplete();
        }
    }

    private bool CheckIfPuzzleIsComplete()
    {
        int totalLocked = PlayerPrefs.GetInt("TotalLockedPuzzlePieces", 0);
        if (puzzlePieces.Count == totalLocked) 
            return true;

        return false;
    }

    private void MarkAsComplete()
    {
        Image imgComponent = this.GetComponent<Image>();
        var color = imgComponent.color;
        color.a = 1;
        imgComponent.color = color;
        foreach (var puzzlePiece in puzzlePieces)
        {
            puzzlePiece.SetActive(false);
        }
        completePuzzleVFX.SetActive(true);
        IsPuzzleComplete = true;
    }
    
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using DG.Tweening;
//using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Helper : MonoBehaviour
{
    public Transform carryPos;
    public Vector3 spawnPos;
    public Animator animator;
    public float holeOffset = 2;
    public Ease helperEase;
    public float helperRotateTime = 0.5f;
    public List<Part> currentPieces;
    private Vector3 holePos;


    public void Awake()
    {
        spawnPos = transform.position;
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        transform.rotation = StartRot();
        holePos = new Vector3(GameManager.Instance.hole.transform.position.x, transform.position.y, GameManager.Instance.hole.transform.position.z);
    }

    Quaternion StartRot()
    {
        Vector3 dir = new Vector3(GameManager.Instance.target.transform.position.x, transform.position.y, GameManager.Instance.target.transform.position.z) - transform.position;
        return Quaternion.LookRotation(dir);
    }


    public float PieceDistance(Part piece)
    {
        var distance = Vector3.Distance(transform.position, piece.transform.position);
        return distance;
    }
    
    public virtual IEnumerator MoveTowardsPiece(Part piece)
    {
        StopCoroutine("ToStartPos");
        transform.DOKill();

        AddPiece();

        Vector3 piecePos = new Vector3(piece.transform.position.x, transform.position.y, piece.transform.position.z);
        RotateHelper(piecePos);
        
        yield return StartCoroutine("ToPiece", piecePos);
        CollectPiece(piece);
        RotateHelper(holePos);
        
        yield return StartCoroutine("ToHole", holePos);
        LeavePiece(piece, true);
        SetHelperFree();
        StartCoroutine("ToStartPos");
    }

    void AddPiece()
    {
        int collectCount = 0;
        piece.occupied = true;
        currentPieces.Add(piece);
    }
    
    public void ManageLists()
    {
        if (GameManager.Instance.helpers.Count == 0) return;
        GameManager.Instance.helpers.Remove(this);
        GameManager.Instance.activeHelpers.Add(this);
    }
    
    private List<Part> newOrder;
    public Part ClosestPiece()
    {
        if (GameManager.Instance.grounded.Count == 0) return null;

        List<Part> temp = GameManager.Instance.grounded.Where(k=>k!=null).Where(j=>j.occupied==false).ToList();
        
        if (temp.Count == 0)
            return null;
        
        Part closestPiece =  temp.OrderBy(i => Vector3.Distance(i.transform.position, transform.position)).First();
        temp.Clear();
        return closestPiece;
    }
    


    IEnumerator ToStartPos()
    {
        GameManager.Instance.upgradeSystem.UpdateHelpersPos(GameManager.Instance.helpers.Count);
        animator.Play("Walk");
        transform.DORotateQuaternion(Quaternion.LookRotation(spawnPos-transform.position), helperRotateTime);
        yield return transform.DOMove(spawnPos, GameManager.Instance.upgradeSystem.stats[3].currentValue).SetSpeedBased().SetEase(helperEase).WaitForCompletion();
        yield return transform.DORotateQuaternion(StartRot(), helperRotateTime).WaitForCompletion();
        animator.Play("Idle");
    }

    IEnumerator ToPiece(Vector3 piecePos)
    {
        yield return transform.DOMove(piecePos,
            GameManager.Instance.upgradeSystem.stats[3].currentValue).SetSpeedBased().SetEase(helperEase).WaitForCompletion();
    }
    
    IEnumerator ToHole(Vector3 holePos)
    {
        Vector3 dir = (holePos - transform.position).normalized;
        yield return transform.DOMove(holePos-dir*holeOffset, 
            GameManager.Instance.upgradeSystem.stats[3].currentValue).SetSpeedBased().SetEase(helperEase).WaitForCompletion();
        animator.Play("Idle");
    }

    public void RotateHelper(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.DORotateQuaternion(lookRot, helperRotateTime);
    }
    

    public void CollectPiece(Part piece)
    {

        if (piece == null) return;
        GameManager.Instance.grounded.Remove(piece);
        piece.Disable();
        piece.transform.SetParent(carryPos);
        PiecePos(piece);
    }

    public virtual void PiecePos(Part piece)
    {
        piece.transform.DOLocalMove(Vector3.zero, 0.3f); 
    }
    

    public virtual void LeavePiece(Part piece, bool toHole)
    {
        if (piece != null)
        {
            if (toHole)
            {
                currentPieces.Clear();
                piece.StartCoroutine("PartToHole", 1);
            }
                
            else
                piece.SetPartFree();
        }
    }
    

    public void SetHelperFree()
    {
        GameManager.Instance.activeHelpers.Remove(this);
        GameManager.Instance.helpers.Add(this);
    }

    

    public int GetHelperId()
    {
        int counter = -1;
        for (int i = 0; i < GameManager.Instance.helpers.Count; i++)
        {
            if (GameManager.Instance.helpers[i] == this)
            {
                counter++;
                break;
            }
        }
        return counter;
    }

    public void KillAllTweens()
    {
        DOTween.KillAll();
        StopCoroutine("MoveTowardsPiece");
    }

    public void StopHelper()
    {
        GameManager.Instance.player.StopAllCoroutines();
        GameManager.Instance.upgradeSystem.UpdateHelpersPos(GameManager.Instance.helpers.Count);
        StopAllCoroutines();
        StartCoroutine("ToStartPos");
    }
}
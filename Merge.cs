using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityExtensions;

public class Merge : MonoBehaviour
{
    public List<Helper> helpers = new();
    public List<Helper> activeHelpers = new();

    public void FillMergeList(string helpersToMerge, List<Helper> mergeList, GameObject helperType, int quota)
    {
        helpers.Where(i=>i.CompareTag(helpersToMerge)).ForEach(j=>mergeList.Add(j));
        activeHelpers.Where(i=>i.CompareTag(helpersToMerge)).Where(j=>j != null).ForEach(j=>mergeList.Add(j)); //Where(j=>j != null)
        if (mergeList.Count < quota)
        {
            mergeList.Clear();
            return;
        }

        for (int i = 0; i < quota; i++)
        {
            if (activeHelpers.Contains(mergeList[i])) 
            {
                if(mergeList[i] != null)
                    mergeList[i].LeavePiece(mergeList[i].currentPieces[0], false);
            }
        }
        
        Evolve(mergeList, helperType, quota);

    }

    void Evolve(List<Helper> mergeList, GameObject helperType, int quota)
    {
        Vector3 spawnPos = MergePos(mergeList, quota);
      
        
        for (int i = quota - 1; i >= 0; i--)
        {
            mergeList[i].StopAllCoroutines();
            mergeList[i].transform.DOKill();
            helpers.Remove(mergeList[i]);
            activeHelpers.Remove(mergeList[i]); //new
            Destroy(mergeList[i].gameObject);
        }
        
        mergeList.Clear();
        
        GameObject evolved = Instantiate(helperType,spawnPos, Quaternion.identity);
        helpers.Add(evolved.GetComponent<Helper>());
    }
    
    Vector3 MergePos(List<Helper> mergeList, int quota)
    {
        Vector3 spawnPos = Vector3.zero;
        for (int i = 0; i < quota; i++)
        {
            spawnPos += mergeList[i].transform.position;
        }

        spawnPos /= 3;
        return spawnPos;
    }
    
    public List<Helper> minMergeList = new();
    public List<Helper> maxMergeList = new();
    public GameObject evolvedHelper;
    public GameObject megaHelper;
    public int minMergeQuota = 3;
    public int maxMergeQuota = 2;
    
    public void MergeButton()
    {
        FillMergeList("Untagged", minMergeList, evolvedHelper, minMergeQuota);
        FillMergeList("Evolved", maxMergeList, megaHelper, maxMergeQuota);
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "New Artifact", menuName = "Inventory/Artifact")]
public class ArtifactItem : Item
{
    [Header("Artifact Settings")]
    public int artifactID;
    public bool isQuestItem = true;

    void OnEnable()
    {
        itemType = ItemType.Artifact;
    }
}
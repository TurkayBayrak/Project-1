using System;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    public bool IsMarked { get; set; }

    [SerializeField] private Material defaultMaterial, materialWithXIcon;
    private MeshRenderer meshRenderer;

    public AdjacentGridNodes adjacentGridNodes;
    public List<GridNode> AdjacentGridNodeList { get; } = new List<GridNode>();

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        SetAdjacentGridNodeList();
    }

    public void SetGridNodeMaterial()
    {
        meshRenderer.material = IsMarked ?  materialWithXIcon : defaultMaterial;
    }
    
    private void SetAdjacentGridNodeList()
    {
        AdjacentGridNodeList.Add(adjacentGridNodes.upGrid);
        AdjacentGridNodeList.Add(adjacentGridNodes.rightGrid);
        AdjacentGridNodeList.Add(adjacentGridNodes.downGrid);
        AdjacentGridNodeList.Add(adjacentGridNodes.leftGrid);
    }
}

[Serializable]
public class AdjacentGridNodes
{
    public GridNode upGrid;
    public GridNode rightGrid;
    public GridNode downGrid;
    public GridNode leftGrid;
}


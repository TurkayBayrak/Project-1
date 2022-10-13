using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    public bool isMarked;

    public Material defaultMaterial, materialWithXIcon;
    private MeshRenderer meshRenderer;

    public AdjacentGridNodes adjacentGridNodes;
    public List<GridNode> adjacentGridNodeList = new List<GridNode>();
    private GridManager GridManager;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        GridManager = GridManager.Instance;
        SetAdjacentGridNodeList();
    }

    public void MarkTheGrid()
    {
        meshRenderer.material = isMarked ? defaultMaterial : materialWithXIcon;
        isMarked = !isMarked;
    }
    
    private void SetAdjacentGridNodeList()
    {
        adjacentGridNodeList.Add(adjacentGridNodes.upGrid);
        adjacentGridNodeList.Add(adjacentGridNodes.rightGrid);
        adjacentGridNodeList.Add(adjacentGridNodes.downGrid);
        adjacentGridNodeList.Add(adjacentGridNodes.leftGrid);
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


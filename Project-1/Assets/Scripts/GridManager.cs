using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    
    [SerializeField] private TMP_InputField gridSizeInputFieldText;
    [SerializeField] private Button rebuildButton;
    [SerializeField] private TextMeshProUGUI matchCountText;

    public List<GridNode> matchedGridNodeList = new List<GridNode>();
    public List<GridNode> markedGridNodeList = new List<GridNode>();

    private int gridMapSize;
    private int matchCount;
    private bool isCounted;

    private Camera cam;
    
    private void Awake()
    {
        if (!Instance)
            Instance = this;
            
        cam = Camera.main;
    }

    private void Start()
    {
        rebuildButton.onClick.AddListener(CreateGrids);
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var raycastHit)) return;
        var hitObject = raycastHit.transform.gameObject;

        if (!hitObject.TryGetComponent(out GridNode gridNode)) return;

        gridNode.MarkTheGrid();
        
        if (gridNode.isMarked)
        {
            markedGridNodeList.Add(gridNode);
            CheckMatchCondition();
        }
        else
            markedGridNodeList.Remove(gridNode);
        
        if (isCounted)
            isCounted = false;
    }

    private void CheckMatchCondition()
    {
        foreach (var gridNode in markedGridNodeList)
        {
            var markedCount = 0;
            foreach (var adjacentGridNode in gridNode.adjacentGridNodeList)
            {
                if (!adjacentGridNode) continue;
                if (!adjacentGridNode.isMarked) continue;
                markedCount++;
                if (markedCount > 1)
                    SetListsBeforeMatchAction(gridNode);
            }
        }
    }
    
    private void SetListsBeforeMatchAction(GridNode gridNode)
    {
        if (!matchedGridNodeList.Contains(gridNode))
            matchedGridNodeList.Add(gridNode);

        foreach (var adjacentGridNode in gridNode.adjacentGridNodeList)
        {
            if (adjacentGridNode && adjacentGridNode.isMarked && !matchedGridNodeList.Contains(adjacentGridNode))
                matchedGridNodeList.Add(adjacentGridNode);
        }
        StartCoroutine(WaitForGridNodeCalculations());
    }

    private IEnumerator WaitForGridNodeCalculations()
    {
        yield return null;
        DealWithGridsOnMatch();
    }

    private void DealWithGridsOnMatch()
    {
        foreach (var gridNode in matchedGridNodeList)
        {
            gridNode.MarkTheGrid();
            markedGridNodeList.Remove(gridNode);
        }
        matchedGridNodeList.Clear();
        
        if (isCounted) return;
        matchCount++;
        matchCountText.text = matchCount.ToString();
        isCounted = true;
    }

    private void CreateGrids()
    {
        matchCount = 0;
        matchCountText.text = matchCount.ToString();
        markedGridNodeList.Clear();
        matchedGridNodeList.Clear();
        
        for (var i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        StartCoroutine(WaitForDestroyOldGrids());
    }

    private IEnumerator WaitForDestroyOldGrids()
    {
        yield return new WaitUntil(() => transform.childCount == 0);
        gridMapSize = int.Parse(gridSizeInputFieldText.text);
        
        var totalGridNodeCount = gridMapSize * gridMapSize;

        for (var i = 0; i < totalGridNodeCount; i++)
        {
            Instantiate(Resources.Load<GameObject>("Grid"), transform, true);
        }
        SetAdjacentGridNodes();
        CreateGridMap();
    }

    private void SetAdjacentGridNodes()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            if (i + gridMapSize < transform.childCount)
                transform.GetChild(i).GetComponent<GridNode>().adjacentGridNodes.rightGrid =
                    transform.GetChild(i + gridMapSize).GetComponent<GridNode>();

            if ((i + 1) % gridMapSize != 0)
                transform.GetChild(i).GetComponent<GridNode>().adjacentGridNodes.downGrid =
                    transform.GetChild(i + 1).GetComponent<GridNode>();
            
            if (i - gridMapSize >= 0)
                transform.GetChild(i).GetComponent<GridNode>().adjacentGridNodes.leftGrid =
                    transform.GetChild(i - gridMapSize).GetComponent<GridNode>();

            if (i % gridMapSize != 0)
                transform.GetChild(i).GetComponent<GridNode>().adjacentGridNodes.upGrid =
                    transform.GetChild(i - 1).GetComponent<GridNode>();
        }
    }
    
    private void CreateGridMap()
    {
        var a = 0f;
        var count = 0;
        
        for (var x = 0; x < gridMapSize; x++, a += 1.1f)
        {
            var b = 0f;
            for (var y = 0; y < gridMapSize; y++ , b -= 1.1f)
            {
                var grid = transform.GetChild(count).GetComponent<GridNode>();
                
                grid.transform.position = new Vector3( a, b + 3, 0);
                count++;
            }
        }
    }
}
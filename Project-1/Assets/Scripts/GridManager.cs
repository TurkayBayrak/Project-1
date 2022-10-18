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

    private const string matchCountString = "Match Count: ";

    private readonly List<GridNode> matchedGridNodeList = new List<GridNode>();
    private readonly List<GridNode> markedGridNodeList = new List<GridNode>();

    public int GridMapSize { get; private set; }
    private int matchCount;
    private bool isCounted;
    private bool isMatch;
    private GridNode currentNode;

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

        currentNode = gridNode;
        gridNode.IsMarked = !gridNode.IsMarked;
        
        if (gridNode.IsMarked)
        {
            markedGridNodeList.Add(gridNode);
            CheckMatchCondition();
        }
        else
        {
            markedGridNodeList.Remove(gridNode);
            AudioManager.instance.PlaySound(Sounds.UnmarkSound);
            gridNode.SetGridNodeMaterial();
        }
        
        if (isCounted)
            isCounted = false;
    }

    private void CheckMatchCondition()
    {
        foreach (var gridNode in markedGridNodeList)
        {
            var markedCount = 0;
            foreach (var adjacentGridNode in gridNode.AdjacentGridNodeList)
            {
                if (!adjacentGridNode) continue;
                if (!adjacentGridNode.IsMarked) continue;
                markedCount++;
                if (markedCount <= 1) continue;
                SetListsBeforeMatchAction(gridNode);
                isMatch = true;
            }
        }

        if (isMatch)
        {
            AudioManager.instance.PlaySound(Sounds.MatchSound);
            isMatch = false;
        }
        else
        {
            AudioManager.instance.PlaySound(Sounds.MarkSound);
            currentNode.SetGridNodeMaterial();
        }
    }
    
    private void SetListsBeforeMatchAction(GridNode gridNode)
    {
        if (!matchedGridNodeList.Contains(gridNode))
            matchedGridNodeList.Add(gridNode);

        foreach (var adjacentGridNode in gridNode.AdjacentGridNodeList)
        {
            if (adjacentGridNode && adjacentGridNode.IsMarked && !matchedGridNodeList.Contains(adjacentGridNode))
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
            gridNode.IsMarked = !gridNode.IsMarked;
            gridNode.SetGridNodeMaterial();
            markedGridNodeList.Remove(gridNode);
        }
        matchedGridNodeList.Clear();
        
        if (isCounted) return;
        matchCount++;
        matchCountText.text = matchCountString + matchCount;
        isCounted = true;
    }

    private void CreateGrids()
    {
        matchCount = 0;
        matchCountText.text = matchCountString + matchCount;
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
        
        if (int.TryParse(gridSizeInputFieldText.text, out var size))
            GridMapSize = size;
        else 
            Debug.LogError("Please Enter A Number");
        
        var totalGridNodeCount = GridMapSize * GridMapSize;

        for (var i = 0; i < totalGridNodeCount; i++)
        {
            Instantiate(Resources.Load<GameObject>("GridNode"), transform, true);
        }
        SetAdjacentGridNodes();
        CreateGridMap();
    }

    private void SetAdjacentGridNodes()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            if (i + GridMapSize < transform.childCount)
                transform.GetChild(i).GetComponent<GridNode>().adjacentGridNodes.rightGrid =
                    transform.GetChild(i + GridMapSize).GetComponent<GridNode>();

            if ((i + 1) % GridMapSize != 0)
                transform.GetChild(i).GetComponent<GridNode>().adjacentGridNodes.downGrid =
                    transform.GetChild(i + 1).GetComponent<GridNode>();
            
            if (i - GridMapSize >= 0)
                transform.GetChild(i).GetComponent<GridNode>().adjacentGridNodes.leftGrid =
                    transform.GetChild(i - GridMapSize).GetComponent<GridNode>();

            if (i % GridMapSize != 0)
                transform.GetChild(i).GetComponent<GridNode>().adjacentGridNodes.upGrid =
                    transform.GetChild(i - 1).GetComponent<GridNode>();
        }
    }
    
    private void CreateGridMap()
    {
        var a = 0f;
        var count = 0;
        
        for (var x = 0; x < GridMapSize; x++, a += 1)
        {
            var b = 0f;
            for (var y = 0; y < GridMapSize; y++ , b -= 1)
            {
                var grid = transform.GetChild(count).GetComponent<GridNode>();
                
                grid.transform.position = new Vector3( a - GridMapSize / 2f + 0.5f, b + GridMapSize / 2f - 0.5f, 0);
                count++;
            }
        }
        CameraController.instance.SetCameraPosition();
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node : MonoBehaviour
{

    static List<Node> nodes = new List<Node>();
    static float radius = 0.7f;

    public bool isFarm;
    public Node farmReference;
    public List<Node> adjList = new List<Node>();

    Transform tf;
    bool isPlacing = true;
    SpriteRenderer sr;

    void Start()
    {
        tf = transform;
        sr = GetComponent<SpriteRenderer>();
        sr.color = isFarm ? new Color(0.3f + Random.value * 0.7f, 0.3f + Random.value * 0.7f, 0.3f + Random.value * 0.7f) : Color.white;
    }

    void Update()
    {
        if (isPlacing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isPlacing = false;
                if (isFarm) SetFarm();
                else SetField();
            }
            else
            {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                tf.position = Vector2.Lerp(transform.position, mousePosition, 0.25f);
            }
        }
        else
        {
            for (int i = 0, len = adjList.Count; i < len; ++i)
                Debug.DrawLine(tf.position, adjList[i].tf.position, ReferenceEquals(farmReference, null) ? Color.magenta : farmReference.sr.color);
        }
    }

    private void OnMouseDown()
    {
        if (Input.GetKey(KeyCode.D)){
            DeleteNode();
        }
    }

    void SetFarm()
    {
        //Adiciona todos os Nodes proximos na lista de adjacentes.
        for (int i = 0, len = nodes.Count; i < len; ++i)
            if (Vector2.Distance(nodes[i].tf.position, tf.position) < 3 * radius)
            {
                adjList.Add(nodes[i]);
                nodes[i].adjList.Add(this);
            }

        //Adiciona este Farm na lista de Nodes.
        nodes.Add(this);

        //Propaga esta farm para todos os Nodes sem farmReference.
        for (int i = 0, len = adjList.Count; i < len; ++i)
            if (adjList[i].farmReference == null)
                PropagateFarm(adjList[i], this);
    }

    void SetField()
    {
        //Adiciona todos os Nodes proximos na lista de adjacentes.
        for (int i = 0, len = nodes.Count; i < len; ++i)
            if (Vector2.Distance(nodes[i].tf.position, tf.position) < 3 * radius)
            {
                adjList.Add(nodes[i]);
                nodes[i].adjList.Add(this);
            }

        //Adiciona este Farm na lista de Nodes.
        nodes.Add(this);

        //Se algum adjacente tiver farmRefence, seta para ele tambem.
        for (int i = 0, len = adjList.Count; i < len; ++i)
            if (adjList[i].farmReference != null)
            {
                farmReference = adjList[i].farmReference;
                break;
            }

        //Se algum Node adjacente nao tem farmReference e o Node
        //atual tem, propaga a Farm para os adjacentes.
        if (farmReference != null)
            for (int i = 0, len = adjList.Count; i < len; ++i)
                if (adjList[i].farmReference == null)
                    PropagateFarm(adjList[i], farmReference);
    }

    void PropagateFarm(Node node, Node farmRef)
    {
        if (!node.isFarm) node.farmReference = farmRef;
        for (int i = 0, len = node.adjList.Count; i < len; ++i)
            if (ReferenceEquals(node.adjList[i].farmReference, null))
                PropagateFarm(node.adjList[i], node.isFarm ? node : farmRef);
    }

    void DeleteNode()
    {
        //Remove o no da lista de nos.
        nodes.RemoveAt(nodes.IndexOf(nodes.Where(p => ReferenceEquals(p, this)).FirstOrDefault()));

        //Removendo conexoes dos vizinhos para este no.
        for (int i = 0, len = adjList.Count; i < len; ++i)
            adjList[i].adjList.RemoveAt(adjList[i].adjList.IndexOf(adjList[i].adjList.Where(p => ReferenceEquals(p, this)).FirstOrDefault()));

        List<Node> farms = new List<Node>();

        //Limpa as farmReferences dos Nodes.
        for(int i = 0, len = nodes.Count; i < len; ++i)
        {
            if (nodes[i].isFarm) farms.Add(nodes[i]);
            else nodes[i].farmReference = null;
        }

        //Propaga todas as Farms novamente.
        for (int i = 0, len = farms.Count; i < len; ++i)
            PropagateFarm(farms[i], farms[i]);
        
        Destroy(gameObject);
    }
        
}

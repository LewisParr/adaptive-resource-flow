using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem04Controller : MonoBehaviour
{
	public int numNodes = 20;

	private List<Node> node;

	void OnEnable()
	{

	}

	private void GenerateNodes()
	{
		float x = Random.value * numNodes;
		float y = Random.value * numNodes;

		Vector3 pos = new Vector3(x, 0, y);

		node.Add(new Node(pos));
	}
}

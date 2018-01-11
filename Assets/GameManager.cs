using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private Stopwatch timer = new Stopwatch ();
	private int score;
	public Text scoreText;
	public Text timerText;

	public GameObject blockPrefab;
	public int width;
	public int height;

	public int maxNumber = 5;

	public int maxDragendSize = 5;

	List<NumberBlock> blocks;
	List<NumberBlock> dragendBlocks = new List<NumberBlock> ();

	System.Random rand;

	public bool allowInvalidPath;

	private static System.Func<NumberBlock, NumberBlock, bool> predicatePosition = (b1, b2) => {
		return Mathf.Abs (b1.row - b2.row) + Mathf.Abs (b1.column - b2.column) == 1;
	};

	private static System.Func<NumberBlock, NumberBlock, bool> predicateValue = (b1, b2) => {
		// return Mathf.Abs (b1.value - b2.value) == 1;
		return b1.value == b2.value;
	};

	private static System.Func<NumberBlock, NumberBlock, bool> predicate = (b1, b2) => {
		return predicatePosition (b1, b2) && predicateValue (b1, b2);
	};

	public bool hints;
	void Start () {
		timer.Start ();
		rand = new System.Random ();
		blocks = new List<NumberBlock> ();
		for (int i = -width / 2; i < width / 2; i++) {
			for (int j = -height / 2; j < height / 2; j++) {
				createBlock (i, j);
			}
		}
		score = 0;
		addScore (new List<NumberBlock> ());
	}

	private void createBlock (int row, int column) {
		GameObject go = GameObject.Instantiate (blockPrefab);
		NumberBlock numberBlock = go.GetComponent<NumberBlock> ();
		numberBlock.Init (this, row, column, rand.Next (maxNumber) + 1);
		blocks.Add (numberBlock);
	}

	public void NotifyExplosion (List<NumberBlock> explodedBlocks) {
		foreach (NumberBlock exploded in explodedBlocks) {
			blocks.FindAll (b => b.row > exploded.row && b.column == exploded.column)
				.ForEach (b => b.SetPosition (b.row - 1, b.column));
			blocks.Remove (exploded);
			createBlock ((height / 2) - 1, exploded.column);
			Destroy (exploded.gameObject);
		};
		addScore (explodedBlocks);
	}

	void Update () {
		if (Input.GetMouseButtonUp (0)) {
			if (!allowInvalidPath || pathIsValid ()) {
				NotifyExplosion (dragendBlocks);
			}
			dragendBlocks = new List<NumberBlock> ();
			UpdateBlockShadows ();
		}
		timerText.text = timer.Elapsed.ToString ();
	}
	private bool pathIsValid () {
		for (var i = 0; i < dragendBlocks.Count - 2; i++) {
			if (!predicate (dragendBlocks[i], dragendBlocks[i + 1])) {
				return false;
			}
		}
		return true;
	}

	public void DragStart (NumberBlock block) {
		dragendBlocks = new List<NumberBlock> ();
		//if (block.value == 1) {
		dragendBlocks.Add (block);
		UpdateBlockShadows ();
		//}
	}

	public void DragendOver (NumberBlock block) {
		if (dragendBlocks.Count == 0) {
			return;
		}
		NumberBlock lastBlock = dragendBlocks[dragendBlocks.Count - 1];
		if (block.Equals (lastBlock)) {
			return;
		}
		if (!predicatePosition (block, dragendBlocks[dragendBlocks.Count - 1])) {
			return;
		}
		if (!allowInvalidPath && !predicateValue (block, dragendBlocks[dragendBlocks.Count - 1])) {
			return;
		}
		if (!dragendBlocks.Contains (block)) {
			dragendBlocks.Add (block);
		} else {
			if (dragendBlocks.Count > 1) {
				List<NumberBlock> removedBlocks = dragendBlocks.GetRange (dragendBlocks.IndexOf (block), dragendBlocks.Count - 1);
				removedBlocks.ForEach (b => dragendBlocks.Remove (b));
				removedBlocks.ForEach (b => b.ResetShadow ());
			}
		}
		UpdateBlockShadows ();
	}

	private void UpdateBlockShadows () {
		foreach (var b in blocks) {
			if (dragendBlocks.Contains (b)) {
				b.SetShadowColor (Color.blue);
			} else if (hints && dragendBlocks.Count > 0 && predicate (b, dragendBlocks[dragendBlocks.Count - 1])) {
				b.SetShadowColor (Color.green);
			} else {
				b.ResetShadow ();
			}
		}
	}

	private void addScore (List<NumberBlock> explodedBlocks) {
		score += (int) Mathf.Pow (2, explodedBlocks.Count);
		scoreText.text = "Score: " + score.ToString ();
	}
}
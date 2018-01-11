using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberBlock : MonoBehaviour {

	public int row;
	public int column;
	public int value;
	public float spacing = 2;

	public Text numberText;
	// Use this for initialization
	GameManager manager;

	public Renderer shadow;
	public Renderer main;

	void Start () {

	}

	public void Init (GameManager manager, int row, int column, int value) {
		this.manager = manager;
		this.value = value;
		this.numberText.text = value.ToString ();
		transform.position = calculatePosition (row + 1, column);
		SetPosition (row, column);
		this.main.material.color = Color.HSVToRGB ((float) value / 3, 1, 1);
	}

	public void SetPosition (int row, int column) {
		this.row = row;
		this.column = column;
		this.name = "Block" + row + column;
	}

	// Update is called once per frame
	void FixedUpdate () {
		transform.position = Vector3.Lerp (transform.position, calculatePosition (row, column), Time.deltaTime * 3);
	}

	private Vector3 calculatePosition (int row, int column) {
		return new Vector3 (column, row, 0) * spacing;
	}

	void OnMouseDown () {
		manager.DragStart (this);
		//manager.NotifyExplosion (new List<NumberBlock> () { this });
		//Destroy (this.gameObject);
	}

	void OnMouseOver () {
		if (Input.GetMouseButton (0)) {
			manager.DragendOver (this);
		}
	}
	public override int GetHashCode () {
		return this.row + 10000 + this.column;
	}

	public void SetShadowColor (Color color) {
		shadow.enabled = true;
		shadow.material.color = color;
	}

	public void ResetShadow () {
		shadow.enabled = false;
	}

}
using UnityEngine;
using System.Collections;

public class RuneManager : MonoBehaviour {
	public static RuneManager Instance;
	public static void Play() {
		Instance.gameObject.SetActiveRecursively(true);
	}
	
	public float boundingBoxWidth;
	public float boundingBoxHeight;
	public float distanceFromTop;
	public float boundingBoxPadding;
	public float runeEntryWidth;
	public GameObject[] runePrefabs;
	public Rune[] runes;
	
	#region MonoBehaviour
	void Awake() {
		Instance = this;
	}
	
	void Start() {
		runes = new Rune[runePrefabs.Length];
		for (int i = 0; i < runePrefabs.Length; i++) {
			if (runePrefabs[i] != null) {
				GameObject rune = (GameObject)Instantiate(runePrefabs[i]);
				runes[i] = rune.GetComponent<Rune>();
			}
		}
	}
	
	void OnEnable() {
		foreach (Rune rune in runes) {
			if (rune != null) {
				rune.Apply();
			}
		}
	}
	
	void OnDisable() {
		foreach (Rune rune in runes) {
			if (rune != null) {
				rune.Unapply();
			}
		}
	}
	
	void OnGUI() {
		GUI.Box(new Rect(Screen.width/2 - boundingBoxWidth/2,
						distanceFromTop,
						boundingBoxWidth,
						boundingBoxHeight),
				"");
		DrawRuneSlots();
	}
	#endregion
	
	void DrawRuneSlots() {
		float slotsStartPosition = Screen.width/2 - boundingBoxWidth/2;
		for (int i = 0; i < runes.Length; i++) {
			Rune rune = runes[i];
			string name = rune != null && rune.Name.Length > 0 ? rune.Name : "";
			GUI.Box(new Rect(slotsStartPosition + (runeEntryWidth + boundingBoxPadding) * i + boundingBoxPadding,
							distanceFromTop + boundingBoxPadding,
							runeEntryWidth,
							boundingBoxHeight - boundingBoxPadding*2),
					name);
		}
	}
}

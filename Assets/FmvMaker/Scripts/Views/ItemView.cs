using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemView : MonoBehaviour {

    [SerializeField]
    private Button _itemButton;
    [SerializeField]
    private Text _itemText;

    private RectTransform _rectTransform;

    private void Awake() {
        _itemButton = GetComponent<Button>();
        _itemText = GetComponentInChildren<Text>();
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start() {

    }

    private void Update() {

    }
}

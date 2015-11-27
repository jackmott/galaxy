using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SLS.Widgets.Table {
public class InputCell : MonoBehaviour {

  public Table table { private set; get; }
  public InputField inputField { private set; get; }
  public RectTransform rectTransform { private set; get; }

  public bool initialize(Table table, RectTransform rt, InputField inputField) {
    this.table = table;
    this.rectTransform = rt;
    this.inputField = inputField;
    return true;
  }

  private Cell cell;

  void Start() {
    if (this.inputField != null) {
      this.inputField.onEndEdit.AddListener(this.onEndEdit);
    }
  }

  void OnEnable() {
    if (this.inputField != null) {
      this.inputField.onEndEdit.AddListener(this.onEndEdit);
    }
  }

  void OnDisable() {
    if (this.inputField != null) {
      this.inputField.onEndEdit.RemoveListener(this.onEndEdit);
    }
  }

  private void onEndEdit(string s) {
    this.removeFocus();
  }



  public void setFocus(Cell c) {

    this.removeFocus(true);

    this.cell = c;

    this.rectTransform.SetParent(this.cell.transform.parent);
    this.rectTransform.pivot = this.cell.rt.pivot;
    this.rectTransform.anchorMin = this.cell.rt.anchorMin;
    this.rectTransform.anchorMax = this.cell.rt.anchorMax;
    this.rectTransform.offsetMin = this.cell.rt.offsetMin;
    this.rectTransform.offsetMax = this.cell.rt.offsetMax;
    this.inputField.image.color = this.table.cellSelectColor;
    this.inputField.textComponent.color = this.table.rowTextColor;
    this.inputField.text = this.cell.text.text;
    this.inputField.textComponent.font = this.cell.text.font;
    this.inputField.textComponent.fontStyle = this.cell.text.fontStyle;
    this.inputField.textComponent.fontSize = this.cell.text.fontSize;

    this.inputField.textComponent.rectTransform.localPosition = new Vector2(3,-3);
    this.inputField.textComponent.rectTransform.sizeDelta =
        new Vector2(this.table.inputCell.rectTransform.sizeDelta.x - 6,
                    this.table.inputCell.rectTransform.sizeDelta.y - 6);

    this.cell.text.gameObject.SetActive(false);
    this.inputField.interactable = true;
    this.gameObject.SetActive(true);
    this.inputField.Select();

  }

  public void removeFocus(bool withRefocus=false) {
    if (this.cell == null) return;
    if (this.cell.element.value != this.inputField.text) {
      this.cell.column.inputChangeCallback(this.cell.row.datum, this.cell.column,
        this.cell.element.value, this.inputField.text);
      this.cell.element.value = this.inputField.text;
    }
    this.cell.text.gameObject.SetActive(true);
    if (!withRefocus) {
      StartCoroutine(this.deactivateLater());
    }
    this.cell = null;
  }

  IEnumerator deactivateLater() {
    this.inputField.interactable = false;
    yield return new WaitForEndOfFrame();
    this.gameObject.SetActive(false);
  }

}
}
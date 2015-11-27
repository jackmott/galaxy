using UnityEngine;
using UnityEngine.EventSystems;

namespace SLS.Widgets.Table {

public class ScrollWatcher : MonoBehaviour,
  IBeginDragHandler, IEndDragHandler  {

  public Table table { private set; get; }

  private bool _isDragging;
  public bool isDragging {
    get {
      return this._isDragging;
    }
  }

  void Start() {
    if (this.table != null) {
      this.table.bodyScroller.verticalScrollbar.onValueChanged.AddListener(this.onScrollerValueChanged);
      this.table.bodyScroller.horizontalScrollbar.onValueChanged.AddListener(this.onScrollerValueChanged);
    }
  }

  void OnEnable() {
    if (this.table != null) {
      this.table.bodyScroller.verticalScrollbar.onValueChanged.AddListener(this.onScrollerValueChanged);
      this.table.bodyScroller.horizontalScrollbar.onValueChanged.AddListener(this.onScrollerValueChanged);
    }
  }

  void OnDisable() {
    if (this.table != null) {
      this.table.bodyScroller.verticalScrollbar.onValueChanged.RemoveListener(this.onScrollerValueChanged);
      this.table.bodyScroller.horizontalScrollbar.onValueChanged.RemoveListener(this.onScrollerValueChanged);
    }
  }

	public void OnBeginDrag(PointerEventData data) {
    this._isDragging = true;
    if (this.table.inputCell != null)
		  this.table.inputCell.removeFocus();
  }

  public void OnEndDrag(PointerEventData data) {
    this._isDragging = false;
  }

  public bool initialize(Table table) {
    this.table = table;
    return true;
  }

  private void onScrollerValueChanged(float f) {
    if (this.table.inputCell != null)
		  this.table.inputCell.removeFocus();
	}

}
}

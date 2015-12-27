using UnityEngine;
using UnityEngine.UI;

namespace SLS.Widgets.Table{
public class Factory  {

  private bool firstBuild;

  private Control control;

  private Table table;
  public Factory(Table table) {
    this.firstBuild = true;
    this.table = table;
  }

  public void build(Datum headerDatum, Datum footerDatum, Control control) {

    this.control = control;

    if (this.table.columns.Count == 0) {
      this.table.error("Zero columns defined");
    }

    if (this.table.hasError) {
      Debug.LogError("Table Error Exists, aborting display");
      return;
    }

    GameObject go;
    Text text;
    Image img;
    RectTransform rt;
    RectTransform rt2;
    CanvasGroup cg;
    MeasureMaster mm;
    InputField inputField;

    if (this.table.root == null) {
      go = new GameObject();
      go.name = "TablePro";
      this.table.root = go.AddComponent<RectTransform>();
      #if !(UNITY_5_0 || UNITY_5_1)
      if (this.table.use2DMask)
        go.AddComponent<RectMask2D>();
      else {
        img = go.AddComponent<Image>();
        img.sprite = this.table.fillerSprite;
        go.AddComponent<Mask>().showMaskGraphic = false;
      }
      #else
      img = go.AddComponent<Image>();
      img.sprite = this.table.fillerSprite;
      go.AddComponent<Mask>().showMaskGraphic = false;
      #endif
      this.table.root.SetParent(this.table.transform, false);
      this.table.root.anchorMin = new Vector2(0, 0);
      this.table.root.anchorMax = new Vector2(1, 1);
      this.table.root.pivot = new Vector2(0.5f, 0.5f);
      this.table.root.offsetMin = Vector2.zero;
      this.table.root.offsetMax = Vector2.zero;
    }

    /////////////////////////////////////////
    // Make our Header
    /////////////////////////////////////////
    if (this.table.hasHeader) {
      if (this.table.headerRect == null) {
        go = new GameObject();
        this.table.headerRect = go.AddComponent<RectTransform>();
        this.table.headerRect.name = "Header";
        this.table.headerRect.SetParent(this.table.root, false);

        this.table.headerRect.pivot = new Vector2(0, 1);
        this.table.headerRect.anchorMin = new Vector2(0, 1);
        this.table.headerRect.anchorMax = new Vector2(1, 1);

        go = new GameObject();
        rt = go.AddComponent<RectTransform>();
        this.table.headerRow = go.AddComponent<Row>();
        img = go.AddComponent<Image>();
        img.sprite = this.table.fillerSprite;
        img.color = this.table.headerNormalColor;
        this.table.headerRow.name = "HeaderRow";

        go = new GameObject();
        go.name = "CanvasGroup";
        rt2 = go.AddComponent<RectTransform>();
        cg = go.AddComponent<CanvasGroup>();
        rt2.anchorMin = new Vector2(0, 0);
        rt2.anchorMax = new Vector2(1, 1);
        rt2.pivot = new Vector2(0f, 1f);
        rt2.SetParent(rt, false);
        rt2.offsetMin = new Vector2(0, 0);
        rt2.offsetMax = new Vector2(0, 0);

        mm = this.makeMeasureMaster(rt);

        this.table.headerRow.initialize(this.table, rt, rt2, cg, img, mm, true);
        this.table.headerRow.rt.anchorMin = new Vector2(0, 1f);
        this.table.headerRow.rt.anchorMax = new Vector2(1, 1f);
        this.table.headerRow.rt.pivot = new Vector2(0, 1);
        this.table.headerRow.rt.sizeDelta = new Vector2
          (this.table.headerRow.rt.sizeDelta.x,
           this.table.minHeaderHeight);
        this.table.headerRow.rt.SetParent
          (this.table.headerRect.transform, false);
        this.table.headerRow.rt.localPosition = new Vector3(0, 0);
        this.table.headerRow.datum = headerDatum;

        go = new GameObject();
        go.name = "Border";
        rt = go.AddComponent<RectTransform>();
        img = go.AddComponent<Image>();
        img.sprite = this.table.fillerSprite;
        img.color = this.table.headerBorderColor;
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.pivot = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(0, 1);
        rt.localPosition = Vector3.zero;
        rt.transform.SetParent(this.table.headerRect, false);
      } // if headerRect == null

      this.instantiateCells(this.table.headerRow, true, false);

    } // if hasHeader

    /////////////////////////////////////////
    // Make our Footer
    /////////////////////////////////////////

    if (this.table.hasFooter) {
      if (this.table.footerRect == null) {
        go = new GameObject();
        this.table.footerRect = go.AddComponent<RectTransform>();
        this.table.footerRect.name = "Footer";
        this.table.footerRect.SetParent(this.table.root, false);

        this.table.footerRect.pivot = new Vector2(0, 1);
        this.table.footerRect.anchorMin = new Vector2(0, 1);
        this.table.footerRect.anchorMax = new Vector2(1, 1);

        go = new GameObject();
        rt = go.AddComponent<RectTransform>();
        this.table.footerRow = go.AddComponent<Row>();
        img = go.AddComponent<Image>();
        img.sprite = this.table.fillerSprite;
        img.color = this.table.footerBackgroundColor;
        this.table.footerRow.name = "FooterRow";

        go = new GameObject();
        go.name = "CanvasGroup";
        rt2 = go.AddComponent<RectTransform>();
        cg = go.AddComponent<CanvasGroup>();
        rt2.anchorMin = new Vector2(0, 0);
        rt2.anchorMax = new Vector2(1, 1);
        rt2.pivot = new Vector2(0f, 1f);
        rt2.SetParent(rt, false);
        rt2.offsetMin = new Vector2(0, 0);
        rt2.offsetMax = new Vector2(0, 0);

        mm = this.makeMeasureMaster(rt);

        this.table.footerRow.initialize(this.table, rt, rt2, cg, img, mm, false,
          true);
        this.table.footerRow.rt.anchorMin = new Vector2(0, 1f);
        this.table.footerRow.rt.anchorMax = new Vector2(1, 1f);
        this.table.footerRow.rt.pivot = new Vector2(0, 1);
        this.table.footerRow.rt.sizeDelta = new Vector2
          (this.table.footerRow.rt.sizeDelta.x,
           this.table.minFooterHeight);
        this.table.footerRow.rt.SetParent
          (this.table.footerRect.transform, false);
        this.table.footerRow.rt.localPosition = new Vector3(0, 0);
        this.table.footerRow.datum = footerDatum;

        go = new GameObject();
        go.name = "Border";
        rt = go.AddComponent<RectTransform>();
        img = go.AddComponent<Image>();
        img.sprite = this.table.fillerSprite;
        img.color = this.table.footerBorderColor;
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0, 1);
        rt.sizeDelta = new Vector2(0, 1);
        rt.localPosition = Vector3.zero;
        rt.transform.SetParent(this.table.footerRect, false);
      } // if footerRect == nul;

      this.instantiateCells(this.table.footerRow, false, true);

    } // if hasFooter

    /////////////////////////////////////////
    // Make our Body
    /////////////////////////////////////////

    if (this.table.bodyRect == null) {
      go = new GameObject();
      rt = go.AddComponent<RectTransform>();
      img = go.AddComponent<Image>();
      img.sprite = this.table.fillerSprite;
      img.color = this.table.bodyBackgroundColor;
      #if !(UNITY_5_0 || UNITY_5_1)
      if (this.table.use2DMask)
        go.AddComponent<RectMask2D>();
      else {
        go.AddComponent<Mask>().showMaskGraphic = true;
      }
      #else
      go.AddComponent<Mask>().showMaskGraphic = true;
      #endif

      this.table.bodyScroller = go.AddComponent<ScrollRect>();
      this.table.bodyScroller.transform.SetParent(this.table.root, false);
      this.table.bodyScroller.movementType = ScrollRect.MovementType.Clamped;
      this.table.bodyScroller.scrollSensitivity = this.table.scrollSensitivity;
      this.table.bodyScrollWatcher = go.AddComponent<ScrollWatcher>();
      this.table.bodyScrollWatcher.initialize(this.table);

      rt.pivot = new Vector2(0f, 1f);
      rt.anchorMin = new Vector2(0, 0);
      rt.anchorMax = new Vector2(1, 1);

      //don't hook this in till down here so we don't capture the resize above
      this.table.bodyRect = go.AddComponent<BodyRect>();
      this.table.bodyRect.name = "Body";
      this.table.bodyRect.init(this.table, rt);

      go = new GameObject();
      this.table.bodySizer = go.AddComponent<RectTransform>();
      this.table.bodySizer.name = "Sizer";
      this.table.bodySizer.transform.SetParent
        (this.table.bodyScroller.transform, false);
      this.table.bodySizer.anchorMin = new Vector2(0, 1);
      this.table.bodySizer.anchorMax = new Vector2(0, 1);
      this.table.bodySizer.pivot = new Vector2(0, 1);

      this.table.bodyScroller.content = this.table.bodySizer;
    }

    /////////////////////////////////////////
    // Make our Column Lines
    /////////////////////////////////////////

    if (this.table.hasColumnOverlay) {
      if (this.table.columnOverlayContent == null) {
        go = new GameObject();
        rt = go.AddComponent<RectTransform>();
        go.name = "ColumnOverlay";
        cg = go.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0, 1);
        rt.localPosition = Vector3.zero;
        rt.sizeDelta = Vector2.zero;
        rt.SetParent(this.table.root, false);
        this.table.columnOverlay = rt;

        go = new GameObject();
        this.table.columnOverlayContent = go.AddComponent<RectTransform>();
        this.table.columnOverlayContent.name = "Content";
        this.table.columnOverlayContent.transform.SetParent(rt, false);
        this.table.columnOverlayContent.pivot = new Vector2(0, 1);
        this.table.columnOverlayContent.anchorMin = new Vector2(0, 1);
        this.table.columnOverlayContent.anchorMax = new Vector2(0, 1);
        this.table.columnOverlayContent.sizeDelta = new Vector2(10f, 10f);
      } // if columnOverlayContent == null

      for (int i = 1; i < this.table.columns.Count; i++) {
        if (this.table.columnOverlayLines.Count < i) {
          go = new GameObject();
          go.name = "Border" + i.ToString();
          rt = go.AddComponent<RectTransform>();
          img = go.AddComponent<Image>();
          img.sprite = this.table.fillerSprite;
          img.color = this.table.columnLineColor;
          rt.anchorMin = new Vector2(0, 1);
          rt.anchorMax = new Vector2(0, 1);
          rt.pivot = new Vector2(0, 1);
          rt.sizeDelta = new Vector2(1, 50);
          rt.localPosition = Vector3.zero;
          rt.transform.SetParent(this.table.columnOverlayContent, false);
          this.table.columnOverlayLines.Add(rt);
        }
      }

      // deactivate any old overlay lines if we are a re-run with less cols
      for (int i = 0; i < this.table.columnOverlayLines.Count; i++) {
        // NOTE: - 2 here because we aactually have 1 less column line than col
        if (i <= this.table.columns.Count - 2)
          this.table.columnOverlayLines[i].gameObject.SetActive(true);
        else
          this.table.columnOverlayLines[i].gameObject.SetActive(false);
      }

    } // if hasColumnOverlay

    if (this.firstBuild) {
      this.makeScrollbar(true);
      this.makeScrollbar(false);

      go = new GameObject();
      go.name = "InputField";
      rt = go.AddComponent<RectTransform>();
      img = go.AddComponent<Image>();
      img.sprite = this.table.fillerSprite;

      go = new GameObject();
      go.name = "Text";
      rt2 = go.AddComponent<RectTransform>();
      text = go.AddComponent<Text>();
      text.horizontalOverflow = HorizontalWrapMode.Wrap;
      text.verticalOverflow = VerticalWrapMode.Overflow;
      text.font = this.table.font;
      text.fontStyle = this.table.fontStyle;
      text.supportRichText = false;
      rt2.SetParent(rt);
      rt2.anchorMin = new Vector2(0f, 1f);
      rt2.anchorMax = new Vector2(0f, 1f);
      rt2.pivot = new Vector2(0f, 1f);
      rt2.offsetMin = new Vector2(0, 0);
      rt2.offsetMax = new Vector2(0, 0);

      inputField = rt.gameObject.AddComponent<InputField>();
      inputField.textComponent = text;
      inputField.transform.SetParent(this.table.root);
      inputField.gameObject.SetActive(false);

      this.table.inputCell = rt.gameObject.AddComponent<InputCell>();
      this.table.inputCell.initialize(this.table, rt, inputField);

    }

    if (this.table.loadingOverlay == null) {
      go = new GameObject();
      rt = go.AddComponent<RectTransform>();
      rt.SetParent(this.table.root, false);
      rt.anchorMin = new Vector2(0, 0);
      rt.anchorMax = new Vector2(1, 1);
      rt.pivot = new Vector2(0.5f, 0.5f);
      rt.offsetMin = Vector2.zero;
      rt.offsetMax = Vector2.zero;
      go.name = "LoadingOverlay";
      img = go.AddComponent<Image>();
      img.sprite = this.table.fillerSprite;
      img.color = this.table.bodyBackgroundColor;
      this.table.loadingOverlay = go.AddComponent<CanvasGroup>();
      this.table.loadingOverlay.blocksRaycasts = true;

      go = new GameObject();
      go.name = "Spinner";
      rt = go.AddComponent<RectTransform>();
      rt.SetParent(this.table.loadingOverlay.transform, false);
      img = go.AddComponent<Image>();
      img.sprite = this.table.spinnerSprite;
      img.color = this.table.spinnerColor;
      go.AddComponent<Spinner>();
      rt.anchorMin = new Vector2(0.5f, 0.5f);
      rt.anchorMax = new Vector2(0.5f, 0.5f);
      rt.pivot = new Vector2(0.5f, 0.5f);
      rt.offsetMin = new Vector2(-16f, -16f);
      rt.offsetMax = new Vector2(16f, 16f);

    }

    this.firstBuild = false;

  } // build

  public void makeRows() {

    GameObject go;
    RectTransform rt;
    RectTransform rt2;
    CanvasGroup cg;
    Row row;
    Image img;
    MeasureMaster mm;

    for (int i = 0; i <
      ((this.table.bodyRect.rt.rect.height - this.table.minHeaderHeight -
        this.table.minFooterHeight) / this.table.minRowHeight) + 1 || i < this.table.rows.Count; i++) {
      if (this.table.rows.Count <= i) {
        go = new GameObject();
        rt = go.AddComponent<RectTransform>();
        img = go.AddComponent<Image>();
        img.sprite = this.table.fillerSprite;
        img.color = this.table.rowNormalColor;
        row = go.AddComponent<Row>();
        row.name = "Row" + i.ToString();

        go = new GameObject();
        go.name = "CanvasGroup";
        rt2 = go.AddComponent<RectTransform>();
        cg = go.AddComponent<CanvasGroup>();
        rt2.anchorMin = new Vector2(0, 0);
        rt2.anchorMax = new Vector2(1, 1);
        rt2.pivot = new Vector2(0f, 1f);
        rt2.SetParent(rt, false);
        rt2.offsetMin = new Vector2(0, 0);
        rt2.offsetMax = new Vector2(0, 0);

        mm = this.makeMeasureMaster(rt);

        row.initialize(this.table, rt, rt2, cg, img, mm);
        row.rt.anchorMin = new Vector2(0, 1f);
        row.rt.anchorMax = new Vector2(0, 1f);
        row.rt.pivot = new Vector2(0, 1);
        row.rt.sizeDelta = new Vector2
          (row.rt.sizeDelta.x, this.table.minRowHeight);

        row.rt.SetParent(this.table.bodySizer.transform, false);

        if (i > 0) {
          row.preceedingRow = this.table.rows[i - 1];
        }

        go = new GameObject();
        row.extraTextRt = go.AddComponent<RectTransform>();
        go.name = "ExtraTextBox";
        img = row.extraTextBackground = go.AddComponent<Image>();
        img.sprite = this.table.fillerSprite;
        row.extraTextBackground.color = this.table.extraTextBoxColor;
        row.extraTextRt.anchorMin = new Vector2(0, 1);
        row.extraTextRt.anchorMax = new Vector2(0, 1);
        row.extraTextRt.pivot = new Vector2(0, 1);
        row.extraTextRt.transform.SetParent(row.rt, false);

        go = new GameObject();
        go.name = "ExtraText";
        rt = go.AddComponent<RectTransform>();
        row.extraText = go.AddComponent<Text>();
        row.extraText.font = this.table.font;
        row.extraText.fontStyle = this.table.fontStyle;
        row.extraText.fontSize = this.table.defaultFontSize;
        row.extraText.horizontalOverflow = HorizontalWrapMode.Wrap;
        row.extraText.verticalOverflow = VerticalWrapMode.Overflow;
        row.extraText.color = this.table.extraTextColor;
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0, 1);
        rt.SetParent(row.extraTextRt, false);
        rt.localPosition = Vector3.zero;
        rt.offsetMin = new Vector2(this.table.horizontalSpacing * 0.5f,
                                   this.table.rowVerticalSpacing * 0.5f);
        rt.offsetMax = new Vector2(this.table.horizontalSpacing * -0.5f,
                                   this.table.rowVerticalSpacing * -0.5f);

        if (i > 0) {
          go = new GameObject();
          go.name = "Border";
          rt = go.AddComponent<RectTransform>();
          img = go.AddComponent<Image>();
          img.sprite = this.table.fillerSprite;
          img.color = this.table.rowLineColor;
          rt.anchorMin = new Vector2(0, 1);
          rt.anchorMax = new Vector2(1, 1);
          rt.pivot = new Vector2(0, 1);
          rt.sizeDelta = new Vector2(0, 1);
          rt.localPosition = Vector3.zero;
          rt.transform.SetParent(row.rt, false);
        }
      } // if rows.Count <= i
      else {
        row = this.table.rows[i];
        row.isOver = false;
        row.isDown = false;
      }

      this.instantiateCells(row, false, false);

    } // for
  }  // makeRows

  private void instantiateCells(Row row, bool isHeader, bool isFooter) {

    GameObject go;
    RectTransform rt;
    RectTransform crt;
    Text text;
    Cell cell;
    Image img = null;

    for (int j = 0; j < this.table.columns.Count; j++) {
      Column column = this.table.columns[j];

      if (row.cells.Count <= j) {
        go = new GameObject();
        go.name = "Column" + j;
        rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1f);
        img = go.AddComponent<Image>();
        img.sprite = this.table.fillerSprite;
        img.color = Color.red;
        if (isHeader)
          cell = go.AddComponent<HeaderCell>();
        else
          cell = go.AddComponent<Cell>();
        cell.background = img;
        go = new GameObject();
        crt = go.AddComponent<RectTransform>();
        crt.transform.SetParent(rt, false);
      }
      else {
        cell = row.cells[j];
        // We can't change an image cell to a text cell, so
        //  nuke here and rebuild if that's what's happening
        if (((column.columnType == Column.ColumnType.TEXT ||
              isHeader || isFooter) && cell.image != null) ||
             ((column.columnType == Column.ColumnType.IMAGE &&
              !isHeader && !isFooter) && cell.text != null))  {
          Component.Destroy(cell.gameObject);
          go = new GameObject();
          go.name = "Column" + j;
          rt = go.AddComponent<RectTransform>();
          rt.anchorMin = new Vector2(0, 1);
          rt.anchorMax = new Vector2(0, 1);
          rt.pivot = new Vector2(0, 1f);
          if (isHeader) {
            img = go.AddComponent<Image>();
            img.sprite = this.table.fillerSprite;
            img.color = Color.red;
            cell = go.AddComponent<HeaderCell>();
            (cell as HeaderCell).background = img;
          }
          else
            cell = go.AddComponent<Cell>();
          go = new GameObject();
          crt = go.AddComponent<RectTransform>();
          crt.transform.SetParent(rt, false);
        }
        else {
          cell.gameObject.SetActive(true);
          go = cell.crt.gameObject;
          rt = cell.rt;
          crt = cell.crt;
        }
      }

      if (column.columnType == Column.ColumnType.TEXT || isHeader || isFooter) {

        if (column.horAlignment == Column.HorAlignment.LEFT) {
          crt.anchorMin = new Vector2(0, 0.5f);
          crt.anchorMax = new Vector2(0, 0.5f);
        }
        else if (column.horAlignment == Column.HorAlignment.CENTER) {
          crt.anchorMin = new Vector2(0.5f, 0.5f);
          crt.anchorMax = new Vector2(0.5f, 0.5f);
          crt.pivot = new Vector2(0, 1);
        }
        else {
          crt.anchorMin = new Vector2(1, 0.5f);
          crt.anchorMax = new Vector2(1, 0.5f);
          crt.pivot = new Vector2(0, 1);
        }

        crt.pivot = new Vector2(0, 1);
        crt.sizeDelta = Vector2.zero;
        go.name = j + "Text";
        if (cell.text == null)
          text = go.AddComponent<Text>();
        else
          text = cell.text;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.font = this.table.font;
        text.fontStyle = this.table.fontStyle;
        if (isHeader)
          text.color = this.table.headerTextColor;
        else if (isFooter)
          text.color = this.table.footerTextColor;
        else
          text.color = this.table.rowTextColor;
        if (column.horAlignment == Column.HorAlignment.LEFT) {
          text.alignment = TextAnchor.MiddleLeft;
        }
        else if (column.horAlignment == Column.HorAlignment.CENTER) {
          text.alignment = TextAnchor.MiddleCenter;
        }
        else {
          text.alignment = TextAnchor.MiddleRight;
        }
        if (cell.rt == null)
          cell.initialize(this.table, row, column, j, rt, crt, text);
        text.fontSize = column.calcFont(isHeader, isFooter);
      }
      else {
        crt.anchorMin = new Vector2(0.5f, 0.5f);
        crt.anchorMax = new Vector2(0.5f, 0.5f);
        crt.pivot = new Vector2(0.5f, 0.5f);
        crt.sizeDelta = new Vector2(column.imageWidth, column.imageHeight);
        go.name = j + "Image";
        if (cell.image == null) {
          img = go.AddComponent<Image>();
          img.sprite = this.table.fillerSprite;
        }
        else
          img = cell.image;
        img.color = Color.white;
        if (cell.rt == null)
          cell.initialize(this.table, row, column, j, rt, crt, img);
      }

      cell.rt.SetParent(row.cgrt, false);

      if (isHeader && (cell as HeaderCell).icon == null) {
        go = new GameObject();
        go.name = "Icon";
        rt = go.AddComponent<RectTransform>();
        img = go.AddComponent<Image>();
        img.sprite = this.table.fillerSprite;
        img.color = this.table.rowLineColor;
        rt.anchorMin = new Vector2(1f, 0.5f);
        rt.anchorMax = new Vector2(1f, 0.5f);
        rt.pivot = new Vector2(0f, 0.5f);
        rt.SetParent(cell.crt, false);
        rt.sizeDelta = new Vector2
          (this.table.headerIconWidth, this.table.headerIconHeight);
        rt.localPosition = new Vector2(2f, 0f);
        (cell as HeaderCell).icon = img;
      }
    } // for (int j = 0; j < this.table.columns.Count; j++)

    for (int j = 0; j < row.cells.Count; j++) {
      if (j <= this.table.columns.Count - 1)
        row.cells[j].gameObject.SetActive(true);
      else
        row.cells[j].gameObject.SetActive(false);
    }

  } // instantiateCells

  private void makeScrollbar(bool hor) {

    GameObject go;
    RectTransform rt;
    RectTransform rt2;
    Image img;
    Scrollbar sb;

    go = new GameObject();
    rt = go.AddComponent<RectTransform>();
    img = go.AddComponent<Image>();
    img.sprite = this.table.fillerSprite;
    img.color = this.table.scrollBarBackround;
    sb = go.AddComponent<Scrollbar>();
    if (hor) {
      go.name = "HorizontalScollbar";
      rt.anchorMin = new Vector2(0, 0);
      rt.anchorMax = new Vector2(1, 0);
      rt.pivot = new Vector2(0, 1);
      this.table.horScrollerRt = rt;
    }
    else {
      go.name = "VerticalScollbar";
      rt.anchorMin = new Vector2(1, 0);
      rt.anchorMax = new Vector2(1, 1);
      rt.pivot = new Vector2(0, 1);
      this.table.verScrollerRt = rt;
      sb.direction = Scrollbar.Direction.BottomToTop;
    }
    rt.transform.SetParent(this.table.root);

    go = new GameObject();
    go.name = "SlidingArea";
    rt = go.AddComponent<RectTransform>();
    rt.anchorMin = new Vector2(0, 0);
    rt.anchorMax = new Vector2(1, 1);
    rt.pivot = new Vector2(0.5f, 0.5f);
    rt.transform.SetParent(sb.transform);
    rt.offsetMin = new Vector2(10, 10);
    rt.offsetMax = new Vector2(-10, -10);

    go = new GameObject();
    go.name = "Handle";
    rt2 = go.AddComponent<RectTransform>();
    img = go.AddComponent<Image>();
    img.sprite = this.table.fillerSprite;
    img.color = this.table.scrollBarForeground;
    rt2.anchorMin = new Vector2(0.8f, 0);
    rt2.anchorMax = new Vector2(1, 1);
    rt2.pivot = new Vector2(0.5f, 0.5f);
    rt2.transform.SetParent(rt);
    rt2.offsetMin = new Vector2(-10, -10);
    rt2.offsetMax = new Vector2(10, 10);
    sb.targetGraphic = img;
    sb.handleRect = rt2;

    if (hor)
      this.table.bodyScroller.horizontalScrollbar = sb;
    else
      this.table.bodyScroller.verticalScrollbar = sb;
  } // makeScrollbar

  private MeasureMaster makeMeasureMaster(RectTransform parent) {
    GameObject go = new GameObject();
    go.name = "MeasureMaster";
    go.AddComponent<RectTransform>();
    go.SetActive(false);
    MeasureMaster mm = go.AddComponent<MeasureMaster>();
    go.transform.SetParent(parent, false);
    Text text = go.AddComponent<Text>();
    text.font = this.table.font;
    text.fontStyle = this.table.fontStyle;
    CanvasGroup cg = go.AddComponent<CanvasGroup>();
    cg.alpha = 0f;
    cg.blocksRaycasts = false;
    mm.initialize(this.table, text, this.control);
    return mm;
  }

} // Factory
}

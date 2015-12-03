using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SLS.Widgets.Table
{
    public class Table : UIBehaviour, ILayoutGroup
    {
        public Font font;
        public FontStyle fontStyle;
        public bool use2DMask;
        public Sprite fillerSprite;

        // GENERAL SETTINGS
        public int defaultFontSize = 12;
        public int scrollSensitivity = 5;
        public float leftMargin;
        public float rightMargin;
        public float horizontalSpacing;
        public Color bodyBackgroundColor = Color.white;
        public Color columnLineColor = Color.white;
        public bool min100PercentWidth = true;
        public bool max100PercentWidth = false;
        public Sprite spinnerSprite;
        public Color spinnerColor = Color.white;

        // HEADER SETTINGS
        public float minHeaderHeight = 50f;
        public float headerTopMargin;
        public float headerBottomMargin;
        public Color headerNormalColor = Color.white;
        public Color headerHoverColor = Color.white;
        public Color headerDownColor = Color.white;
        public Color headerBorderColor = Color.white;
        public Color headerTextColor = Color.black;
        public int headerIconWidth = 8;
        public int headerIconHeight = 16;

        // FOOTER SETTINGS
        public float minFooterHeight = 40f;
        public float footerTopMargin;
        public float footerBottomMargin;
        public Color footerBackgroundColor = Color.white;
        public Color footerBorderColor = Color.white;
        public Color footerTextColor = Color.black;

        // ROW SETTINGS
        public float minRowHeight = 40f;
        public float rowVerticalSpacing;
        public Color rowLineColor = Color.white;
        public Color rowNormalColor = Color.black;
        public Color rowHoverColor = Color.black;
        public Color rowDownColor = Color.black;
        public Color rowSelectColor = Color.black;
        public Color cellHoverColor = new Color(0, 0, 0, 0f);
        public Color cellDownColor = new Color(0, 0, 0, 0f);
        public Color cellSelectColor = new Color(0, 0, 0, 0f);
        public Color rowTextColor = Color.white;

        public Action<RectTransform, string> tooltipHandler;

        // EXTRA TEXT
        // 0 - 1 (percent of frame extra text box should fill 1 == 100%)
        public float extraTextWidthRatio = 0.6f;
        public Color extraTextBoxColor = Color.black;
        public Color extraTextColor = Color.white;

        // SCROLLBAR
        public Color scrollBarBackround = new Color(0.5f, 0.5f, 0.5f, 0.1f);
        public Color scrollBarForeground = new Color(0.5f, 0.5f, 0.5f, 0.5f);


        //public Dictionary<string, Column> _columns;
        //public Dictionary<string, Column> columns { get { return this._columns; } }
        public List<Column> _columns;
        public List<Column> columns { get { return this._columns; } }

        [HideInInspector]
        public List<Row> rows;
        [HideInInspector]
        public TableDatumList data;

        private Action<Datum> selectionCallback;

        private Action<Datum, Column> selectionCallbackWithColumn;

        private Datum _selectedDatum;

        public Datum selectedDatum
        {
            get { return this._selectedDatum; }
            set { this.setSelected(value); }
        }

        private Column _selectedColumn;

        public Column selectedColumn
        {
            get { return this._selectedColumn; }
        }

        public void setSelected(Datum d, Column c = null, bool doCallback = true)
        {
            Row row;
            this._selectedColumn = c;
            if (this._selectedDatum != d)
            {
                row = this.getRowForDatum(this._selectedDatum);
                this._selectedDatum = d;
                if (row != null) row.setColor();
            }
            row = this.getRowForDatum(this._selectedDatum);
            if (row != null) row.setColor();
            if (doCallback)
            {
                if (this.selectionCallback != null)
                    this.selectionCallback(this._selectedDatum);
                if (this.selectionCallbackWithColumn != null)
                    this.selectionCallbackWithColumn(this._selectedDatum, this._selectedColumn);
            }
        }

        public void setSelected(int x, int y, bool doCallback = true)
        {
            Column column = _selectedColumn;
            Datum datum = _selectedDatum;

            if (x < _columns.Count && x >= 0)
            {
                column = _columns[x];
            }

            if (y < data.Count && y >= 0)
            {
                datum = data[y];
            }

            setSelected(datum, column, doCallback);
        }

        public Element GetSelectedElement()
        {
            if (_selectedColumn != null && _selectedDatum != null)
            {
                int x = _columns.IndexOf(_selectedColumn);
                return _selectedDatum.elements[x];
            }
            return null;
        }
        

        public void moveSelectionUp(bool doCallback = true)
        {
            int x = _columns.IndexOf(_selectedColumn);
            int y = data.IndexOf(_selectedDatum);
            y = y - 1;
            setSelected(x, y, doCallback);
        }

        public void moveSelectionDown(bool doCallback = true)
        {
            int x = _columns.IndexOf(_selectedColumn);
            int y = data.IndexOf(_selectedDatum);
            y = y + 1;
            setSelected(x, y, doCallback);
        }

        public void moveSelectionLeft(bool doCallback = true)
        {
            int x = _columns.IndexOf(_selectedColumn);
            int y = data.IndexOf(_selectedDatum);
            x = x - 1;
            setSelected(x, y, doCallback);
        }

        public void moveSelectionRight(bool doCallback = true)
        {
            int x = _columns.IndexOf(_selectedColumn);
            int y = data.IndexOf(_selectedDatum);
            x = x + 1;
            setSelected(x, y, doCallback);
        }
    

        

        [HideInInspector]
        public RectTransform root;

        private bool _hasHeader;
        public bool hasHeader { get { return this._hasHeader; } }
        private bool _hasHeaderIcons;
        public bool hasHeaderIcons { get { return this._hasHeaderIcons; } }
        private bool _hasFooter;
        public bool hasFooter { get { return this._hasFooter; } }

        private Datum headerDatum;
        private Datum footerDatum;

        [HideInInspector]
        public RectTransform headerRect;
        [HideInInspector]
        public Row headerRow;

        [HideInInspector]
        public RectTransform footerRect;
        [HideInInspector]
        public Row footerRow;

        [HideInInspector]
        public ScrollRect bodyScroller;
        [HideInInspector]
        public ScrollWatcher bodyScrollWatcher;
        [HideInInspector]
        public BodyRect bodyRect;
        [HideInInspector]
        public RectTransform bodySizer;
        [HideInInspector]
        public RectTransform horScrollerRt;
        [HideInInspector]
        public RectTransform verScrollerRt;

        public CanvasGroup loadingOverlay;

        private bool _hasColumnOverlay;
        public bool hasColumnOverlay { get { return this._hasColumnOverlay; } }

        [HideInInspector]
        public RectTransform columnOverlayContent;
        [HideInInspector]
        public List<RectTransform> columnOverlayLines;

        private Factory factory;
        private Control control;

        [HideInInspector]
        public Column extraTextColumn;

        [HideInInspector]
        public InputCell inputCell;

        private bool _isRunning;
        public bool isRunning { get { return this._isRunning; } }

        private bool _hasError;
        public bool hasError { get { return this._hasError; } }
        public void error(string message)
        {
            this._hasError = true;
            Debug.LogError(message);
        }

        private bool doingDirtyLater;

        private Dictionary<string, Sprite> _sprites;
        public Dictionary<string, Sprite> sprites { get { return this._sprites; } }


        public void reset()
        {
            this._isRunning = false;
            this._hasError = false;
            this._hasHeader = false;
            this._hasFooter = false;
            this._hasColumnOverlay = false;
            this._hasHeaderIcons = false;
            if (this._columns != null) this._columns.Clear();
            else this._columns = new List<Column>();
            this.headerDatum = null;
            this.footerDatum = null;
            this.columnOverlayContent = null;
            this.columnOverlayLines = null;
            GameObject header = GameObject.Find("Header");
            GameObject footer = GameObject.Find("Footer");
            GameObject columnOverlay = GameObject.Find("ColumnOverlay");
            if (header != null) Destroy(header);
            if (footer != null) Destroy(footer);
            if (columnOverlay != null) Destroy(columnOverlay);
            
            
        }

        public Column addTextColumn(string header = null, string footer = null,
          float minWidth = -1, float maxWidth = -1)
        {
            return this._addTextOrInputColumn(header, footer, minWidth, maxWidth, false);
        }

        public Column addInputColumn(Action<Datum, Column, string, string> changeCallback,
          string header = null, string footer = null,
          float minWidth = -1, float maxWidth = -1)
        {
            Column c = this._addTextOrInputColumn(header, footer, minWidth, maxWidth, true);
            c.inputChangeCallback = changeCallback;
            return c;
        }

        private Column _addTextOrInputColumn(string header = null, string footer = null,
          float minWidth = -1, float maxWidth = -1, bool isInput = false)
        {
            if (this.headerDatum == null)
            {
                this.headerDatum = Datum.Header();
                this.headerDatum.table = this;
            }
            if (this.footerDatum == null)
            {
                this.footerDatum = Datum.Footer();
                this.footerDatum.table = this;
            }
            this.headerDatum.elements.Add();
            this.footerDatum.elements.Add();
            Column c = Column.TextColumn(this, this.columns.Count,
              minWidth, maxWidth, this.headerDatum, this.footerDatum, isInput);
            this.columns.Add(c);
            c.headerValue = header;
            c.footerValue = footer;
            return c;
        }

        public Column addImageColumn(string header = null, string footer = null,
          int imageWidth = 32, int imageHeight = 32)
        {
            if (this.headerDatum == null)
            {
                this.headerDatum = Datum.Header();
                this.headerDatum.table = this;
            }
            if (this.footerDatum == null)
            {
                this.footerDatum = Datum.Footer();
                this.footerDatum.table = this;
            }
            this.headerDatum.elements.Add();
            this.footerDatum.elements.Add();
            Column c = Column.ImageColumn(this, this.columns.Count,
              imageWidth, imageHeight, this.headerDatum, this.footerDatum);
            this.columns.Add(c);
            c.headerValue = header;
            c.footerValue = footer;
            return c;
        }

        public void initialize()
        {
            this.selectionCallback = null;
            this.selectionCallbackWithColumn = null;
            this._initialize();
        }

        public void initialize(Action<Datum, Column> selectionCallback,
          Dictionary<string, Sprite> sprites = null, bool hasHeaderIcons = false,
          Action<Column, PointerEventData> headerClickCallback = null)
        {
            this.selectionCallback = null;
            this.selectionCallbackWithColumn = selectionCallback;
            this._initialize(sprites, hasHeaderIcons, headerClickCallback);
        }

        public void initialize(Action<Datum> selectionCallback,
          Dictionary<string, Sprite> sprites = null, bool hasHeaderIcons = false,
          Action<Column, PointerEventData> headerClickCallback = null)
        {
            this.selectionCallback = selectionCallback;
            this.selectionCallbackWithColumn = null;
            this._initialize(sprites, hasHeaderIcons, headerClickCallback);
        }

        private void _initialize(Dictionary<string, Sprite> sprites = null, bool hasHeaderIcons = false,
          Action<Column, PointerEventData> headerClickCallback = null)
        {

            this._sprites = sprites;

            this._hasHeader = false;
            this._hasFooter = false;

            if (this.font == null)
            {
                this.font = Resources.GetBuiltinResource
                  (typeof(Font), "Arial.ttf") as Font;
                Debug.Log("FYI: Using Default Arial Font");
            }

            if (this.minRowHeight < 14)
            {
                this.error("Table Min Row Height must be >= 14 (set this as high as practical in order to reduce initialization overhead)");
            }

            if (this.defaultFontSize < 12)
            {
                this.error("Table Default Font Size must be >= 12");
            }

            if (this.rowVerticalSpacing + this.defaultFontSize > this.minRowHeight)
            {
                this.minRowHeight = this.rowVerticalSpacing + this.defaultFontSize;
            }

            if (this.headerTopMargin + this.headerBottomMargin + this.defaultFontSize > this.minHeaderHeight)
            {
                this.minHeaderHeight = this.headerTopMargin + this.headerBottomMargin + this.defaultFontSize;
            }

            if (this.footerTopMargin + this.footerBottomMargin + this.defaultFontSize > this.minFooterHeight)
            {
                this.minFooterHeight = this.footerTopMargin + this.footerBottomMargin + this.defaultFontSize;
            }

            if (this.loadingOverlay != null)
            {
                this.loadingOverlay.gameObject.SetActive(true);
                this.loadingOverlay.alpha = 1f;
                this.overlayIsHiding = false;
            }


            if (this.rows == null)
                this.rows = new List<Row>();

            for (int i = 0; i < columns.Count; i++)
            {
                Column c = columns[i];
                if (c.columnType == Column.ColumnType.IMAGE)
                {
                    if (this.sprites == null)
                        this.error("Cannot declare Image Column without spriteDict");
                    this.minRowHeight = Mathf.Max(this.minRowHeight,
                      c.imageHeight + this.rowVerticalSpacing);
                }
                if (c.headerValue != null)
                    this._hasHeader = true;
                if (c.footerValue != null)
                    this._hasFooter = true;
            }

            if (this.hasHeader)
            {
                this.minHeaderHeight = Mathf.Max(this.minHeaderHeight,
                  this.headerIconHeight + this.headerTopMargin +
                  this.headerBottomMargin);
            }

            this._hasHeaderIcons = hasHeaderIcons;

            if (this.columns.Count > 1)
                this._hasColumnOverlay = true;
            else
                this._hasColumnOverlay = false;

            if (this.columnOverlayLines == null)
                this.columnOverlayLines = new List<RectTransform>();

            if (this.factory == null)
                this.factory = new Factory(this);

            if (this.control == null)
            {
                this.control = new Control(this, this.factory);
                this.extraTextColumn = Column.TextColumn(this, -1, -1, -1,
                  this.headerDatum, this.footerDatum, false);
            }

            if (this.data != null)
                this.data.Clear();
            //intentially make a new one here, dont reuse old
            this.data = new TableDatumList(this, this.control);

            this.factory.build(this.headerDatum, this.footerDatum, this.control);

            //////////////////////////////////////////////
            // Do post factory cleanup and assigns here
            if (this.headerRow != null)
            {
                for (int i = 0; i < this.headerRow.cells.Count; i++)
                {
                    HeaderCell hc = this.headerRow.cells[i] as HeaderCell;
                    hc.icon.gameObject.SetActive(this.hasHeaderIcons);
                    if (this.columns.Count - 1 >= i)
                    {
                        hc.initialize(this.columns[i], headerClickCallback);
                    }
                }
            }

            this.bodyRect.isMeasured = false;

            this.bodyScroller.horizontalNormalizedPosition = 0f;
            this.bodyScroller.verticalNormalizedPosition = 1f;

        }  // _initialize

        override protected void OnEnable()
        {
            base.OnEnable();
        }

        override protected void OnDisable()
        {
            base.OnDisable();
            this._isRunning = false;
        }

        public void setGameObjectActiveLater(GameObject go, bool state)
        {
            if ((go.activeInHierarchy && !state) || (!go.activeInHierarchy && state))
                StartCoroutine(this.doSetGameObjectActiveLater(go, state));
        }

        IEnumerator doSetGameObjectActiveLater(GameObject go, bool state)
        {
            yield return new WaitForEndOfFrame();
            go.SetActive(state);
        }

        public void dirtyNow()
        {
            //this.lastMarkForRebuild = Time.realtimeSinceStartup;
            LayoutRebuilder.MarkLayoutForRebuild(this.root);
        }

        public void dirtyLater()
        {
            //this.lastMarkForRebuild = Time.realtimeSinceStartup;
            if (!this.doingDirtyLater && this.gameObject.activeInHierarchy)
                StartCoroutine(this.doDirtyLater());
        }

        IEnumerator doDirtyLater()
        {
            this.doingDirtyLater = true;
            yield return new WaitForEndOfFrame();
            this.doingDirtyLater = false;
            LayoutRebuilder.MarkLayoutForRebuild(this.root);
        }


        private bool overlayIsHiding;
        public void fadeOverlay(float overTime, float v0, float v1, float delay = 0)
        {
            if (this.overlayIsHiding ||
              !this.loadingOverlay.gameObject.activeInHierarchy)
                return;
            this.overlayIsHiding = true;
            StartCoroutine(this.dofadeOverlay(overTime, v0, v1, delay));
        }

        IEnumerator dofadeOverlay(float overTime, float v0, float v1, float delay)
        {
            yield return new WaitForSeconds(delay);
            float startTime = Time.time;
            while (Time.time < startTime + overTime)
            {
                this.loadingOverlay.alpha =
                  Mathf.Lerp(v0, v1, (Time.time - startTime) / overTime);
                yield return null;
            }
            this.loadingOverlay.gameObject.SetActive(false);
        }

        public void startRenderEngine()
        {
            if (this.hasError) return;
            this._isRunning = true;
            if (this.hasHeader)
            {
                this.headerRow.datum = this.headerDatum;
                this.headerDatum.isDirty = true;
            }
            if (this.hasFooter)
            {
                this.footerRow.datum = this.footerDatum;
                this.footerDatum.isDirty = true;
            }
            this.control.draw();
        }

        public float sumColumnWidths()
        {
            float wsum = 0;
            for (int i = 0; i < this.columns.Count; i++)
            {
                //print("sum col: " + i + " width: " + this.columnList[i].rawWidth);
                wsum += this.columns[i].safeWidth;
            }
            return wsum;
        }

        public void SetLayoutVertical()
        {
            if (this.bodyRect == null || !this.isRunning) return;
            this.control.setLayoutVertical();
        }

        public void SetLayoutHorizontal()
        {
            if (this.bodyRect == null || !this.isRunning) return;
            this.control.setLayoutHorizontal();
        }

        public void exposedDimensionsChanged()
        {
            this.OnRectTransformDimensionsChange();
        }

        override protected void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (this.isActiveAndEnabled && this.control != null)
            {
                this.loadingOverlay.gameObject.SetActive(true);
                this.loadingOverlay.alpha = 1f;
                this.overlayIsHiding = false;
                for (int i = 0; i < this.columns.Count; i++)
                    this.columns[i].clearMeasure();
                this.bodyScroller.horizontalNormalizedPosition = 0f;
                this.bodyScroller.verticalNormalizedPosition = 1f;
                this.control.sizeForRectTransform();
                this.data.clearMeasured();
                this.data.clearSafeHeightSum();
                for (int i = 0; i < this.data.Count; i++)
                    this.data[i].isDirty = true;
                StartCoroutine(this.startRenderLater());
            }
        }

        IEnumerator startRenderLater()
        {
            yield return new WaitForEndOfFrame();
            this.startRenderEngine();
        }

        public Row getRowForDatum(Datum item)
        {
            if (item == null) return null;
            if (item.isHeader) return this.headerRow;
            if (item.isFooter) return this.footerRow;
            for (int i = 0; i < this.rows.Count; i++)
            {
                if (this.rows[i].datum == item)
                    return this.rows[i];
            }
            return null;
        }

    }

}
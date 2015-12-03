using UnityEngine;
using System;
using System.Collections.Generic;

namespace SLS.Widgets.Table
{

    public class Element
    {

        private Datum datum;
        private object payload;



        public string tooltip { set; get; }

        private string _value;
        public string value
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this.datum.animationStartTime = Time.realtimeSinceStartup;
                this.datum.isDirty = true;
            }
        }

        private Color? _color;
        public Color? color
        {
            get { return this._color; }
            set
            {
                this._color = value;
                this.datum.isDirty = true;
            }
        }

        public float? measuredWidth;
        public float? measuredHeight;

        public object GetPayload()
        {
            return payload;
        }

        public Element(Datum d, string value, object payload = null)
        {
            this.datum = d;
            this.value = value;
            this.payload = payload;
        }

        public void clearMeasure()
        {
            this.measuredWidth = null;
            this.measuredHeight = null;
        }

    }

    public class Datum
    {
        public string uid { set; get; }

        public object rawObject { set; get; }

        public string tooltip { set; get; }

        public DatumElementList elements { set; get; }

        public Element extraText { set; get; }
        public Color? extraTextBoxColor;
        public Color? extraTextColor;

        public Table table;

        public float animationStartTime;

        private bool _isDirty;
        private float _revision;
        public float revision
        {
            get
            {
                return this._revision;
            }
        }
        private float _lastDirty;
        public float lastDirty
        {
            get
            {
                return this._lastDirty;
            }
        }

        private float _lastClean;
        public float lastClean
        {
            get
            {
                return this._lastClean;
            }
        }

        public bool isDirty
        {
            get { return this._isDirty; }
            set
            {
                //Debug.Log(this.uid + " setting isDirty: " + value);
                this._isDirty = value;
                if (this._isDirty)
                {
                    this._revision = Time.realtimeSinceStartup;
                    this._lastDirty = this._revision;
                }
                else
                {
                    this._lastClean = Time.realtimeSinceStartup;
                }
                if (this._isDirty && this.table != null && this.table.isRunning && this.table.data != null)
                {
                    this.table.data.clearSafeHeightSum();
                    this.table.data.clearMeasuredVertPos();
                    Row r = this.table.getRowForDatum(this);
                    if (r != null)
                    {
                        //Debug.Log("running row refresh on datum: " + this.uid);
                        r.refresh();
                    }
                }
            }
        }

        public void clearMeasure()
        {
            //Debug.Log("Clearing measure on " + this.uid);
            //this.addedHeight = false;
            this.measuredVertPos = null;
            this._measuredHeight = null;
            this._measuredCellHeight = null;
            this._lastSafeHeightResult = null;
            this.table.data.clearSafeHeightSum();
            for (int i = 0; i < this.elements.Count; i++)
            {
                if (this.elements[i] != null)
                    this.elements[i].clearMeasure();
            }
            if (this.extraText != null)
                this.extraText.clearMeasure();
        }

        public float? measuredVertPos;

        public float safeExtraTextHeight()
        {
            if (this.extraText != null && this.extraText.measuredHeight.HasValue)
            {
                //Debug.Log("ETH: " + this.extraText.measuredHeight.Value);
                return this.extraText.measuredHeight.Value;
            }
            return 0;
        }

        private float? _measuredCellHeight;
        public float safeCellHeight()
        {
            if (!this._measuredCellHeight.HasValue)
            {
                float tmpHeight = 0;
                for (int i = 0; i < this.elements.Count; i++)
                {
                    if (this.elements[i] != null)
                    {
                        Element element = this.elements[i];
                        if (!element.measuredHeight.HasValue)
                            return -1;
                        if (element.measuredHeight.Value > tmpHeight)
                        {
                            //Debug.Log("SCH: " + this.uid + " incrementing height on cell " + i + " to " + element.measuredHeight.Value);
                            tmpHeight = element.measuredHeight.Value;
                        }
                    }
                }
                this._measuredCellHeight = tmpHeight;
            }
            return this._measuredCellHeight.Value;
        }

        private float? _measuredHeight;

        private float? _lastSafeHeightResult;
        public float safeHeight()
        {
            if (this.safeCellHeight() < 0 ||
              (this.extraText != null && !this.extraText.measuredHeight.HasValue) ||
               this.table == null)
            {
                //Debug.Log("MH " + this.uid + ": -1");
                return -1;
            }
            if (!this._measuredHeight.HasValue)
            {
                // See the comment in vertical layout in Control.cs
                //  if the rowVertSpace multiples don't make sense
                float tmpHeight;
                if (this.isHeader)
                    tmpHeight = this.safeCellHeight() +
                      this.table.headerTopMargin + this.table.headerBottomMargin;
                else if (this.isFooter)
                    tmpHeight = this.safeCellHeight() +
                      this.table.footerTopMargin + this.table.footerBottomMargin;
                else
                    tmpHeight = this.safeCellHeight() +
                      this.table.rowVerticalSpacing;
                if (this.safeExtraTextHeight() > 0)
                {
                    tmpHeight += this.safeExtraTextHeight() +
                    this.table.rowVerticalSpacing * 1.5f;
                }
                this._measuredHeight = tmpHeight;
            }

            if (!this._lastSafeHeightResult.HasValue ||
                this._lastSafeHeightResult.Value != this._measuredHeight.Value)
            {
                if (!this.table.data.doingSafeHeightSum)
                    this.table.data.clearSafeHeightSum();
                this._lastSafeHeightResult = this._measuredHeight.Value;
            }

            //Debug.Log("MH " + this.uid + ": " + this._measuredHeight.Value);
            //Thanks greay!
            //(http://forum.unity3d.com/threads/table-pro-1-3-when-your-data-isnt-a-game.347893/#post-2312395)
            if (this._measuredHeight.Value > this.table.minRowHeight) return this._measuredHeight.Value;
            else return this.table.minRowHeight;
        }

        //public bool addedHeight;

        private bool _isHeader;
        private bool _isFooter;

        public bool isHeader { get { return this._isHeader; } }
        public bool isFooter { get { return this._isFooter; } }

        public static Datum Body(string uid)
        {
            Datum d = new Datum(uid);
            return d;
        }

        public static Datum Header()
        {
            Datum d = new Datum("Header_" + Guid.NewGuid().ToString());
            d._isHeader = true;
            return d;
        }

        public static Datum Footer()
        {
            Datum d = new Datum("Footer_" + Guid.NewGuid().ToString());
            d._isFooter = true;
            return d;
        }

        private Datum(string uid)
        {
            this.uid = uid;
            this.elements = new DatumElementList(this);
            this.isDirty = true;
        }
    }

    public class DatumElementList
    {

        private Datum datum;
        private Control control;
        private List<Element> list;

        public int Count
        {
            get { return this.list.Count; }
        }

        public DatumElementList(Datum d)
        {
            this.datum = d;
            this.list = new List<Element>();
        }

        public Element this[int index]
        {
            get
            {
                if (index >= 0 && index < this.list.Count)
                    return this.list[index];
                return null;
            }
        }

        public Element Add(string val, object payload = null)
        {
            Element e = new Element(this.datum, val, payload);
            this.list.Add(e);
            return e;
        }

        public Element Add()
        {
            Element e = new Element(this.datum, null);
            this.list.Add(e);
            return e;
        }

    }

    public class TableDatumList
    {

        private Table table;
        private Control control;
        private List<Datum> list;
        private Dictionary<string, int> indexes;
        private int count;
        public int Count
        {
            get { return this.count; }
        }

        private float? _safeTempRowHeight;
        public float safeTempRowHeight
        {
            get
            {
                // this is a hack to just issue the getter for safeHeightSum if we
                //   need to assign a value for our _avgHeight
                if (!this._avgHeight.HasValue && safeHeightSum == 99.99f) { };
                return Mathf.Max(this._avgHeight.Value, this.table.minRowHeight);
            }
        }

        private bool _doingSafeHeightSum;
        public bool doingSafeHeightSum
        {
            get { return this._doingSafeHeightSum; }
        }

        private float? _avgHeight;
        private float? _safeHeightSum;
        public float safeHeightSum
        {
            get
            {
                if (!this._safeHeightSum.HasValue)
                {
                    this._doingSafeHeightSum = true;
                    float undefCount = 0;
                    float defCount = 0;
                    this._safeHeightSum = 0.0f;
                    for (int i = 0; i < this.count; i++)
                    {
                        Datum d = this.list[i];
                        if (d.safeHeight() >= 0)
                        {
                            this._safeHeightSum += this.list[i].safeHeight();
                            defCount += 1;
                        }
                        else
                            undefCount += 1;
                    } // for
                    if (defCount > 0)
                        this._avgHeight = (this._safeHeightSum / defCount);
                    else
                        this._avgHeight = 0f;
                    this._safeHeightSum += this._avgHeight.Value * undefCount;
                    this._doingSafeHeightSum = false;
                }
                return this._safeHeightSum.Value;
            }
        }

        private bool _changing;
        public bool changing
        {
            get
            {
                return this._changing;
            }
        }

        public TableDatumList(Table table, Control control)
        {
            this.table = table;
            this.control = control;
            this.count = 0;
            this.clearSafeHeightSum();
            this.list = new List<Datum>();
            this.indexes = new Dictionary<string, int>();
        }

        private void initDatum(Datum item, bool isNew = true)
        {
            item.table = this.table;
            if (isNew && this.indexes.ContainsKey(item.uid))
            {
                this.table.error("Datum UID collision: " + item.uid);
            }
            if (item.elements.Count != this.table.columns.Count)
            {
                this.table.error("Datum Column mismatch on UID: " + item.uid);
            }
        }

        public void clearMeasured()
        {
            for (int i = 0; i < this.count; i++)
            {
                // this calls our 'clearSafeHeightSum'
                this.list[i].clearMeasure();
            }
        }

        public void clearSafeHeightSum()
        {
            this._safeHeightSum = null;
            this._avgHeight = null;
        }

        public void clearMeasuredVertPos()
        {
            for (int i = 0; i < this.count; i++)
            {
                // this calls our 'clearSafeHeightSum'
                this.list[i].measuredVertPos = null;
            }
        }

        private void finishDataChange()
        {
            this._changing = false;
            if (this.table.isRunning)
            {
                this.clearMeasured();
                for (int i = 0; i < this.count; i++)
                {
                    this.list[i].isDirty = true;
                }
                this.control.handleDataChange();
            }
        }

        private void rebuildIndexes()
        {
            int i = 0;
            this.indexes.Clear();
            for (i = 0; i < this.list.Count; i++)
            {
                this.indexes.Add(this.list[i].uid, i);
            }
            this.count = i;
            //Debug.Log("Set DS Count to: " + this.count);
        }

        public void Add(Datum item)
        {
            this._changing = true;
            this.initDatum(item);
            this.list.Add(item);
            this.indexes.Add(item.uid, this.count);
            this.count += 1;
            item.animationStartTime = Time.realtimeSinceStartup;
            this.finishDataChange();
        }

        public void Clear()
        {
            this._changing = true;
            this.count = 0;
            this.list.Clear();
            this.indexes.Clear();
            this.finishDataChange();
        }

        public bool Contains(Datum item)
        {
            return this.indexes.ContainsKey(item.uid);
        }

        public bool Remove(string uid)
        {
            this._changing = true;
            bool res = false;
            if (this.indexes.ContainsKey(uid))
            {
                res = true;
                this.list.RemoveAt(this.indexes[uid]);
                this.rebuildIndexes();
                this.finishDataChange();
            }
            else this._changing = false;
            return res;
        }

        public bool Remove(Datum item)
        {
            return this.Remove(item.uid);
        }

        public void Insert(int index, Datum item)
        {
            this._changing = true;
            this.initDatum(item);
            this.list.Insert(index, item);
            this.rebuildIndexes();
            item.animationStartTime = Time.realtimeSinceStartup;
            this.finishDataChange();
        }

        public void RemoveAt(int index)
        {
            this._changing = true;
            if (this.count - 1 >= index)
            {
                this.list.RemoveAt(index);
                this.rebuildIndexes();
                this.finishDataChange();
            }
            else this._changing = false;
        }

        public Datum this[int index]
        {
            get
            {
                if (index >= 0 && index < this.count)
                    return this.list[index];
                return null;
            }
            set
            {
                this._changing = true;
                if (index >= 0 && index < this.count)
                {
                    this.initDatum(value, false);
                    this.list[index] = value;
                    value.animationStartTime = Time.realtimeSinceStartup;
                    this.finishDataChange();
                }
            }
        }

        public int IndexOf(Datum item)
        {
            if (item != null && this.indexes.ContainsKey(item.uid))
                return this.indexes[item.uid];
            else
                return -1;
        }

        public bool Update(Datum item)
        {
            this._changing = true;
            int index = this.IndexOf(item);
            if (index >= 0)
            {
                this.initDatum(item, false);
                this.list[index] = item;
                item.animationStartTime = Time.realtimeSinceStartup;
                this.finishDataChange();
                return true;
            }
            return false;
        }

    }

}

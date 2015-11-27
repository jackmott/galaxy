using UnityEngine;
using UnityEngine.UI;

namespace SLS.Widgets.Table {
public class MeasureMaster : MonoBehaviour {

  public Table table;
  public Control control;
  private Text text;

  public void initialize(Table table, Text text, Control control) {
    this.table = table;
    this.text = text;
    this.control = control;
  }

  public float datumRevision;

  public void doMeasure(Datum d) {

    this.datumRevision = d.revision;
    d.clearMeasure();
    for (int i = 0; i < d.elements.Count; i++) {
      this.measureCell(d, d.elements[i], this.table.columns[i]);
    }
    this.measureCell(d, d.extraText, this.table.extraTextColumn);

    d.isDirty = false;

    //Debug.Log("Completed measure on: " + d.uid + " REV: " + d.revision + " safeHeight: " + d.safeHeight() + " " + d.safeCellHeight());

    if (!d.isHeader && !d.isFooter) {

      // size us based on 100% table settings
      if (this.table.min100PercentWidth || this.table.max100PercentWidth) {

        float bodyw = this.table.bodyRect.rt.rect.width -
          this.table.leftMargin - this.table.rightMargin -
          ((this.table.columns.Count + 1) *
            this.table.horizontalSpacing);

        float wsum = this.table.sumColumnWidths();

        // we are too wide... set us so we try to fit
        // (+1 here cause sometimes wierd rounding on prev loop)
        if (this.table.max100PercentWidth &&
            wsum > bodyw + 1f) {

          //Debug.Log(d.uid + " DOING MAX!: " + wsum + " vs" + bodyw);

          int tries = 0;
          bool hadFudge = true;
          int nonImageCols = 0;

          for (int i = 0; i < this.table.columns.Count; i++) {
            Column c = this.table.columns[i];
            // clear our previously set min (if any)
            c.measuredMinWidth = null;
            if (c.columnType != Column.ColumnType.IMAGE)
              nonImageCols += 1;
          }

          while (hadFudge && tries < 100) {

            //Debug.Log("COL RESIZE: " + tries);

            tries += 1;

            // yeah, i get it... this will be run for the first
            //  iteration and it doesn't need to be.  want to fight about it?
            wsum = this.table.sumColumnWidths();
            hadFudge = false;

            for (int i = 0; i < this.table.columns.Count; i++) {
              Column c = this.table.columns[i];

              if (nonImageCols == 0 ||
                  c.columnType != Column.ColumnType.IMAGE) {
                float usew = (c.safeWidth / wsum) * bodyw;
                if (usew < c.minWidth) {
                  c.measuredMaxWidth = c.minWidth;
                  hadFudge = true;
                  continue;
                }
                else {
                  c.measuredMaxWidth = usew;
                }
              }

            }
          }  // while fudge

          // recurse this fool as it takes a few attempts to get us to fit
          //  sometimes and our width adjusts "carefully"
          d.clearMeasure();
          this.doMeasure(d);
          return;

        } // if max100

        // we are too narrow... set us so we try to fit
        // (-1 here cause sometimes wierd rounding on prev loop)
        if (this.table.min100PercentWidth &&
            wsum < bodyw - 1f) {

          //Debug.Log(d.uid + " DOING MIN!: " + wsum + " vs" + bodyw);

          int nonImageCols = 0;
          float minSum = 0;

          for (int i = 0; i < this.table.columns.Count; i++) {
            Column c = this.table.columns[i];
            // clear our previously set max (if any)
            c.measuredMaxWidth = null;
            if (c.minWidth > 0f)
              minSum += Mathf.Max(c.minWidth);
            if (c.columnType != Column.ColumnType.IMAGE)
              nonImageCols += 1;
          }

          for (int i = 0; i < this.table.columns.Count; i++) {
            Column c = this.table.columns[i];
            float usew = 0;
            if (nonImageCols > 0 && c.columnType == Column.ColumnType.IMAGE)
              usew = c.minWidth;
            else if (minSum > 0)
              usew = (c.minWidth / minSum) * (bodyw - wsum);
            else {
              if (nonImageCols > 0)
                usew = (1f / nonImageCols) * (bodyw - wsum);
              else
                usew = (1f / this.table.columns.Count) * (bodyw - wsum);
            }

            c.measuredMinWidth = c.rawWidth + usew;
            //Debug.Log(d.uid + " Seting measured MW!: " + c.measuredMinWidth.Value + ": " + c.idx);
          }

        } // if min100


      } // if min100 or max 100

      /*
      Row r = this.table.getRowForDatum(d);
      if (r != null) {
        r.checkAnimation();
        this.table.dirtyLater();
      }
      */

    } // if !isHeader and !isFooter

    else if (d.isHeader) {
      /*
      if (d.safeCellHeight() >= 0) {
        this.table.headerRow.checkAnimation();
        //print("HEADER COMPLETE: " + d.uid + ": " + d.safeHeight());
        if (d.safeCellHeight() + this.table.headerTopMargin +
            this.table.headerBottomMargin > this.table.minHeaderHeight) {
          this.table.dirtyLater();
        }
      }
      */
    }
    else {
      /*
      if (d.safeCellHeight() >= 0) {
        this.table.footerRow.checkAnimation();
        //print("FOOTER COMPLETE: " + d.uid + ": " + d.safeHeight());
        if (d.safeCellHeight() + this.table.footerTopMargin +
            this.table.footerBottomMargin > this.table.minFooterHeight) {
          this.table.dirtyLater();
        }
      }
      */
    }

  } // doMeasure

  public void measureCell(Datum d, Element e, Column c) {

    if (c.columnType == Column.ColumnType.TEXT || d.isHeader || d.isFooter) {
      if (e == null) {
        this.measureCellDone(c, d, e);
        return;
      }
      if (string.IsNullOrEmpty(e.value)) {
        e.measuredWidth = 0;
        e.measuredHeight = 0;
        this.measureCellDone(c, d, e);
        return;
      }

      TextGenerationSettings settings;
      float useW;
      float mw;
      float mh;
      float pixelsPerUnit;
      Vector2 size;
      this.text.fontSize = c.calcFont(d);
      this.text.text = e.value;

      if (c.measuredMaxWidth.HasValue) {
        this.text.horizontalOverflow = HorizontalWrapMode.Wrap;
        size = new Vector2(c.measuredMaxWidth.Value, 0);
      }
      else {
        this.text.horizontalOverflow = HorizontalWrapMode.Overflow;
        size = Vector2.zero;
      }

      settings = this.text.GetGenerationSettings(size);
      pixelsPerUnit = this.text.pixelsPerUnit;

      mw = this.text.cachedTextGeneratorForLayout.GetPreferredWidth
        (e.value, settings) / pixelsPerUnit;
      mh = this.text.cachedTextGeneratorForLayout.GetPreferredHeight
        (e.value, settings) / pixelsPerUnit;

     //if (c.idx == 0) Debug.Log("Col0 Width: " + mw + " Row: " + d.uid + " Value: " + e.value);

      useW = c.maxWidth;
      if (c.measuredMaxWidth.HasValue &&
          (c.measuredMaxWidth.Value < useW || useW <= 0))
        useW = c.measuredMaxWidth.Value;

      if (useW > 0 && mw > useW) {

        //Debug.Log("Doing Max Width on " + c.idx + " Row: " + d.uid + " Width: " + useW);

        this.text.horizontalOverflow = HorizontalWrapMode.Wrap;
        size = new Vector2(useW, 0);

        settings = this.text.GetGenerationSettings(size);
        pixelsPerUnit = this.text.pixelsPerUnit;

        mw = this.text.cachedTextGeneratorForLayout.GetPreferredWidth
          (e.value, settings) / pixelsPerUnit;
        mh = this.text.cachedTextGeneratorForLayout.GetPreferredHeight
          (e.value, settings) / pixelsPerUnit;
      }

     //if (c.idx == 0) Debug.Log("Col0 Width: " + mw + " Row: " + d.uid + " Value: " + e.value);

      mw = Mathf.Max(c.minWidth, mw);

      //if (c.idx == 0) Debug.Log("Col0 Width: " + mw + " Row: " + d.uid + " Value: " + e.value);

      if (c.measuredMinWidth.HasValue && c.measuredMinWidth.Value > mw)
        mw = c.measuredMinWidth.Value;

      //if (c.idx == 0) Debug.Log("Col0 Width: " + mw + " Row: " + d.uid + " Value: " + e.value);

      e.measuredWidth = mw;
      e.measuredHeight = mh;
      this.measureCellDone(c, d, e);

    }  // if TEXT
    else {
      e.measuredWidth = c.imageWidth;
      e.measuredHeight = c.imageHeight;
      this.measureCellDone(c, d, e);
    }
  }

  private void measureCellDone(Column c, Datum d, Element e) {
    if (c != null && d != null && e != null)
    //if (c.idx == 0) Debug.Log("DONE MEASURE: " + d.uid + ": " + c.idx + ": " + e.value + " h: " + e.measuredHeight + " w: " + e.measuredWidth + " fzise: " + this.text.fontSize + " PPU: " + this.text.pixelsPerUnit);
    if (e != null && c != null &&
        (d != null && d.revision == this.datumRevision)) {
      if (d != null && d.isHeader && d.table.hasHeaderIcons)
        c.checkWidth(e.measuredWidth.Value +
          d.table.rowVerticalSpacing * 0.5f +
          d.table.headerIconWidth);
      else
        c.checkWidth(e.measuredWidth.Value);
    }
  } // measureCallback

} // MeasureMaster
}

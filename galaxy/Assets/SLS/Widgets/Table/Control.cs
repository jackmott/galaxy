using UnityEngine;
using System;

namespace SLS.Widgets.Table {
public class Control {

  private Factory factory;
  private Table table;
  public Control(Table table, Factory factory) {
    this.table = table;
    this.factory = factory;
  }

  public void draw() {
    if (this.table.hasError) return;
    this.table.bodyScroller.onValueChanged.RemoveAllListeners();
    this.table.bodyScroller.onValueChanged.AddListener(this.onBodyScroll);
    this.checkSizerVerticalSize();
    this.sizeForRectTransform();
    this.onBodyScroll(Vector2.zero);
  }

  public void handleDataChange() {
    this.checkSizerVerticalSize();
    this.onBodyScroll(Vector2.zero);
  }

  private class VerticalControl {
    public int top;
    public float verticalPosition;
  }

  private VerticalControl getVerticalControl() {
    VerticalControl res = new VerticalControl();
    float vscroll = 1f - this.table.bodyScroller.verticalNormalizedPosition;
    float visY = ((this.table.bodySizer.rect.height - this.table.bodyRect.rt.rect.height) * vscroll);

    int i = 0;
    float useH = 0;
    bool hadUndef = false;

    // use our cached vpos if available
    Datum lastD;
    Datum d = this.table.rows[0].datum;
    i = this.table.data.IndexOf(d);

    if (d != null && d.measuredVertPos.HasValue && i >= 0) {

      useH = d.measuredVertPos.Value;

      //Debug.Log("CHECKING: " + useH + " : " +
      //    visY + " : " + i);

      // if our top visible row (useH) is 'below' or screen top
      //   so we need to move our top visible row 'up'
      if (useH > visY) {
        while (d != null && d.measuredVertPos.HasValue) {
          useH = d.measuredVertPos.Value;
          //Debug.Log("trying cached BELOW result: " + useH + " : " +
          //          visY + " : " + i);
          if (useH <= visY) {
            res.top = i;
            res.verticalPosition = useH;
            //Debug.Log("returning cached below val: " + useH + " : " +
            //        visY + " : " + i);
            return res;
          }
          i -= 1;
          if (i >= 0)
            d = this.table.data[i];
          else
            d = null;
        }
      } // if useH > visY


      // else our top visible row (useH) is 'above' our screen top
      //   so we need to move our top visible row 'down'
      else {
        while (d != null && d.measuredVertPos.HasValue) {
          useH = d.measuredVertPos.Value;
          //Debug.Log("trying cached ABOVE result: " + useH + " : " +
          //          visY + " : " + i);
          if (useH > visY || i == this.table.data.Count - 1) {
            d = this.table.data[i - 1];
            if (d == null) break;
            // this handles the special case where we have a data update
            //  and haven't measured yet, or we contain no data
            if (!d.measuredVertPos.HasValue) break;
            res.top = i - 1;
            res.verticalPosition = d.measuredVertPos.Value;
            //Debug.Log("returning from ABOVE");
            return res;
          }
          i += 1;
          if (i < this.table.data.Count) {
            lastD = d;
            d = this.table.data[i];
            if (!d.measuredVertPos.HasValue &&
                d.safeHeight() >= 0 &&
                lastD.safeHeight() >= 0)
              d.measuredVertPos = useH +
                Mathf.Max(this.table.minRowHeight, lastD.safeHeight());
            //Debug.Log(i + " HasValue " + d.measuredVertPos.HasValue);
          }
          else {
            d = this.table.data[i - 1];
            // this handles the special case where we have a data update
            //  and haven't measured yet
            if (!d.measuredVertPos.HasValue) break;
            res.top = i - 1;
            res.verticalPosition = d.measuredVertPos.Value;
            //Debug.Log("returning LAST ROW from ABOVE");
            return res;
          }
        }
      } // else (useH <= visY)
    } // if d!= null & d.measuredVertPos.HasValue

    //Debug.LogWarning("CACHE FAILED!");

    // else if we didn't return anything yet,
    //    brute force and cache what we can
    useH = 0;
    for (i = 0; i < this.table.data.Count; i++ ) {
      if (this.table.data[i].safeHeight() >= 0) {
        if (!hadUndef) {
          this.table.data[i].measuredVertPos = res.verticalPosition;
        }
        useH = Mathf.Max(this.table.minRowHeight,
          this.table.data[i].safeHeight());
      }
      else {
        hadUndef = true;
        useH = this.table.data.safeTempRowHeight;
      }
      if (res.verticalPosition + useH > visY) {
        res.top = i;
        return res;
      }
      else
        res.verticalPosition += useH;
    }
    res.top = Math.Max(0, this.table.data.Count - 1);
    return res;

  }

  public void onBodyScroll(Vector2 val) {

    if (!this.table.isRunning) return;

    if (this.table.hasHeader)
      this.table.headerRow.rt.localPosition =
        new Vector3(this.table.bodySizer.localPosition.x, 0f);
    if (this.table.hasFooter)
      this.table.footerRow.rt.localPosition =
        new Vector3(this.table.bodySizer.localPosition.x, 0f);
    if (this.table.hasColumnOverlay)
      this.table.columnOverlayContent.localPosition =
        new Vector3(this.table.bodySizer.localPosition.x, 0f);

    VerticalControl res = this.getVerticalControl();
    //Debug.Log("TOP: " + res.top + " VPOS: " + res.verticalPosition);

    if (this.table.rows.Count > 0)
      this.table.rows[0].rt.localPosition =
        new Vector3(0, res.verticalPosition * -1f);

    for (int i = 0; i < this.table.rows.Count; i++) {
      Row row = this.table.rows[i];
      if (i + res.top >= this.table.data.Count) {
        row.gameObject.SetActive(false);
      }
      else {
        row.gameObject.SetActive(true);
        row.datum = this.table.data[res.top + i];
      }
      this.horPosExtraText(row);
    }

  }

  private void horPosExtraText(Row row) {
    if (row.datum != null && row.datum.extraText != null
        && row.datum.safeHeight() > 0) {
      float x = ((this.table.bodySizer.rect.width -
        this.table.bodyRect.rt.rect.width) *
        this.table.bodyScroller.horizontalNormalizedPosition);
      row.extraTextRt.localPosition = new Vector3
        ((this.table.root.rect.width * 0.5f) -
         (row.extraTextRt.sizeDelta.x * 0.5f) + x,
         row.extraTextRt.localPosition.y);
    }
  }

  private void checkSizerVerticalSize() {
    //Debug.Log("CSVS: " + this.table.data.safeHeightSum + " : " + this.safeTempRowHeight);
    float before = this.table.bodySizer.sizeDelta.y;
    this.table.bodySizer.sizeDelta = new Vector2
      (this.table.bodySizer.sizeDelta.x, this.table.data.safeHeightSum);
    if (before != this.table.bodySizer.sizeDelta.y)
      this.table.dirtyNow();
  }

  public void setLayoutVertical() {

    if (!this.table.isRunning) return;

    //Debug.Log("SLV:" + Time.realtimeSinceStartup + ": " +
    //  this.table.availableMeasures.Count);

    RectTransform rt;
    float useH;
    float useHeaderH = 0;
    float useFooterH = 0;

    // Position header/body/footer main blocks
    if (this.table.hasHeader) {
      useHeaderH = this.table.minHeaderHeight;
      if (this.table.headerRow.datum.safeCellHeight() > 0)
        useHeaderH = Mathf.Max(this.table.minHeaderHeight,
           this.table.headerRow.datum.safeCellHeight() +
           this.table.headerTopMargin + this.table.headerBottomMargin);
      this.table.headerRect.offsetMin = new Vector2(0, useHeaderH * -1f);
      this.table.headerRect.offsetMax = new Vector2(0, 0);
      this.table.headerRow.rt.sizeDelta = new Vector2
        (this.table.headerRow.rt.sizeDelta.x, useHeaderH);
    }
    if (this.table.hasFooter) {
      useFooterH = this.table.minFooterHeight;
      if (this.table.footerRow.datum.safeCellHeight() > 0)
        useFooterH = Mathf.Max(this.table.minFooterHeight,
           this.table.footerRow.datum.safeCellHeight() +
           this.table.footerTopMargin + this.table.footerBottomMargin);
      this.table.footerRect.offsetMin = new Vector2(0, this.table.root.rect.height * -1f);
      this.table.footerRect.offsetMax =
        new Vector2(0, (this.table.root.rect.height - useFooterH) * -1f);
      this.table.footerRow.rt.sizeDelta = new Vector2
        (this.table.footerRow.rt.sizeDelta.x, useFooterH);
    }
    this.table.bodyRect.rt.offsetMin = new Vector2(0, useFooterH);
    this.table.bodyRect.rt.offsetMax = new Vector2(0, useHeaderH * -1f);

    //Debug.Log("set br offsets: " + this.table.bodySizer.rect.width + " vs " + this.table.bodyRect.rt.rect.width);

    this.table.horScrollerRt.offsetMin = new Vector2(10f, useFooterH);
    this.table.horScrollerRt.offsetMax = new Vector2(-10f, useFooterH + 10f);
    // because our scrollrect for some reason does wierd scaling
    this.table.horScrollerRt.localScale = Vector3.one;

    this.table.verScrollerRt.offsetMin = new Vector2(-10f, useFooterH + 10f);
    this.table.verScrollerRt.offsetMax = new Vector2
      (0, (useHeaderH + 10f) * -1f);
    // because our scrollrect for some reason does wierd scaling
    this.table.verScrollerRt.localScale = Vector3.one;

    /* Row Vertical Layout ********
    | [ Row Border Line
    | > rowVerticalSpacing / 2
    -
    | [ Row cells
    | > measuredHeight
    _
    .......... if Has Extra Text
    |
    | > rowVerticalSpacing / 2
    _
    | [ ExtraTextBox
        |
        | > rowVerticalSpacing / 2
        _
        | [ extraText
        | > extaTextHeight
        _
        |
        | > rowVerticalSpacing / 2
    |
    _
    .......... end if Has Extra Text
    |
    | > rowVerticalSpacing / 2
     *****************************/

    bool allRowsHaveHeght = true;

    for (int i = 0; i < this.table.rows.Count; i++) {
      Row row = this.table.rows[i];
      if (row.datum != null) {

        if (row.datum.safeHeight() >= 0) {

          useH = Mathf.Max
            (this.table.minRowHeight, row.datum.safeHeight());

          if (row.datum.safeExtraTextHeight() > 0) {
            if (row.datum.safeHeight() > this.table.minRowHeight) {
              row.extraTextRt.localPosition = new Vector3
                (row.extraTextRt.localPosition.x,
                (this.table.rowVerticalSpacing +
                 row.datum.safeCellHeight()) * -1f);
            }
            else {
              //logic here for when our extra text still
              //  doesn't make our height > our min
              row.extraTextRt.localPosition = new Vector3
                (row.extraTextRt.localPosition.x,
                (this.table.minRowHeight -
                  row.datum.safeExtraTextHeight() -
                  this.table.rowVerticalSpacing * 1.5f) * -1f);
               Debug.Log(row.extraTextRt.localPosition.y);
            }
            row.extraTextRt.sizeDelta = new Vector2
              (row.extraTextRt.sizeDelta.x,
               this.table.rowVerticalSpacing +
               row.datum.safeExtraTextHeight());
          }
        }
        else {
          //Debug.Log("didn't have a safe height on row: " + row.datum.uid);
          allRowsHaveHeght = false;
          useH = this.table.data.safeTempRowHeight;
        }

        row.rt.sizeDelta = new Vector2(row.rt.sizeDelta.x, useH);
      }
      if (i > 0) {
        row.rt.localPosition = new Vector3
          (0, row.preceedingRow.rt.rect.height * -1f +
           row.preceedingRow.rt.localPosition.y);
      }
    } // for i in rows.Count

    if (allRowsHaveHeght || this.table.rows.Count == 0) {
      this.table.fadeOverlay(0.3f, 1, 0, 0.0f);
    }

    // Position column lines
    if (this.table.hasColumnOverlay) {
      for (int i = 0; i < this.table.columnOverlayLines.Count; i++) {
        rt = this.table.columnOverlayLines[i];
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, this.table.root.rect.height);
      }
    }

    this.checkSizerVerticalSize();

    // Check our Scrollbar visibility
    if (this.table.bodySizer.rect.height > this.table.bodyRect.rt.rect.height) {
      this.table.setGameObjectActiveLater
        (this.table.bodyScroller.verticalScrollbar.gameObject, true);
    }
    else {
      this.table.setGameObjectActiveLater
        (this.table.bodyScroller.verticalScrollbar.gameObject, false);
    }

 }

  public void setLayoutHorizontal() {

    if (!this.table.isRunning) return;

    //Debug.Log("SLH:" + Time.realtimeSinceStartup + ": " +
    //  this.table.availableMeasures.Count);

    float hPos;
    float useW;

    if (this.table.hasHeader) {
      useW = this.layoutRow(this.table.headerRow, this.table.minHeaderHeight);
      this.table.headerRow.rt.sizeDelta = new Vector2
        (useW, this.table.headerRow.rt.sizeDelta.y);
    }
    if (this.table.hasFooter){
      useW = this.layoutRow(this.table.footerRow, this.table.minFooterHeight);
      this.table.footerRow.rt.sizeDelta = new Vector2
        (useW, this.table.footerRow.rt.sizeDelta.y);
    }

    useW = 0;
    hPos = 0;
    for (int r = 0; r < this.table.rows.Count; r++) {
      Row row = this.table.rows[r];
      hPos = this.layoutRow(row, this.table.minRowHeight);
      hPos = Mathf.Max(hPos, this.table.bodyRect.rt.rect.width);
      row.rt.sizeDelta = new Vector2(hPos, row.rt.sizeDelta.y);
      useW = Mathf.Max(useW, hPos);
    }
    this.table.bodySizer.sizeDelta = new Vector2(
      useW, this.table.bodySizer.sizeDelta.y);

    if (this.table.hasColumnOverlay) {
      hPos = this.table.leftMargin;
      for (int i = 0; i < this.table.columns.Count; i++) {
        Column column = this.table.columns[i];
        hPos += column.safeWidth;
        if (i < this.table.columns.Count - 1) {
          this.table.columnOverlayLines[i].localPosition =
            new Vector3(hPos + this.table.horizontalSpacing * 0.5f, 0f);
          hPos += this.table.horizontalSpacing;
        }
      }
    }

    //Debug.Log("VSCROLL: " + this.table.bodySizer.rect.width + " vs " + this.table.bodyRect.rt.rect.width);

    // Check our Scrollbar visibility
    if (this.table.bodySizer.rect.width > this.table.bodyRect.rt.rect.width) {
      this.table.setGameObjectActiveLater
        (this.table.bodyScroller.horizontalScrollbar.gameObject, true);
    }
    else {
      this.table.setGameObjectActiveLater
        (this.table.bodyScroller.horizontalScrollbar.gameObject, false);
    }
  }

  private float layoutRow(Row row, float minH) {

    float useW;
    float hPos = 0;

    if (row.datum != null && row.datum.safeHeight() >= 0 && row.datum.safeExtraTextHeight() > 0) {
      row.extraTextRt.sizeDelta = new Vector2((this.table.root.rect.width * this.table.extraTextWidthRatio) + this.table.horizontalSpacing, row.extraTextRt.sizeDelta.y);
      this.horPosExtraText(row);
    }

    for (int i = 0; i < row.cells.Count; i++) {
      Cell cell = row.cells[i];

      if (this.table.columns.Count - 1 < i) break;

      if (row.datum != null) {

        if (i == 0)
          useW = this.table.columns[i].safeWidth +
            this.table.leftMargin + this.table.horizontalSpacing * 0.5f;
        else
          useW = this.table.columns[i].safeWidth +
            this.table.horizontalSpacing;
        cell.rt.sizeDelta = new Vector2(useW, row.datum.safeHeight());
        cell.rt.localPosition = new Vector3(hPos, 0);

        if (i == 0) {
          useW =
            this.table.leftMargin;
        }
        else {
          useW = this.table.horizontalSpacing * 0.5f;
        }
        if (row.datum.isHeader && this.table.hasHeaderIcons)
          cell.setContentSizeDelta(new Vector2(this.table.columns[i].safeWidth - this.table.headerIconWidth,
            row.datum.safeCellHeight()));
        else {
          cell.setContentSizeDelta(new Vector2(this.table.columns[i].safeWidth, row.datum.safeCellHeight()));
        }

        if (row.datum.isHeader) {
          cell.setContentLocalPosition(useW, this.table.headerTopMargin * -1f);
        }
        else if (row.datum.isFooter) {
          cell.setContentLocalPosition(useW, this.table.footerTopMargin * -1f);
        }
        else {
          cell.setContentLocalPosition(useW, this.table.rowVerticalSpacing * -0.5f);
        }

/*
        if (row.datum.isHeader) {
          HeaderCell hc = cell as HeaderCell;
          hc.icon.rectTransform.localPosition = new Vector2
            (cell.rt.rect.width - this.table.rowVerticalSpacing * -0.5f -
            this.table.headerIconWidth, (row.datum.safeCellHeight() * -0.5f) +
            (this.table.headerTopMargin - this.table.headerBottomMargin) * -0.5f);
        }
        */

        
        hPos += cell.rt.rect.width;
      }
    }
    hPos += this.table.rightMargin;
    return hPos;
  }

  public void sizeForRectTransform() {
    if (this.table == null || this.factory == null) return;
    this.table.extraTextColumn.measuredMaxWidth =
      (this.table.root.rect.width * this.table.extraTextWidthRatio);
    this.factory.makeRows();
  }

}

}

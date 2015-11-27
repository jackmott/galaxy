Thanks for using Table Pro!

Simple instructions to get started:

  1. Create an 'empty' UI element and anchor it however you wish.
  2. Add our Table script to your empty UI element.
  3. In one of your scripts, just access and work with the Table
     component something like the simple example below.

Please see our other samples in the included Samples directory or visit
  http://semi-legitimate.com/tablepro/ for additional documentation!

Sample Usage:

using UnityEngine;
using SLS.Widgets.Table;

public class Simple : MonoBehaviour {

  private Table table;

  void Start () {

    this.table = this.GetComponent<Table>();

    this.table.reset();

    this.table.addTextColumn();
    this.table.addTextColumn();
    this.table.addTextColumn();

    // Initialize Your Table
    this.table.initialize(this.onTableSelectedWithCol);

    // Populate Your Rows (obviously this would be real data here)
    for (int i = 0; i < 100; i++) {
      Datum d = Datum.Body(i.ToString());
      d.elements.Add("Col1:Row" + i.ToString());
      d.elements.Add("Col2:Row" + i.ToString());
      d.elements.Add("Col3:Row" + i.ToString());
      this.table.data.Add(d);
    }

    // Draw Your Table
    this.table.startRenderEngine();

  }

  // Handle the row selection however you wish (be careful as column can be null here
  //  if your table doesn't fill the full horizontal rect space and the user clicks an edge)
  private void onTableSelectedWithCol(Datum datum, Column column) {
    string cidx = "N/A";
    if (column != null) cidx = column.idx.ToString();
    print("You Clicked: " + datum.uid + " Column: " + cidx);
  }

}
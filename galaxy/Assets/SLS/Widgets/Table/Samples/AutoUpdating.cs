using UnityEngine;
using System.Collections;

namespace SLS.Widgets.Table {
public class AutoUpdating : MonoBehaviour {

  private Table table;

  void Start () {

    this.table = this.GetComponent<Table>();

    this.table.reset();

    this.table.addTextColumn("h1", "f1");
    this.table.addTextColumn("h2", "f2");
    this.table.addTextColumn("h3", "f3");
    this.table.addTextColumn("h4", "f4");
    this.table.addTextColumn("h5", "f5");

    this.table.initialize();

    // Populate Your Rows (obviously this would be real data here)
    for (int i = 0; i < 10; i++) {
      this.table.data.Add(this.makeDatum("INIT"));
    }

    // Draw Your Table
    this.table.startRenderEngine();

    StartCoroutine(this.doRandomData());

  }

  private Datum makeDatum(string pfx) {
    string sfx = Time.realtimeSinceStartup.ToString();
    Datum d = Datum.Body(sfx);
    d.elements.Add("Col1:" + pfx + ":" + sfx);
    d.elements.Add("Col2:" + pfx + ":" + sfx);
    d.elements.Add("Col3:" + pfx + ":" + sfx);
    d.elements.Add("Col4:" + pfx + ":" + sfx);
    d.elements.Add("Col5:" + pfx + ":" + sfx);
    return d;
  }

  IEnumerator doRandomData() {
    yield return new WaitForSeconds(2f);
    while (true) {
      float action = Random.Range(0, 50);
      if (action < 5) {
        this.table.data.Add(this.makeDatum("ADD"));
      }
      else if (action < 10) {
        this.table.data.Insert(0, this.makeDatum("TOP"));
      }
      else if (action < 15) {
        int idx = Random.Range(0, this.table.data.Count);
        this.table.data.Insert(idx, this.makeDatum("INS"));
      }
      else if (action < 20) {
        if (this.table.data.Count > 0) {
          int idx = Random.Range(0, this.table.data.Count);
          this.table.data.RemoveAt(idx);
        }
      }
      else if (action < 25) {
        int cidx = Random.Range(0, this.table.columns.Count);
        this.table.columns[cidx].headerValue =
          "UPD:" + Time.realtimeSinceStartup.ToString();
      }
      else if (action < 30) {
        int cidx = Random.Range(0, this.table.columns.Count);
        this.table.columns[cidx].footerValue =
          "UPD:" + Time.realtimeSinceStartup.ToString();
      }
      else {
        if (this.table.data.Count > 0) {
          int ridx = Random.Range(0, this.table.data.Count);
          int cidx = Random.Range(0, this.table.columns.Count);
          this.table.data[ridx].elements[cidx].value =
            "UPD:" + Time.realtimeSinceStartup.ToString();
        }
      }
      yield return new WaitForSeconds(Random.Range(0, 1));
    }
  }

/*
  void Update() {
    if (this.table != null && this.table.columns != null &&
        this.table.columns.Count > 0) {
      this.table.columns[0].headerText = Time.realtimeSinceStartup.ToString();
      this.table.columns[0].headerIconColor =
      new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
      this.table.columns[0].footerText = Time.realtimeSinceStartup.ToString();
    }

    if (this.table != null && this.table.data != null &&
        this.table.data.Count > 0) {
      //print("doing it");
      this.table.data[0].elements[0].value = Time.realtimeSinceStartup.ToString();
    }
  }
     */

}
}